using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEngine;
using System;
using System.Collections.Generic;

public class GraphNode : Node
{
    public string nodeName;
    public string nodeID;
    public List<string> universities;

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
        // Список строк
        var listContainer = new VisualElement();
        listContainer.style.marginTop = 10;

        var listLabel = new Label("Items:");
        listLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        listContainer.Add(listLabel);

        foreach (var item in universities)
        {
            listContainer.Add(CreateListItem(item));
        }

        var addItemButton = new Button(() =>
        {
            AddItemToList("New University", listContainer);
        })
        {
            text = "Add University"
        };
        listContainer.Add(addItemButton);

        mainContainer.Add(listContainer);

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

    private VisualElement CreateListItem(string text)
{
    var listItem = new TextField { value = text };
    listItem.RegisterValueChangedCallback(evt =>
    {
        int index = universities.IndexOf(text);
        if (index != -1)
        {
            universities[index] = evt.newValue;
        }
    });

    var removeButton = new Button(() =>
    {
        universities.Remove(text);
        listItem.parent.RemoveFromHierarchy();
    })
    {
        text = "X"
    };

    var itemContainer = new VisualElement();
    itemContainer.style.flexDirection = FlexDirection.Row;
    itemContainer.Add(listItem);
    itemContainer.Add(removeButton);

    return itemContainer;
}

private void AddItemToList(string newItem, VisualElement listContainer)
{
    universities.Add(newItem);
    listContainer.Insert(listContainer.childCount - 1, CreateListItem(newItem)); // Добавляем перед кнопкой
}
}
