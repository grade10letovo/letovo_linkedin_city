using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class SaveUtility
{
    [System.Serializable]
    public class NodeData
    {
        public string nodeName;
        public string nodeID;
        public float x_position;
        public float y_position;
    }

    [System.Serializable]
    public class EdgeData
    {
        public string outputNodeID;
        public string inputNodeID;
    }

    [System.Serializable]
    public class GraphData
    {
        public List<NodeData> nodes = new List<NodeData>();
        public List<EdgeData> edges = new List<EdgeData>();
    }

    public static void SaveGraph(GraphView graphView, string filePath)
    {
        Debug.Log("Starting graph save process...");

        var graphData = new GraphData();
        Debug.Log(graphView.nodes.ToList().Count);

        foreach (var nodeElement in graphView.nodes.ToList())
        {
            if (nodeElement is GraphNode graphNode)
            {
                Debug.Log($"Saving node: {graphNode.nodeName}, ID: {graphNode.nodeID}");

                graphData.nodes.Add(new NodeData
                {
                    nodeName = graphNode.nodeName,
                    nodeID = graphNode.nodeID,
                    x_position = graphNode.style.left.value.value,
                    y_position = graphNode.style.top.value.value
                });
            }
        }

        foreach (var edgeElement in graphView.edges.ToList())
        {
            if (edgeElement is Edge edge)
            {
                var outputNodeID = ((GraphNode)edge.output.node).nodeID;
                var inputNodeID = ((GraphNode)edge.input.node).nodeID;

                Debug.Log($"Saving edge: {outputNodeID} -> {inputNodeID}");

                graphData.edges.Add(new EdgeData
                {
                    outputNodeID = outputNodeID,
                    inputNodeID = inputNodeID
                });
            }
        }

        var json = JsonConvert.SerializeObject(graphData, Formatting.Indented);
        File.WriteAllText(filePath, json);

        Debug.Log("Graph saved successfully to " + filePath);
    }

    public static void LoadGraph(GraphView graphView, string filePath)
    {
        Debug.Log("Starting graph load process...");

        if (!File.Exists(filePath))
        {
            Debug.LogError("File not found: " + filePath);
            return;
        }

        Debug.Log("Reading file: " + filePath);

        var json = File.ReadAllText(filePath);
        var graphData = JsonConvert.DeserializeObject<GraphData>(json);

        Debug.Log("Clearing existing graph elements...");
        graphView.DeleteElements(graphView.graphElements.ToList());

        var nodes = new Dictionary<string, GraphNode>();

        foreach (var nodeData in graphData.nodes)
        {
            Debug.Log($"Loading node: {nodeData.nodeName}, ID: {nodeData.nodeID}");

            var node = new GraphNode(nodeData.nodeName, new Vector2(nodeData.x_position, nodeData.y_position), nodeData.nodeID);
            graphView.AddElement(node);
            nodes[nodeData.nodeID] = node;
        }

        foreach (var edgeData in graphData.edges)
        {
            Debug.Log($"Loading edge: {edgeData.outputNodeID} -> {edgeData.inputNodeID}");

            if (nodes.TryGetValue(edgeData.outputNodeID, out var outputNode) &&
                nodes.TryGetValue(edgeData.inputNodeID, out var inputNode))
            {
                var outputPort = outputNode.outputContainer[0] as Port;
                var inputPort = inputNode.inputContainer[0] as Port;

                if (outputPort != null && inputPort != null)
                {
                    var edge = new Edge
                    {
                        output = outputPort,
                        input = inputPort
                    };

                    edge.output.Connect(edge);
                    edge.input.Connect(edge);

                    graphView.AddElement(edge);

                    Debug.Log("Edge successfully added to the graph.");
                }
                else
                {
                    Debug.LogWarning("Could not find valid ports for edge.");
                }
            }
            else
            {
                Debug.LogWarning("Could not find nodes for edge.");
            }
        }

        Debug.Log("Graph loaded successfully from " + filePath);
    }
}