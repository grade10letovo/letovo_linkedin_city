using UnityEditor;
using UnityEditor.Experimental.GraphView;
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
        window.minSize = new Vector2(300, 300);
    }

    private void CreateGUI()
    {
        // Добавляем канвас
        canvas = new GraphCanvas();
        rootVisualElement.Add(canvas);

        CreateToolbar();
    }
    private void CreateToolbar()
    {
        var toolbar = new Toolbar();
        // Кнопка для создания узлов
        var createNodeButton = new Button(() =>
        {
            // Создание узла
            var nodeName = "Node " + (canvas.childCount + 1);
            var node = new GraphNode(nodeName, new Vector2(200, 200));
            canvas.AddElement(node);
            Debug.Log("Create node");
            Debug.Log(canvas.nodes.ToList().Count);
        })
        {
            text = "Add Node"
        };
        toolbar.Add(createNodeButton);

        var saveButton = new Button(() => SaveGraph()) { text = "Save Graph" };
        toolbar.Add(saveButton);

        var loadButton = new Button(() => LoadGraph()) { text = "Load Graph" };
        toolbar.Add(loadButton);

        rootVisualElement.Add(toolbar);
    }
    private void SaveGraph()
    {
        string path = EditorUtility.SaveFilePanel("Save Graph", "", "Graph", "json");
        if (!string.IsNullOrEmpty(path))
        {
            SaveUtility.SaveGraph(canvas, path);
        }
        else
        {
            Debug.LogWarning("Save operation was canceled.");
        }
    }

    private void LoadGraph()
    {
        string path = EditorUtility.OpenFilePanel("Load Graph", "", "json");
        if (!string.IsNullOrEmpty(path))
        {
            SaveUtility.LoadGraph(canvas, path);
        }
        else
        {
            Debug.LogWarning("Load operation was canceled.");
        }
    }
}
