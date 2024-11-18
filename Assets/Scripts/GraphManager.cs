using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using System.Collections;
using UnityEngine;
using System.Linq;
using System;
using TMPro;

public class GraphManager : MonoBehaviour
{
    // Variables
    [Header("Graph Data")]
    public Graph graph;
    public GameObject vertexPrefab;
    public GameObject edgePrefab;

    [Header("Physics Parameters")]
    public float centerForceMultiplier = 0.5f;
    public float repelForceMultiplier = 0.5f;
    public float linkForceStiffness = 0.5f;

    [Header("Text UX")]
    public TMP_Text textUX;

    [Header("Timelapse")]
    public List<string> actions = new();
    public float timelapsePopSpeed = 0.2f;

    [Header("Tilemaps")]
    public Tilemap groundTilemap;
    public Tilemap waterTilemap;

    // Flags
    // Timelapse
    private bool isPlayingTimelapse = false;
    // Physics
    private bool physicsEnabled = true;

    // Start is called before the first frame update
    void Start()
    {
        graph = new Graph();
    }

    // FixedUpdate is called every fixed frame-rate frame (Let's say 1 time every 0.02 seconds)
    void FixedUpdate()
    {
        if (physicsEnabled)
        {
            ApplyPhysicsForces();
        }
    }

    // Get the state of Physics
    public bool IsPhysicsEnabled()
    {
        return physicsEnabled;
    }

    // Apply Physics Forces
    void ApplyPhysicsForces()
    {
        // Get the vertices and edges
        List<Vertex> vertices = graph.vertices;
        List<Edge> edges = graph.edges;

        // Initializing a dictionary to accumulate forces
        Dictionary<Vertex, Vector2> forces = new();
        foreach (var vertex in vertices)
        {
            forces[vertex] = Vector2.zero;
        }

        // Center Force
        // Assuming (0,0) is the center of the screen (which will always be in the beginning)
        Vector2 center = Vector2.zero; 

        // Calculate the center of all vertices
        foreach (var vertex in vertices)
        {
            // Skip if the vertex is being dragged (We are holding it)
            if (vertex.isDragging) continue;

            // Add the position of the vertex to the center
            Vector2 toCenter = center - (Vector2)vertex.transform.position;
            forces[vertex] += toCenter * centerForceMultiplier;
        }

        // Repel Force
        // Loop through all vertices
        for (int i = 0; i < vertices.Count; i++)
        {
            // Get the vertex
            Vertex vA = vertices[i];

            // Skip if the vertex is being dragged (We are holding it)
            if (vA.isDragging) continue;

            for (int j = i + 1; j < vertices.Count; j++)
            {
                // Get the other vertex
                Vertex vB = vertices[j];

                // Skip if the other vertex is being dragged (We are holding it)
                if (vB.isDragging) continue;

                // Calculate the direction and distance between the vertices
                Vector2 direction = (Vector2)vA.transform.position - (Vector2)vB.transform.position;
                float distance = direction.magnitude;

                 // We still can't divide by zero... right?
                if (distance < 0.1f) distance = 0.1f;

                // Calculate the repel force
                Vector2 repel = direction.normalized / (distance * distance) * repelForceMultiplier;

                // Apply the repel force to both vertices
                forces[vA] += repel;
                forces[vB] -= repel;
            }
        }

        // Link Force
        // Loop through all edges
        foreach (var edge in edges)
        {
            // Get the vertices of the edge
            Vertex vA = edge.vertexA;
            Vertex vB = edge.vertexB;

            // Skip if both vertices are being dragged (We are holding them)
            if (vA.isDragging && vB.isDragging) continue;

            // Calculate the direction and distance between the vertices
            Vector2 direction = (Vector2)vB.transform.position - (Vector2)vA.transform.position;
            float distance = direction.magnitude;
            
            // The edge want to stay this lenght
            float restLength = 1f; 

            // Calculating the link force...
            Vector2 linkForce = (distance - restLength) * linkForceStiffness * direction.normalized;

            // Apply the link force to the vertices (If they are not being dragged)
            if (!vA.isDragging) forces[vA] += linkForce;

            // Apply the link force to the vertices (If they are not being dragged)
            if (!vB.isDragging) forces[vB] -= linkForce;
        }

        // Apply forces to vertices (If they are not being dragged)
        foreach (var vertex in vertices)
        {
            // Skip if the vertex is being dragged (We are holding it)
            if (vertex.isDragging) continue;

            // Apply the force to the vertex... Finally!
            vertex.rb.AddForce(forces[vertex]);
        }
    }

