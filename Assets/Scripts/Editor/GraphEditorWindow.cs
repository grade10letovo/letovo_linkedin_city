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
            var nodeName = "Node " + (canvas.childCount + 1);
            var node = new GraphNode(nodeName, new Vector2(200, 200));
            canvas.AddElement(node);
        })
        {
            text = "Add Node"
        };
        toolbar.Add(createNodeButton);

        // Кнопка сохранения в БД
        var saveButton = new Button(() => SaveUtility.SaveGraph(canvas))
        {
            text = "Save Graph"
        };
        toolbar.Add(saveButton);

        // Кнопка загрузки из БД
        var loadButton = new Button(() => SaveUtility.LoadGraphFromDatabase(canvas))
        {
            text = "Load Graph"
        };
        toolbar.Add(loadButton);

        rootVisualElement.Add(toolbar);
    }
}
