using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.GraphView;

public class GraphCanvas : GraphView
{
    public GraphCanvas()
    {
        // ������������� ������� � ����� �������
        this.StretchToParentSize();
        this.style.backgroundColor = new StyleColor(Color.gray);

        // ��������� ��������� �������������� � ����
        this.SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
        // ��������� ����� ��� ����
        this.AddManipulator(new EdgeManipulator());
        // ����������� ������� ��� ���������� � ������������ ����
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
        Debug.Log("START");
        List<Port> connectionList = new List<Port>();
        Debug.Log(ports.ToList().Count);
        Debug.Log(startPort);
        ports.ForEach(port =>
        {
            if (port == startPort)
            {
                Debug.Log("1");
                return;
            }
            if (port.node == startPort.node)
            {
                Debug.Log("2");
                return;
            }
            if (port.direction == startPort.direction)
            {
                Debug.Log("3");
                return;
            }
            Debug.Log("CHECKED");
            connectionList.Add(port);
        });
        return connectionList;
    }

}