    // Toggle Physics
    public void TogglePhysics()
    {
        // Toggle the state of Physics
        physicsEnabled = !physicsEnabled;

        // Loop through all vertices and enable/disable physics
        foreach (var vertex in graph.vertices)
        {
            if (physicsEnabled)
            {
                vertex.EnablePhysics();
            }
            else
            {
                vertex.DisablePhysics();
            }
        }
    }

    // Add a Vertex
    public void AddVertex(int id)
    {
        // Check if the vertex already exists
        if (graph.vertices.Exists(v => v.id == id))
        {
            textUX.text = "A Vertex with this ID already exists.";
            return;
        }

        // Get a random spawn position
        Vector3 spawnPosition = GetRandomSpawnPosition();

        // If no valid position found
        if (spawnPosition == Vector3.zero)
        {
            textUX.text = "We could not found a safe position for the Vertex. :/ Try again!";
            return;
        }

        // Instantiate (Or Spawn for the Gamers) the vertex object | Set the parent | Get the Vertex component | Set the ID
        GameObject vertexObj = Instantiate(vertexPrefab, spawnPosition, Quaternion.identity);
        vertexObj.transform.SetParent(transform);
        Vertex vertex = vertexObj.GetComponent<Vertex>();
        vertex.id = id;

        // Add the vertex to the graph
        graph.vertices.Add(vertex);

        // Add the action to the timelapse (If it's not playing)
        if (!isPlayingTimelapse)
        {
            actions.Add($"AddVertex,{id}");
        }

        // Adjust the sizes of the vertices and edges
        AdjustVertexSizes();
        AdjustEdgeWidths();
    }

    // Remove a Vertex
    public void RemoveVertex(int id)
    {
        // Find the vertex by ID
        Vertex vertex = graph.vertices.Find(v => v.id == id);
        if (vertex != null)
        {
            // Remove all connected edges
            List<Edge> connectedEdges = new(vertex.edges);
            foreach (Edge edge in connectedEdges)
            {
                RemoveEdge(edge.vertexA.id, edge.vertexB.id);
            }

            // Remove the vertex from the graph
            graph.vertices.Remove(vertex);

            // Destroy the GameObject
            Destroy(vertex.gameObject);

            // Add the action to the timelapse (If it's not playing)
            if (!isPlayingTimelapse)
            {
                actions.Add($"RemoveVertex,{id}");
            }

            // Adjust the sizes of the vertices and edges
            AdjustVertexSizes();
            AdjustEdgeWidths();
        }
        else
        {
            textUX.text = "We couldn't found the Vertex.";
        }
    }

