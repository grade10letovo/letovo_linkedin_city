using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEngine;
using System;

public class GraphNode : Node
{
    public string nodeName;
    public string nodeID;

    public GraphNode(string name, Vector2 position, string nodeID = null)
    {
        nodeName = name;
        if (nodeID == null)
            this.nodeID = GenerateUniqueId();
        style.position = Position.Absolute;
        style.left = position.x;
        style.top = position.y;
        style.backgroundColor = new StyleColor(Color.white);
        style.width = 150;
        style.height = 70;

        // Устанавливаем имя узла
        title = nodeName;

        // Создаём входной порт
        var inputPort = CreatePort("Input", Direction.Input);
        inputContainer.Add(inputPort);
        Debug.Log("Created input port");
        // Создаём выходной порт
        var outputPort = CreatePort("Output", Direction.Output);
        outputContainer.Add(outputPort);
        Debug.Log("Created output port");

        // Добавить возможность перемещения узла
        this.AddManipulator(new Dragger());

        RefreshExpandedState();
        RefreshPorts();
        Debug.Log($"Created node with ID {nodeID}");
    }

    private Port CreatePort(string portName, Direction direction)
    {
        var port = InstantiatePort(Orientation.Horizontal, direction, Port.Capacity.Multi, typeof(float));
        port.portName = portName;

        return port;
    }
    private static string GenerateUniqueId()
    {
        return Guid.NewGuid().ToString();
    }
}
