using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;

namespace InkX.App
{
    public partial class MainWindow : Window
    {
        private Ellipse? pointerIndicator;

        public MainWindow()
        {
            InitializeComponent();

            DrawCanvas.PointerPressed += OnPointerPressed;
            DrawCanvas.PointerMoved += OnPointerMoved;
            DrawCanvas.PointerReleased += OnPointerReleased;
        }

        private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            var position = e.GetPosition(DrawCanvas);

            pointerIndicator = new Ellipse
            {
                Width = 8,
                Height = 8,
                Fill = Brushes.Red
            };

            Canvas.SetLeft(pointerIndicator, position.X - 4);
            Canvas.SetTop(pointerIndicator, position.Y - 4);

            DrawCanvas.Children.Add(pointerIndicator);
        }

        private void OnPointerMoved(object? sender, PointerEventArgs e)
        {
            if(pointerIndicator == null)
                return;

            var position = e.GetPosition(DrawCanvas);
            Canvas.SetLeft(pointerIndicator, position.X - 4);
            Canvas.SetTop(pointerIndicator, position.Y - 4);
        }

        private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
        {
           if(pointerIndicator != null)
            {
                DrawCanvas.Children.Remove(pointerIndicator);
                pointerIndicator = null;
            }
        }
    }
}