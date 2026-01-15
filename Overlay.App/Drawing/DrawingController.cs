using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;
using System.Collections.Generic;
using System.Linq;

namespace Overlay.App.Drawing;

public sealed class DrawingController
{
    private readonly Canvas canvas;
    private readonly BrushState brush;

    private Point? lastPoint;
    private List<Line>? activeStroke;

    private readonly List<ICommand> history = new();
    private int historyIndex = -1;

    public DrawingController(Canvas canvas, BrushState brush)
    {
        this.canvas = canvas;
        this.brush = brush;
    }

    public void OnPressed(PointerPressedEventArgs e)
    {
        lastPoint = e.GetPosition(canvas);
        activeStroke = new List<Line>();
    }

    public void OnMoved(PointerEventArgs e)
    {
        if (lastPoint is null || activeStroke is null)
            return;

        var current = e.GetPosition(canvas);

        var line = new Line
        {
            StartPoint = lastPoint.Value,
            EndPoint = current,
            Stroke = brush.Brush,
            StrokeThickness = brush.Size,
            StrokeLineCap = PenLineCap.Round,
            StrokeJoin = PenLineJoin.Round
        };

        canvas.Children.Add(line);
        activeStroke.Add(line);

        lastPoint = current;
    }

    public void OnReleased()
    {
        if (activeStroke is { Count: > 0 })
            Commit(new StrokeCommand(activeStroke));

        activeStroke = null;
        lastPoint = null;
    }

    public void Clear()
    {
        var lines = canvas.Children.OfType<Line>().ToList();
        if (lines.Count == 0)
            return;

        Commit(new ClearCommand(lines));
        canvas.Children.Clear();
    }

    public void Undo()
    {
        if (historyIndex < 0)
            return;

        history[historyIndex].Undo(canvas);
        historyIndex--;
    }

    public void Redo()
    {
        if (historyIndex + 1 >= history.Count)
            return;

        historyIndex++;
        history[historyIndex].Redo(canvas);
    }

    private void Commit(ICommand command)
    {
        if (historyIndex < history.Count - 1)
            history.RemoveRange(historyIndex + 1, history.Count - historyIndex - 1);

        history.Add(command);
        historyIndex++;

        command.Redo(canvas);
    }

    private interface ICommand
    {
        void Undo(Canvas canvas);
        void Redo(Canvas canvas);
    }

    private sealed class StrokeCommand : ICommand
    {
        private readonly List<Line> lines;

        public StrokeCommand(List<Line> lines)
        {
            this.lines = lines;
        }

        public void Undo(Canvas canvas)
        {
            foreach (var line in lines)
                canvas.Children.Remove(line);
        }

        public void Redo(Canvas canvas)
        {
            foreach (var line in lines)
                if (!canvas.Children.Contains(line))
                    canvas.Children.Add(line);
        }
    }

    private sealed class ClearCommand : ICommand
    {
        private readonly List<Line> removedLines;

        public ClearCommand(List<Line> removedLines)
        {
            this.removedLines = removedLines;
        }

        public void Undo(Canvas canvas)
        {
            foreach (var line in removedLines)
                if (!canvas.Children.Contains(line))
                    canvas.Children.Add(line);
        }

        public void Redo(Canvas canvas)
        {
            canvas.Children.Clear();
        }
    }
}