    // Add an Edge
    public void AddEdge(int idA, int idB, float weight)
    {
        // Find the vertices by ID
        Vertex vertexA = graph.vertices.Find(v => v.id == idA);
        Vertex vertexB = graph.vertices.Find(v => v.id == idB);

        // Check if the vertices exist
        if (vertexA == null || vertexB == null)
        {
            textUX.text = "One or both Vertex do not exist. :/ Try again!";
            return;
        }

        // Check if the edge already exists
        if (graph.edges.Exists(e => (e.vertexA == vertexA && e.vertexB == vertexB) || (!graph.isDirected && e.vertexA == vertexB && e.vertexB == vertexA)))
        {
            textUX.text = "We already have a Vertex with that ID :/.";
            return;
        }

        // Instantiate the edge object | Set the parent | Get the Edge component | Set the vertices and weight
        GameObject edgeObj = Instantiate(edgePrefab);
        edgeObj.transform.SetParent(transform);
        Edge edge = edgeObj.GetComponent<Edge>();
        edge.vertexA = vertexA;
        edge.vertexB = vertexB;
        edge.weight = weight;

        // Set the directed flag and update the position
        edge.isDirected = graph.isDirected;

        // Update the color of the edge
        edge.UpdatePosition();

        graph.edges.Add(edge);

        // Add the edge to the vertices
        vertexA.edges.Add(edge);
        vertexB.edges.Add(edge);

        // Add the edge to the incoming and outgoing edges of the vertices
        if (graph.isDirected)
        {
            vertexA.outgoingEdges.Add(edge);
            vertexB.incomingEdges.Add(edge);
        }
        else
        {
            vertexA.incomingEdges.Add(edge);
            vertexB.incomingEdges.Add(edge);
        }

        // Add the action to the timelapse (If it's not playing)
        if (!isPlayingTimelapse)
        {
            actions.Add($"AddEdge,{idA},{idB},{weight}");
        }
    }

    // Remove an Edge
    public void RemoveEdge(int idA, int idB)
    {
        // Find the vertices by ID
        Vertex vertexA = graph.vertices.Find(v => v.id == idA);
        Vertex vertexB = graph.vertices.Find(v => v.id == idB);

        if (vertexA == null || vertexB == null)
        {
            textUX.text = "One or both Vertex do not exist. :/ Try again!";
            return;
        }

        // Find the edge by vertices
        Edge edge = graph.edges.Find(e => (e.vertexA == vertexA && e.vertexB == vertexB) || (!graph.isDirected && e.vertexA == vertexB && e.vertexB == vertexA));

        if (edge != null)
        {
            // Remove the edge from the graph
            graph.edges.Remove(edge);

            vertexA.edges.Remove(edge);
            vertexB.edges.Remove(edge);

            // Remove the edge from the incoming and outgoing edges of the vertices
            if (graph.isDirected)
            {
                vertexA.outgoingEdges.Remove(edge);
                vertexB.incomingEdges.Remove(edge);
            }
            else
            {
                vertexA.incomingEdges.Remove(edge);
                vertexB.incomingEdges.Remove(edge);
            }

            // Destroy the GameObject
            Destroy(edge.gameObject);

            // Add the action to the timelapse (If it's not playing)
            if (!isPlayingTimelapse)
            {
                actions.Add($"RemoveEdge,{idA},{idB}");
            }
        }
        else
        {
            textUX.text = "We could not find the Edge...";
        }
    }

    // Toggle the Directed Flag
    public void UpdateEdgesDirection()
    {
        foreach (Edge edge in graph.edges)
        {
            edge.isDirected = graph.isDirected;
            edge.UpdateColor();
        }
    }

    // Toggle the Weighted Flag
    public int GetOrder()
    {
        return graph.vertices.Count;
    }

    // Get the size of the graph
    public int GetSize()
    {
        return graph.edges.Count;
    }

    // Get the degree of a vertex
    public (int degree, int inDegree, int outDegree) GetVertexDegree(int vertexId)
    {
        // Find the vertex by ID
        Vertex vertex = graph.vertices.Find(v => v.id == vertexId);
        if (vertex != null)
        {
            // Return the degree of the vertex (inDegree and outDegree for directed graphs)
            if (graph.isDirected)
            {
                int inDegree = vertex.GetInDegree();
                int outDegree = vertex.GetOutDegree();
                int totalDegree = inDegree + outDegree;
                return (totalDegree, inDegree, outDegree);
            }
            else
            {
                int degree = vertex.GetDegree();
                return (degree, 0, 0);
            }
        }
        else
        {
            // Handle vertex not found
            return (-1, -1, -1);
        }
    }

