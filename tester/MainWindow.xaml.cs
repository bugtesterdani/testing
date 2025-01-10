using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace tester
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Knotenpunkt _startKnoten;
        private Line _currentLine;
        private bool _isDraggingTeil;
        private bool _isDraggingLine;
        private Point _startPoint;
        private Teil? _selectedTeil = null;
        public MainWindow()
        {
            InitializeComponent();
        }
        
        private void Rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Ellipse ellipse && ellipse.DataContext is Knotenpunkt knoten && !_isDraggingTeil)
            {
                if (knoten.IsConnected)
                    return;
                _isDraggingLine = true;
                _startKnoten = knoten;
                _startPoint = e.GetPosition(DesignerCanvas);

                Point _p = new();

                // If we hit a line, we need to find the closest Ellipse (Knotenpunkt)
                if (Looking(_startPoint, false) is Tuple<bool, Point> outval && outval.Item1)
                    _p = outval.Item2;

                // Create a new line
                _currentLine = new Line
                {
                    Stroke = Brushes.Black,
                    StrokeThickness = 2,
                    X1 = _p.X,
                    Y1 = _p.Y,
                    X2 = _startPoint.X,
                    Y2 = _startPoint.Y
                };
                DesignerCanvas.Children.Add(_currentLine);

                ellipse.CaptureMouse();
            }
            else if (sender is FrameworkElement element && element.DataContext is Teil teil && !_isDraggingLine)
            {
                _isDraggingTeil = true;
                _startPoint = e.GetPosition(DesignerCanvas);
                _selectedTeil = teil;
                element.CaptureMouse();
            }
        }
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDraggingTeil && _selectedTeil != null)
            {
                var currentPosition = e.GetPosition(DesignerCanvas);
                var offsetX = currentPosition.X - _startPoint.X;
                var offsetY = currentPosition.Y - _startPoint.Y;
                _startPoint = currentPosition;
                MainWindowViewModel viewmodel = (MainWindowViewModel)DataContext;
                foreach (var knoten in _selectedTeil.Knoten)
                    if (knoten.IsConnected)
                    {
                        if (knoten.selectedpos == Positionselected.Start)
                        {
                            Point Start = viewmodel.LineDic[knoten.LineID].StartPoint;
                            viewmodel.LineDic[knoten.LineID].StartPoint = new Point(Start.X + offsetX, Start.Y + offsetY);
                        }
                        else if (knoten.selectedpos == Positionselected.End)
                        {
                            Point End = viewmodel.LineDic[knoten.LineID].EndPoint;
                            viewmodel.LineDic[knoten.LineID].EndPoint = new Point(End.X + offsetX, End.Y + offsetY);
                        }
                    }

                viewmodel.UpdatePosition(_selectedTeil, _selectedTeil.PositionX + offsetX, _selectedTeil.PositionY + offsetY);
            }
            if (_isDraggingLine && _currentLine != null)
            {
                var currentPosition = e.GetPosition(DesignerCanvas);
                _currentLine.X2 = currentPosition.X;
                _currentLine.Y2 = currentPosition.Y;
            }
        }
        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isDraggingTeil && _selectedTeil != null)
            {
                _isDraggingTeil = false;
                _selectedTeil = null;
                Mouse.Capture(null);
            }
            if (_isDraggingLine && _currentLine != null)
            {
                _isDraggingLine = false;
                var currentPosition = e.GetPosition(DesignerCanvas);
                var hitTestResult = VisualTreeHelper.HitTest(DesignerCanvas, currentPosition);

                if (hitTestResult?.VisualHit is Ellipse ellipse && ellipse.DataContext is Knotenpunkt endKnoten)
                    AddDicPosKnoten(endKnoten, currentPosition);
                else if (hitTestResult?.VisualHit is Line line)
                    Looking(currentPosition, true);

                DesignerCanvas.Children.Remove(_currentLine);
                _currentLine = null;
                Mouse.Capture(null);
            }
        }

        private Tuple<bool, Point> Looking(Point currentPosition, bool docklineon)
        {
            foreach (var child in DesignerCanvas.Children)
            {
                if (child is ItemsControl itControl)
                    foreach (var subitem in itControl.Items)
                    {
                        ContentPresenter c = (ContentPresenter)itControl.ItemContainerGenerator.ContainerFromItem(subitem);
                        if (c.ContentTemplate.FindName("BoxCanvas", c) is Canvas potentialTeil)
                            foreach (var child2 in potentialTeil.Children)
                                if (child2 is ItemsControl itControl2)
                                    foreach (var subitem2 in itControl2.Items)
                                    {
                                        ContentPresenter c2 = (ContentPresenter)itControl2.ItemContainerGenerator.ContainerFromItem(subitem2);
                                        if (c2.ContentTemplate.FindName("PointConnect", c2) is Ellipse potentialEllipse)
                                            if (SubParse(currentPosition, potentialEllipse, Canvas.GetLeft(c2) + Canvas.GetLeft(c), Canvas.GetTop(c2) + Canvas.GetTop(c), docklineon: docklineon) is Tuple<bool, Point> outval && outval.Item1)
                                                return outval;
                                    }
                    }
                else if (child is Ellipse potentialEllipse)
                    if (SubParse(currentPosition, potentialEllipse, docklineon: docklineon) is Tuple<bool, Point> outval && outval.Item1)
                        return outval;
            }
            return new(false, new());
        }

        private void AddDicPosKnoten(Knotenpunkt potentialKnoten, Point Endpoint)
        {
            int id = ((MainWindowViewModel)DataContext).LineDic.Count + 1;
            _startKnoten.Dock(potentialKnoten, id);
            Dictionary<int, Lines> _tmplines = ((MainWindowViewModel)DataContext).LineDic;
            _tmplines.Add(id, new Lines { StartPoint = _startPoint, EndPoint = Endpoint });
            _startKnoten.selectedpos = Positionselected.Start;
            potentialKnoten.selectedpos = Positionselected.End;
            ((MainWindowViewModel)DataContext).LineDic = _tmplines;
        }

        private Tuple<bool, Point> SubParse(Point currentPosition, Ellipse potentialEllipse, double defaultvalueX = 0, double defaultvalueY = 0, bool docklineon = false)
        {
            if (potentialEllipse.DataContext is not Knotenpunkt potentialKnoten)
                return new(false, new());
            var ellipseCenterX = Canvas.GetLeft(potentialEllipse);
            if (double.IsNaN(ellipseCenterX))
                ellipseCenterX = defaultvalueX;
            ellipseCenterX += potentialEllipse.Width / 2;
            var ellipseCenterY = Canvas.GetTop(potentialEllipse);
            if (double.IsNaN(ellipseCenterY))
                ellipseCenterY = defaultvalueY;
            ellipseCenterY += potentialEllipse.Height / 2;
            if (Math.Abs(currentPosition.X - ellipseCenterX) < 10 && Math.Abs(currentPosition.Y - ellipseCenterY) < 10)
            {
                if (docklineon)
                {
                    if (_startKnoten.selectedpos != potentialKnoten.selectedpos)
                        AddDicPosKnoten(potentialKnoten, new(ellipseCenterX, ellipseCenterY));
                    else
                        return new(false, new());
                }
                return new(true, new(ellipseCenterX, ellipseCenterY));
            }
            return new(false, new());
        }
    }
}