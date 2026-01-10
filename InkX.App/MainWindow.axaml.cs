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
        private IBrush currentBrush = Brushes.Red;

        private const double minBrushSize = 2;
        private const double maxBrushSize = 50;
        private const double brushStep = 2;

        public MainWindow()
        {
            InitializeComponent();
            this.Focus();

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
            if (lastPoint == null)
                return;

            var currentPoint = e.GetPosition(DrawCanvas);

            var line = new Line
            {
                StartPoint = lastPoint.Value,
                EndPoint = currentPoint,
                Stroke = currentBrush,
                StrokeThickness = brushSize
            };

            DrawCanvas.Children.Add(line);

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
        }

        private void UpdateBrushSizeUI()
        {
            BrushSizeInput.Text = brushSize.ToString();
        }

        private void IncreaseBrushSize(object? sender, RoutedEventArgs e)
        {
            brushSize += brushStep;
            if (brushSize > maxBrushSize)
                brushSize = maxBrushSize;

            UpdateBrushSizeUI();
        }

        private void DecreaseBrushSize(object? sender, RoutedEventArgs e)
        {
            brushSize -= brushStep;
            if (brushSize < minBrushSize)
                brushSize = minBrushSize;

            UpdateBrushSizeUI();
        }

        private void OnBrushSizeInputChanged(object? sender, RoutedEventArgs e)
        {
            ApplyBrushSizeFromInput();
        }

        private void OnBrushSizeInputKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                ApplyBrushSizeFromInput();
        }

        private void ApplyBrushSizeFromInput()
        {
            if (double.TryParse(BrushSizeInput.Text, out double value))
            {
                value = Math.Round(value / brushStep) * brushStep;

                if (value < minBrushSize) value = minBrushSize;
                if (value > maxBrushSize) value = maxBrushSize;

                brushSize = value;
                UpdateBrushSizeUI();
            }
            else
            {
                UpdateBrushSizeUI(); 
            }
        }

        private void SetRed(object? sender, RoutedEventArgs e)
        {
            currentBrush = Brushes.Red;
        }

        private void SetBlue(object? sender, RoutedEventArgs e)
        {
            currentBrush = Brushes.Blue;
        }
    }
}