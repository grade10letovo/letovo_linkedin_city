using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System.Collections.Generic;
using Npgsql;

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

    public static void SaveGraph(GraphView graphView, string unusedPath = null)
    {
        Debug.Log("🔄 Starting graph save process to database...");

        var graphData = new GraphData();

        try
        {
            foreach (var nodeElement in graphView.nodes.ToList())
            {
                if (nodeElement is GraphNode graphNode)
                {
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

                    graphData.edges.Add(new EdgeData
                    {
                        outputNodeID = outputNodeID,
                        inputNodeID = inputNodeID
                    });
                }
            }

            SaveGraphToDatabase(graphData);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("❌ Error while preparing graph data: " + ex.Message);
        }
    }

    private static void EnsureTablesExist(NpgsqlConnection conn)
    {
        string createVertices = @"CREATE TABLE IF NOT EXISTS vertices (
            id SERIAL PRIMARY KEY,
            x_coord DOUBLE PRECISION NOT NULL,
            y_coord DOUBLE PRECISION NOT NULL);";

        string createEdges = @"CREATE TABLE IF NOT EXISTS edges (
            id SERIAL PRIMARY KEY,
            start_vertex_id INTEGER NOT NULL REFERENCES vertices(id),
            end_vertex_id INTEGER NOT NULL REFERENCES vertices(id));";

        using (var cmd = new NpgsqlCommand(createVertices, conn))
        {
            cmd.ExecuteNonQuery();
        }

        using (var cmd = new NpgsqlCommand(createEdges, conn))
        {
            cmd.ExecuteNonQuery();
        }

        Debug.Log("✅ Verified that required tables exist.");
    }

    private static void SaveGraphToDatabase(GraphData graphData)
    {
        const string connString = "Host=localhost;Username=postgres;Password=code1234;Database=city";

        try
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                Debug.Log("✅ Database connection opened.");

                EnsureTablesExist(conn);

                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;

                    // Clear old data
                    cmd.CommandText = "DELETE FROM edges";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "DELETE FROM vertices";
                    cmd.ExecuteNonQuery();
                    Debug.Log("🧹 Cleared old data from tables.");

                    // Insert nodes
                    for (int i = 0; i < graphData.nodes.Count; i++)
                    {
                        var node = graphData.nodes[i];
                        cmd.CommandText = "INSERT INTO vertices (id, x_coord, y_coord) VALUES (@id, @x, @y)";
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("id", i + 1);
                        cmd.Parameters.AddWithValue("x", node.x_position);
                        cmd.Parameters.AddWithValue("y", node.y_position);
                        cmd.ExecuteNonQuery();
                        Debug.Log($"📌 Inserted vertex {i + 1}: ({node.x_position}, {node.y_position})");
                    }

                    // Insert edges
                    foreach (var edge in graphData.edges)
                    {
                        int startId = graphData.nodes.FindIndex(n => n.nodeID == edge.outputNodeID) + 1;
                        int endId = graphData.nodes.FindIndex(n => n.nodeID == edge.inputNodeID) + 1;

                        if (startId > 0 && endId > 0)
                        {
                            cmd.CommandText = "INSERT INTO edges (start_vertex_id, end_vertex_id) VALUES (@start, @end)";
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("start", startId);
                            cmd.Parameters.AddWithValue("end", endId);
                            cmd.ExecuteNonQuery();
                            Debug.Log($"🔗 Inserted edge: {startId} -> {endId}");
                        }
                        else
                        {
                            Debug.LogWarning("⚠️ Could not resolve node IDs for edge.");
                        }
                    }
                }
                conn.Close();
                Debug.Log("✅ Graph successfully saved to database.");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("❌ Database error while saving graph: " + ex.Message);
        }
    }

    public static void LoadGraphFromDatabase(GraphView graphView)
    {
        const string connString = "Host=localhost;Username=postgres;Password=code1234;Database=city";

        var nodes = new List<NodeData>();
        var edges = new List<EdgeData>();

        try
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                Debug.Log("✅ Database connection opened for loading.");

                EnsureTablesExist(conn);

                using (var cmd = new NpgsqlCommand("SELECT id, x_coord, y_coord FROM vertices ORDER BY id", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        float x = (float)reader.GetDouble(1);
                        float y = (float)reader.GetDouble(2);
                        nodes.Add(new NodeData
                        {
                            nodeName = "Node " + id,
                            nodeID = id.ToString(),
                            x_position = x,
                            y_position = y
                        });
                    }
                }

                using (var cmd = new NpgsqlCommand("SELECT start_vertex_id, end_vertex_id FROM edges", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        edges.Add(new EdgeData
                        {
                            outputNodeID = reader.GetInt32(0).ToString(),
                            inputNodeID = reader.GetInt32(1).ToString()
                        });
                    }
                }

                conn.Close();
                Debug.Log($"📥 Loaded {nodes.Count} nodes and {edges.Count} edges from database.");
            }

            graphView.DeleteElements(graphView.graphElements.ToList());
            var nodeMap = new Dictionary<string, GraphNode>();

            foreach (var nodeData in nodes)
            {
                var node = new GraphNode(nodeData.nodeName, new Vector2(nodeData.x_position, nodeData.y_position), nodeData.nodeID);
                graphView.AddElement(node);
                nodeMap[nodeData.nodeID] = node;
            }

            foreach (var edgeData in edges)
            {
                if (nodeMap.TryGetValue(edgeData.outputNodeID, out var outputNode) &&
                    nodeMap.TryGetValue(edgeData.inputNodeID, out var inputNode))
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
                    }
                    else
                    {
                        Debug.LogWarning("⚠️ Missing ports on one of the nodes.");
                    }
                }
                else
                {
                    Debug.LogWarning("⚠️ Edge node references not found.");
                }
            }

            Debug.Log("✅ Graph loaded successfully from database.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("❌ Error loading graph from database: " + ex.Message);
        }
    }
}
