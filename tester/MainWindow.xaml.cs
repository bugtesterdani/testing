using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System.Configuration;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
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
        private bool _isDraggingTeil; private bool _isDraggingLine; private Point _startPoint; private Teil? _selectedTeil = null; public MainWindow() { InitializeComponent(); }
        private void Rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Ellipse ellipse && ellipse.DataContext is Knotenpunkt knoten && !_isDraggingTeil)
            {
                if (knoten.IsConnected)
                    return;
                _isDraggingLine = true;
                _startKnoten = knoten;
                _startPoint = e.GetPosition(DesignerCanvas);

                // Create a new line
                _currentLine = new Line
                {
                    Stroke = Brushes.Black,
                    StrokeThickness = 2,
                    X1 = _startPoint.X,
                    Y1 = _startPoint.Y,
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
                var currentPosition = e.GetPosition(DesignerCanvas); var offsetX = currentPosition.X - _startPoint.X; var offsetY = currentPosition.Y - _startPoint.Y; var newX = _selectedTeil.PositionX + offsetX; var newY = _selectedTeil.PositionY + offsetY; ((MainWindowViewModel)DataContext).UpdatePosition(_selectedTeil, newX, newY); _startPoint = currentPosition;
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
                _isDraggingTeil = false; _selectedTeil = null; Mouse.Capture(null);
            }
            if (_isDraggingLine && _currentLine != null)
            {
                _isDraggingLine = false;
                var currentPosition = e.GetPosition(DesignerCanvas);
                var hitTestResult = VisualTreeHelper.HitTest(DesignerCanvas, currentPosition);

                if (hitTestResult?.VisualHit is Ellipse ellipse && ellipse.DataContext is Knotenpunkt endKnoten)
                {
                    _startKnoten.Dock(endKnoten);
                }
                else if (hitTestResult?.VisualHit is Line line)
                {
                    // If we hit a line, we need to find the closest Ellipse (Knotenpunkt)
                    foreach (var child in DesignerCanvas.Children)
                    {
                        var item = child;
                        if (item is ItemsControl itControl)
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
                                                    if (SubParse(currentPosition, potentialEllipse, Canvas.GetLeft(c2) + Canvas.GetLeft(c), Canvas.GetTop(c2) + Canvas.GetTop(c)))
                                                        break;
                                            }
                            }
                        else if (item is Ellipse potentialEllipse)
                            if (SubParse(currentPosition, potentialEllipse))
                                break;
                    }
                }

                DesignerCanvas.Children.Remove(_currentLine);
                _currentLine = null;
                Mouse.Capture(null);
            }
        }

        private bool SubParse(Point currentPosition, Ellipse potentialEllipse, double defaultvalueX = 0, double defaultvalueY = 0)
        {
            if (potentialEllipse.DataContext is not Knotenpunkt potentialKnoten)
                return false;
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
                _startKnoten.Dock(potentialKnoten);
                // Update the line's end position to match the docked position
                _currentLine.X2 = ellipseCenterX;
                _currentLine.Y2 = ellipseCenterY;
                return true;
            }
            return false;
        }

        private void Rectangle_Loaded(object sender, RoutedEventArgs e) { if (sender is Rectangle rectangle && rectangle.DataContext is Teil teil) { Canvas.SetLeft(rectangle, teil.PositionX); Canvas.SetTop(rectangle, teil.PositionY); } }
    }
}