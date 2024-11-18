using System.Collections.Generic;

// Graph class
public class Graph
{
    // List of vertices and edges and Flags
    public List<Vertex> vertices = new();
    public List<Edge> edges = new();
    public bool isDirected = false;
    public bool isWeighted = false;
    public bool isConnected = false;
}
