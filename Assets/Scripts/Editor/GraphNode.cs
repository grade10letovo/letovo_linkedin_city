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
    private List<string> dropdownOptions = new List<string> { "Brown University", "Õ»” ¬ÿ›", "Yale University", "–¿Õ’Ë√—", "Not selected" };


    public GraphNode(string name, Vector2 position, string nodeID = null)
    {
        nodeName = name;
        if (nodeID == null)
            this.nodeID = GenerateUniqueId();
        universities = new List<string>();

        style.position = Position.Absolute;
        style.left = position.x;
        style.top = position.y;
        style.backgroundColor = new StyleColor(Color.white);
        style.width = 150;

        var titleTextField = new TextField
        {
            value = nodeName,
            style =
            {
                unityTextAlign = TextAnchor.MiddleCenter,
                unityFontStyleAndWeight = FontStyle.Bold,
                marginBottom = 10,
                marginTop = 10
            }
        };

        titleTextField.RegisterValueChangedCallback(evt =>
        {
            nodeName = evt.newValue;
            title = nodeName;
        });

        titleContainer.Clear();
        titleContainer.Add(titleTextField);

        var inputPort = CreatePort("Input", Direction.Input);
        inputContainer.Add(inputPort);

        var outputPort = CreatePort("Output", Direction.Output);
        outputContainer.Add(outputPort);

        var listContainer = new VisualElement();
        listContainer.style.marginTop = 10;

        var listLabel = new Label("Universities:");
        listLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        listContainer.Add(listLabel);

        foreach (var item in universities)
        {
            listContainer.Add(CreateDropdownItem(item));
        }

        var addItemButton = new Button(() =>
        {
            AddDropdownItem("Not selected", listContainer);
        })
        {
            text = "Add University"
        };

        addItemButton.style.marginTop = 5;
        addItemButton.style.marginBottom = 5;
        listContainer.Add(addItemButton);

        mainContainer.Add(listContainer);



        this.AddManipulator(new Dragger());

        RefreshExpandedState();
        RefreshPorts();
        RefreshSize();
    }

    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
        style.left = newPos.xMin;
        style.top = newPos.yMin;
        UpdatePosition();
    }

    private void UpdatePosition() { }

    private Port CreatePort(string portName, Direction direction)
    {
        var port = InstantiatePort(Orientation.Horizontal, direction, Port.Capacity.Multi, typeof(float));
        port.portName = portName;
        return port;
    }

    private void RefreshSize()
    {
        RefreshExpandedState();
        RefreshPorts();

        float totalHeight = 40;
        foreach (var child in mainContainer.Children())
        {
            totalHeight += child.resolvedStyle.height + child.resolvedStyle.marginTop + child.resolvedStyle.marginBottom;
        }

        style.height = totalHeight;
    }

    private float CalculateContainerHeight()
    {
        float height = resolvedStyle.paddingTop + resolvedStyle.paddingBottom;

        foreach (var child in mainContainer.Children())
        {
            height += child.resolvedStyle.height + child.resolvedStyle.marginTop + child.resolvedStyle.marginBottom;
        }

        return height;
    }

    private static string GenerateUniqueId()
    {
        return Guid.NewGuid().ToString();
    }

    private VisualElement CreateDropdownItem(string selectedItem)
    {
        var dropdown = new DropdownField(dropdownOptions, selectedItem);
        dropdown.RegisterValueChangedCallback(evt =>
        {
            int index = universities.IndexOf(selectedItem);
            if (index != -1)
            {
                universities[index] = evt.newValue;
            }
        });

        var removeButton = new Button(() =>
        {
            universities.Remove(selectedItem);
            dropdown.parent.RemoveFromHierarchy();
            RefreshSize();
        })
        {
            text = "X",
            style = { marginLeft = 50 }
        };

        var itemContainer = new VisualElement();
        itemContainer.style.flexDirection = FlexDirection.Row;
        itemContainer.style.marginBottom = 5;
        itemContainer.Add(dropdown);
        itemContainer.Add(removeButton);

        return itemContainer;
    }

    private void AddDropdownItem(string defaultItem, VisualElement listContainer)
    {
        universities.Add(defaultItem);
        listContainer.Insert(listContainer.childCount - 1, CreateDropdownItem(defaultItem));
        RefreshSize();
    }
}
