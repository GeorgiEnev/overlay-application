using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;

namespace InkX.App
{
    public partial class MainWindow : Window
    {
        private Point? lastPoint;
        private double brushSize = 2;
        private IBrush currentBrush = Brushes.Black;

        private bool isToolBarVisible = true;
        private bool isDraggingToolBar;
        private Point dragStartPointer;
        private Vector dragStartOffset;

        private readonly TranslateTransform toolBarTransform = new TranslateTransform();

        private const double MinBrushSize = 2;
        private const double MaxBrushSize = 50;
        private const double BrushStep = 2;

        private bool isColorPopupOpen;
        private const int colorPickerSize = 180;
        private double pickerHue = 0.0;

        public MainWindow()
        {
            InitializeComponent();
            GenerateColorPickerBitmap();
            Focus();

            ToolBar.RenderTransform = toolBarTransform;

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
            if (lastPoint is null)
                return;

            var currentPoint = e.GetPosition(DrawCanvas);

            DrawCanvas.Children.Add(new Line
            {
                StartPoint = lastPoint.Value,
                EndPoint = currentPoint,
                Stroke = currentBrush,
                StrokeThickness = brushSize,
                StrokeLineCap = PenLineCap.Round,
                StrokeJoin = PenLineJoin.Round
            });

            lastPoint = currentPoint;
        }

        private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            lastPoint = null;
        }

        private void OnKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.C)
            {
                DrawCanvas.Children.Clear();
                lastPoint = null;
            }
            else if (e.Key == Key.T)
            {
                isToolBarVisible = !isToolBarVisible;
                ToolBar.IsVisible = isToolBarVisible;
            }
        }

        private void IncreaseBrushSize(object? sender, RoutedEventArgs e)
        {
            SetBrushSize(brushSize + BrushStep);
        }

        private void DecreaseBrushSize(object? sender, RoutedEventArgs e)
        {
            SetBrushSize(brushSize - BrushStep);
        }

        private void OnBrushSizeInputKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                ApplyBrushSizeFromInput();
        }

        private void OnBrushSizeInputChanged(object? sender, RoutedEventArgs e)
        {
            ApplyBrushSizeFromInput();
        }

        private void ApplyBrushSizeFromInput()
        {
            if (!double.TryParse(BrushSizeInput.Text, out var value))
            {
                UpdateBrushSizeUI();
                return;
            }

            value = Math.Round(value / BrushStep) * BrushStep;
            SetBrushSize(value);
        }

        private void SetBrushSize(double value)
        {
            brushSize = Math.Clamp(value, MinBrushSize, MaxBrushSize);
            UpdateBrushSizeUI();
        }

        private void UpdateBrushSizeUI()
        {
            BrushSizeInput.Text = brushSize.ToString("0");
        }

        private void OnToolBarPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            isDraggingToolBar = true;

            dragStartPointer = e.GetPosition(this);
            dragStartOffset = new Vector(toolBarTransform.X, toolBarTransform.Y);

            e.Pointer.Capture(ToolBar);
        }

        private void OnToolBarPointerMoved(object? sender, PointerEventArgs e)
        {
            if (!isDraggingToolBar)
                return;

            var currentPointer = e.GetPosition(this);
            var delta = currentPointer - dragStartPointer;

            toolBarTransform.X = dragStartOffset.X + delta.X;
            toolBarTransform.Y = dragStartOffset.Y + delta.Y;
        }

        private void OnToolBarPointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            isDraggingToolBar = false;
            e.Pointer.Capture(null);
        }

        private void ToggleColorPopup(object? sender, RoutedEventArgs e)
        {
            isColorPopupOpen = !isColorPopupOpen;
            ColorPopup.IsOpen = isColorPopupOpen;
        }

        private void OnColorPickerPressed(object? sender, PointerPressedEventArgs e)
        {
        }

        private void GenerateColorPickerBitmap()
        {
            var bitmap = new WriteableBitmap(
                new PixelSize(colorPickerSize, colorPickerSize),
                new Vector(96, 96),
                PixelFormat.Bgra8888,
                AlphaFormat.Unpremul);

            using var fb = bitmap.Lock();

            unsafe
            {
                uint* buffer = (uint*)fb.Address;

                for (int y = 0; y < colorPickerSize; y++)
                {
                    for (int x = 0; x < colorPickerSize; x++)
                    {
                        double s = (double)x / (colorPickerSize - 1);
                        double v = 1.0 - (double)y / (colorPickerSize - 1);

                        var color = HsvToColor(pickerHue, s, v);

                        buffer[y * colorPickerSize + x] =
                            ((uint)color.A << 24) |
                            ((uint)color.B << 16) |
                            ((uint)color.G << 8) |
                            color.R;
                    }
                }
            }

            ColorPickerImage.Source = bitmap;
        }

        private static Color HsvToColor(double h, double s, double v)
        {
            double c = v * s;
            double x = c * (1 - Math.Abs((h / 60) % 2 - 1));
            double m = v - c;

            double r = 0, g = 0, b = 0;

            if (h < 60) { r = c; g = x; }
            else if (h < 120) { r = x; g = c; }
            else if (h < 180) { g = c; b = x; }
            else if (h < 240) { g = x; b = c; }
            else if (h < 300) { r = x; b = c; }
            else { r = c; b = x; }

            return Color.FromArgb(
                255,
                (byte)((r + m) * 255),
                (byte)((g + m) * 255),
                (byte)((b + m) * 255));
        }
    }
}
