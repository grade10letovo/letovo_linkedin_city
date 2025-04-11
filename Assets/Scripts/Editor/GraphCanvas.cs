using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.GraphView;

public class GraphCanvas : GraphView
{
    private const float gridSpacing = 20f;

    public GraphCanvas()
    {
        this.StretchToParentSize();
        this.style.backgroundColor = new StyleColor(Color.gray);

        this.SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
        this.AddManipulator(new EdgeManipulator());

        Insert(0, new GridBackground());

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

        if (graphViewChange.movedElements != null)
        {
            foreach (var element in graphViewChange.movedElements)
            {
                if (element is GraphNode node)
                {
                    SnapNodeToGrid(node);
                }
            }
        }

        return graphViewChange;
    }

    private void SnapNodeToGrid(GraphNode node)
    {
        float x = Mathf.Round(node.GetPosition().x / gridSpacing) * gridSpacing;
        float y = Mathf.Round(node.GetPosition().y / gridSpacing) * gridSpacing;
        node.SetPosition(new Rect(new Vector2(x, y), node.GetPosition().size));
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        List<Port> connectionList = new List<Port>();
        ports.ForEach(port =>
        {
            if (port == startPort || port.node == startPort.node || port.direction == startPort.direction)
                return;
            connectionList.Add(port);
        });
        return connectionList;
    }
}