    // Get the adjacent vertices of a vertex
    public List<Vertex> GetAdjacentVertices(int vertexId)
    {
        // Find the vertex by ID
        Vertex vertex = graph.vertices.Find(v => v.id == vertexId);
        List<Vertex> adjacentVertices = new();

        // Return the adjacent vertices of the vertex
        if (vertex != null)
        {
            foreach (Edge edge in vertex.edges)
            {
                // Add the adjacent vertex B to the list
                if (edge.vertexA == vertex) adjacentVertices.Add(edge.vertexB);

                // Add the adjacent vertex A to the list
                else adjacentVertices.Add(edge.vertexA);
            }
        }
        return adjacentVertices;
    }

    // Load a graph from a CSV file
    public void ImportGraphFromCSV(string csvContent)
    {
        // Regex to extract numbers from a line
        string[] lines = csvContent.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

        // If the file is empty
        if (lines.Length == 0)
        {
            textUX.text = "Text file is empty.";
            return;
        }

        // Process graph lines
        for (int i = 0; i < lines.Length; i++)
        {
            // Skip empty lines
            string line = lines[i];
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            // Extract numbers from the line
            List<string> numericStrings = ExtractNumbers(line);

            // Skip lines without valid numbers
            if (numericStrings.Count < 2)
            {
                textUX.text = $"Line {i + 1}: Not enough valid numbers. Expected at least two.";
                continue;
            }

            // Parse source and target IDs
            if (!int.TryParse(numericStrings[0], out int sourceId))
            {
                textUX.text = $"Line {i + 1}: Invalid Source ID '{numericStrings[0]}'.";
                continue;
            }

            // Parse source and target IDs
            if (!int.TryParse(numericStrings[1], out int targetId))
            {
                textUX.text = $"Line {i + 1}: Invalid Target ID '{numericStrings[1]}'.";
                continue;
            }

            // Parse edge weight if available
            float weight = 0f;
            if (numericStrings.Count >= 3)
            {
                // Parse the weight
                if (!float.TryParse(numericStrings[2], out weight))
                {
                    // Use default weight if parsing fails
                    textUX.text = $"Line {i + 1}: Invalid Weight '{numericStrings[2]}', using default weight.";
                    weight = 1f;
                }
            }

            // Create source and target vertices if they don't exist
            if (!graph.vertices.Exists(v => v.id == sourceId))
            {
                AddVertex(sourceId);
            }
            if (!graph.vertices.Exists(v => v.id == targetId))
            {
                AddVertex(targetId);
            }

            // Add the edge
            AddEdge(sourceId, targetId, weight);
        }
    }

    // Extracting numbers from a string using Regex
    private List<string> ExtractNumbers(string input)
    {
        List<string> numbers = new();
        string pattern = @"[-+]?\d*\.?\d+";

        // Find all matches in the input string
        foreach (Match m in Regex.Matches(input, pattern))
        {
            numbers.Add(m.Value);
        }

        return numbers;
    }

    // Shortest Path Algorithm
    public (List<Vertex>, float) GetShortestPath(int startId, int endId)
    {
        // Dijkstra's algorithm
        Dictionary<Vertex, float> distances = new();
        Dictionary<Vertex, Vertex> previous = new();
        List<Vertex> vertices = new(graph.vertices);

        // Initialize distances and previous vertices
        foreach (Vertex vertex in vertices)
        {
            distances[vertex] = float.MaxValue;
            previous[vertex] = null;
        }

        // Start vertex has distance 0
        Vertex startVertex = vertices.Find(v => v.id == startId);
        distances[startVertex] = 0;

        // Main loop to find the shortest path
        while (vertices.Count > 0)
        {
            // Sort vertices by distance
            vertices = vertices.OrderBy(v => distances[v]).ToList();
            Vertex current = vertices[0];
            vertices.Remove(current);

            // Stop if the target vertex is reached
            if (current.id == endId)
            {
                break;
            }

            // Update distances and previous vertices
            if (graph.isDirected)
            {
                // For directed graphs, we need iterate over outgoing edges and set neighbor explicitly
                foreach (Edge edge in current.outgoingEdges)
                {
                    // Neighbor is the target of the edge
                    Vertex neighbor = edge.vertexB;

                    // Calculate the new distance
                    float alt = distances[current] + edge.weight;

                    // Update the distance and previous vertex if the new distance is shorter
                    if (alt < distances[neighbor])
                    {
                        distances[neighbor] = alt;
                        previous[neighbor] = current;
                    }
                }
            }
            else
            {
                // For undirected graphs, we use both vertices of the edge | We need to check both directions (:D)
                foreach (Edge edge in current.edges)
                {
                    // Neighbor is the other vertex of the edge
                    Vertex neighbor = edge.vertexA == current ? edge.vertexB : edge.vertexA;
                    float alt = distances[current] + edge.weight;

                    // Update the distance and previous vertex if the new distance is shorter
                    if (alt < distances[neighbor])
                    {
                        distances[neighbor] = alt;
                        previous[neighbor] = current;
                    }
                }
            }
        }

        // Reconstruct the path from start to end
        List<Vertex> path = new();
        Vertex target = graph.vertices.Find(v => v.id == endId);

        // If the target vertex is unreachable
        if (previous[target] == null)
        {
            // No path found
            return (null, float.MaxValue); 
        }

        // Reconstruct the path and calculate the total cost
        float totalCost = distances[target];

        // Reconstruct the path from end to start
        while (target != null)
        {
            path.Insert(0, target);
            target = previous[target];
        }

        // Return the path and total cost. Finally!
        return (path, totalCost);
    }

