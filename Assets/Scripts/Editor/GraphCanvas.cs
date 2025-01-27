using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.GraphView;

public class GraphCanvas : GraphView
{
    public GraphCanvas()
    {
        // Устанавливаем размеры и стиль канваса
        this.StretchToParentSize();
        this.style.backgroundColor = new StyleColor(Color.gray);

        // Добавляем поддержку перетаскивания и зума
        this.SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
        // Добавляем стили для рёбер
        this.AddManipulator(new EdgeManipulator());
        // Настраиваем события для соединения и отсоединения рёбер
        graphViewChanged = OnGraphViewChanged;
    }
    private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
    {
        if (graphViewChange.edgesToCreate != null)
        {
            foreach (var edge in graphViewChange.edgesToCreate)
            {
                Debug.Log($"Created edge between {edge.input.node.title} and {edge.output.node.title}");
            }
        }

        return graphViewChange;
    }
    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        List<Port> connectionList = new List<Port>();
        ports.ForEach(port =>
        {
            if (port == startPort)
            {
                return;
            }
            if (port.node == startPort.node)
            {
                return;
            }
            if (port.direction == startPort.direction)
            {
                return;
            }
            connectionList.Add(port);
        });
        return connectionList;
    }

}
