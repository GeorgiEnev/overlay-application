using Avalonia.Controls;
using Avalonia.Input;

namespace InkX.App
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DrawCanvas.PointerPressed += OnPointerPressed;
            DrawCanvas.PointerMoved += OnPointerMoved;
            DrawCanvas.PointerReleased += OnPointerReleased;
        }

        private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            // This will run when mouse/pen goes down
        }

        private void OnPointerMoved(object? sender, PointerEventArgs e)
        {
            // This will run when mouse/pen moves
        }

        private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            // This will run when mouse/pen goes up
        }

    }
}