    // Play the timelapse (My favorite part)
    public IEnumerator PlayTimelapse()
    {
        // Turn off the UI
        UIManager uiManager = FindObjectOfType<UIManager>();
        uiManager.ToggleUI();

        // Set the flag to indicate timelapse is playing
        isPlayingTimelapse = true;

        // Clear the current graph
        foreach (Edge edge in new List<Edge>(graph.edges))
        {
            RemoveEdge(edge.vertexA.id, edge.vertexB.id);
        }

        foreach (Vertex vertex in new List<Vertex>(graph.vertices))
        {
            RemoveVertex(vertex.id);
        }

        // Disable user interaction during the timelapse (We don't want to mess up the animation... So... Let's just watch and enjoy)
        uiManager.enabled = false;

        // Reset the graph and actions | Start the timelapse | Wait for the next action | Repeat
        foreach (string action in actions)
        {
            string[] data = action.Split(',');

            if (data[0] == "AddVertex")
            {
                int id = int.Parse(data[1]);
                AddVertex(id);
            }
            else if (data[0] == "RemoveVertex")
            {
                int id = int.Parse(data[1]);
                RemoveVertex(id);
            }
            else if (data[0] == "AddEdge")
            {
                int idA = int.Parse(data[1]);
                int idB = int.Parse(data[2]);
                float weight = float.Parse(data[3]);
                AddEdge(idA, idB, weight);
            }
            else if (data[0] == "RemoveEdge")
            {
                int idA = int.Parse(data[1]);
                int idB = int.Parse(data[2]);
                RemoveEdge(idA, idB);
            }

            // Wait for the next action
            yield return new WaitForSeconds(timelapsePopSpeed);
        }

        // Re-enable user interaction
        uiManager.enabled = true;

        // Reset the flag after timelapse
        isPlayingTimelapse = false;
    }

    // Highlight the path | Let's make it shine!
    public void HighlightPath(List<Vertex> path)
    {
        // First, reset the colors of all edges to white
        foreach (Edge edge in graph.edges)
        {
            edge.GetComponent<LineRenderer>().startColor = Color.white;
            edge.GetComponent<LineRenderer>().endColor = Color.white;
        }

        // Then, highlight the edges in the path with yellow
        for (int i = 0; i < path.Count - 1; i++)
        {
            Vertex from = path[i];
            Vertex to = path[i + 1];

            Edge edge = graph.edges.Find(e => (e.vertexA == from && e.vertexB == to) || (!graph.isDirected && e.vertexA == to && e.vertexB == from));

            if (edge != null)
            {
                edge.GetComponent<LineRenderer>().startColor = Color.yellow;
                edge.GetComponent<LineRenderer>().endColor = Color.yellow;
            }
        }
    }

