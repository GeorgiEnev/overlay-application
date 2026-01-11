using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
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

        public MainWindow()
        {
            InitializeComponent();
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
    }
}
