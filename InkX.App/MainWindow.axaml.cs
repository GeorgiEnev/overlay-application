using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;

namespace InkX.App
{
    public partial class MainWindow : Window
    {
        private Point? lastPoint;

        public MainWindow()
        {
            InitializeComponent();

            DrawCanvas.PointerPressed += OnPointerPressed;
            DrawCanvas.PointerMoved += OnPointerMoved;
            DrawCanvas.PointerReleased += OnPointerReleased;
        }

        private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
             lastPoint = e.GetPosition(DrawCanvas);
        }

        private void OnPointerMoved(object? sender, PointerEventArgs e)
        {
            if(lastPoint == null)
                return;

            var currentPoint = e.GetPosition(DrawCanvas);

            var line = new Line
            {
                StartPoint = lastPoint.Value,
                EndPoint = currentPoint,
                Stroke = Brushes.Red,
                StrokeThickness = 2
            };

            DrawCanvas.Children.Add(line);

            lastPoint = currentPoint;
        }

        private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            lastPoint = null;
        }
    }
}