using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class GraphEdge : ImmediateModeElement
{
    private Vector2 start;
    private Vector2 end;

    public GraphEdge(Vector2 startPoint, Vector2 endPoint)
    {
        start = startPoint;
        end = endPoint;
    }

    protected override void ImmediateRepaint()
    {
        // Убедитесь, что графика корректно рисуется
        Handles.BeginGUI();
        Handles.color = Color.black;
        Handles.DrawLine(start, end);
        Handles.EndGUI();
    }

    public void UpdatePosition(Vector2 newStart, Vector2 newEnd)
    {
        start = newStart;
        end = newEnd;
        MarkDirtyRepaint(); // Обновить отрисовку
    }
}
