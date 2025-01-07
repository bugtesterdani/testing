using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
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
        private bool _isDragging; private Point _startPoint; private Teil _selectedTeil; public MainWindow() { InitializeComponent(); }
        private void Rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is Teil teil)
            {
                _isDragging = true;
                _startPoint = e.GetPosition(DesignerCanvas);
                _selectedTeil = teil;
                element.CaptureMouse();
            }
        }
        private void Window_MouseMove(object sender, MouseEventArgs e) { if (_isDragging && _selectedTeil != null) { var currentPosition = e.GetPosition(DesignerCanvas); var offsetX = currentPosition.X - _startPoint.X; var offsetY = currentPosition.Y - _startPoint.Y; var newX = _selectedTeil.PositionX + offsetX; var newY = _selectedTeil.PositionY + offsetY; ((MainWindowViewModel)DataContext).UpdatePosition(_selectedTeil, newX, newY); _startPoint = currentPosition; } }
        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isDragging)
            {
                _isDragging = false; _selectedTeil = null; Mouse.Capture(null);
            }
        }
        private void Rectangle_Loaded(object sender, RoutedEventArgs e) { if (sender is Rectangle rectangle && rectangle.DataContext is Teil teil) { Canvas.SetLeft(rectangle, teil.PositionX); Canvas.SetTop(rectangle, teil.PositionY); } }
    }
}