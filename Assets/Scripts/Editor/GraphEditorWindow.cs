using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class GraphEditorWindow : EditorWindow
{
    private GraphCanvas canvas;

    [MenuItem("Window/Graph Editor")]
    public static void Open()
    {
        var window = GetWindow<GraphEditorWindow>();
        window.titleContent = new GUIContent("Graph Editor");
        window.minSize = new Vector2(600, 400);
    }

    private void CreateGUI()
    {
        // ������ �������� ������� ����������
        VisualElement root = rootVisualElement;

        // ��������� ������
        canvas = new GraphCanvas();
        root.Add(canvas);

        // ��������� ������ ������������
        var toolbar = new Toolbar();
        root.Add(toolbar);

        // ������ ��� �������� �����
        var createNodeButton = new Button(() =>
        {
            // �������� ����
            var nodeName = "Node " + (canvas.childCount + 1);
            var node = new GraphNode(nodeName, new Vector2(200, 200));
            canvas.AddElement(node);
        })
        {
            text = "Add Node"
        };

        toolbar.Add(createNodeButton);
    }
}