    // Adjust the sizes of the vertices and edges
    private void AdjustVertexSizes()
    {
        int vertexCount = graph.vertices.Count;
        if (vertexCount == 0)
            return;

        // Min and max scale values for the vertices
        float minScale = 0.01f;
        float maxScale = 1f;

        // Calculate the scale factor based on the number of vertices
        float scaleFactor = Mathf.Clamp(1f / Mathf.Sqrt(vertexCount), minScale, maxScale);

        // Applying the scale factor to all vertices
        foreach (Vertex vertex in graph.vertices)
        {
            vertex.SetScale(scaleFactor);
        }
    }

    // Adjust the widths of the edges
    private void AdjustEdgeWidths()
    {
        // Get the number of vertices
        int vertexCount = graph.vertices.Count;

        // If there are no vertices, return
        if (vertexCount == 0) return;

        // Min and max width values for the edges
        float minWidth = 0.01f;
        float maxWidth = 0.1f;

        // Calculate the width factor based on the number of vertices 
        // (Basically,  We don't want to make it too big while we have 1000 vertices)
        float widthFactor = Mathf.Clamp(0.1f / Mathf.Sqrt(vertexCount), minWidth, maxWidth);

        // Applying the width factor to all edges
        foreach (Edge edge in graph.edges)
        {
            edge.SetLineWidth(widthFactor);
        }
    }

    // Clear the graph
    public void ClearGraph()
    {
        foreach (Edge edge in new List<Edge>(graph.edges))
        {
            RemoveEdge(edge.vertexA.id, edge.vertexB.id);
        }

        foreach (Vertex vertex in new List<Vertex>(graph.vertices))
        {
            RemoveVertex(vertex.id);
        }
    }

    // Check if two vertices are adjacent
    public bool AreVerticesAdjacent(int idA, int idB)
    {
        // Find the vertices by ID
        Vertex vertexA = graph.vertices.Find(v => v.id == idA);
        Vertex vertexB = graph.vertices.Find(v => v.id == idB);

        if (vertexA == null || vertexB == null)
        {
            // One or both vertices not found
            return false;
        }

        // Check if the vertices are adjacent
        if (graph.isDirected)
        {
            // For directed graphs, check outgoing edges from vertexA to vertexB
            return vertexA.outgoingEdges.Any(e => e.vertexB == vertexB);
        }
        else
        {
            // For undirected graphs, check if any edge connects vertexA and vertexB
            return vertexA.edges.Any(e => (e.vertexA == vertexA && e.vertexB == vertexB) || (e.vertexA == vertexB && e.vertexB == vertexA));
        }
    }

    // Get a random spawn position for a vertex (We don't want to spawn it in the water or in the air... Right?)
    private Vector3 GetRandomSpawnPosition()
    {
        // Get the bounds of the water tilemap
        BoundsInt bounds = waterTilemap.cellBounds;
        List<Vector3> validPositions = new();

        // Loop through all positions within the bounds
        foreach (var pos in bounds.allPositionsWithin)
        {
            // Check if the position has a water tile
            Vector3Int localPlace = new(pos.x, pos.y, pos.z);
            if (waterTilemap.HasTile(localPlace))
            {
                // Check if the position has a ground tile
                bool isGround = groundTilemap.HasTile(localPlace);

                //  Convert the local position to world position
                Vector3 worldPos = waterTilemap.CellToWorld(localPlace) + waterTilemap.tileAnchor;

                // Add the position to the list
                if (isGround)
                {
                    // Add the ground position to the beginning of the list (We prefer ground over water)
                    validPositions.Insert(0, worldPos); 
                }
                else
                {
                    // Add the water position to the end of the list
                    validPositions.Add(worldPos);
                }
            }
        }

        // Return a random position from the list
        if (validPositions.Count > 0)
        {
            // Get a random index within the valid positions
            int index = UnityEngine.Random.Range(0, validPositions.Count);
            return validPositions[index];
        }
        else
        {
            // Return zero if no valid positions found
            return Vector3.zero;
        }
    }

}
