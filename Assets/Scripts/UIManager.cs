using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;
using TMPro;

public class UIManager : MonoBehaviour
{
    // Variables
    [Header("Panels")]
    public GameObject interactionPanel;
    public GameObject physicsPanel;
    public GameObject loadFilePanel;

    [Header("Input Fields")]
    [Header("Vertex")]
    public TMP_InputField vertexIdInputField;
    public TMP_InputField degreeVertexInputField;
    public TMP_InputField adjacentyAVertexInputField;
    public TMP_InputField adjacentyBVertexInputField;
    [Header("Edge")]
    public TMP_InputField edgeVertexAInputField;
    public TMP_InputField edgeVertexBInputField;
    public TMP_InputField edgeWeightInputField;

    [Header("Toggles")]
    public Toggle isDirectedToggle;
    public Toggle isWeightedToggle;

    [Header("Buttons")]
    [Header("Toggle UI")]
    public Button toggleUIButton;
    [Header("Vertex")]
    public Button addVertexButton;
    public Button removeVertexButton;
    [Header("Edge")]
    public Button addEdgeButton;
    public Button removeEdgeButton;
    [Header("Load File")]
    public Button loadfileButton;
    public Button showTimeLoadFileButton;
    public Button closeShowTimeLoadFileButton;
    [Header("Timelapse")]
    public Button startTimelapseButton;
    [Header("Shortest Path")]
    public Button findShortestPathButton;
    [Header("Clear Graph")]
    public Button clearGraphButton;
    [Header("Physics")]
    public Button physicsConfigsButton;
    public Button togglePhysicsButton;
    [Header("Vertex Degree")]
    public Button getVertexDegreeButton;
    [Header("Adjacency Check")]
    public Button checkAdjacencyButton;

    [Header("Sliders")]
    public Slider centerForceSlider;
    public Slider repelForceSlider;
    public Slider linkForceSlider;

    [Header("Dropdown")]
    public TMP_Dropdown fileDropdown;

    [Header("Text Info")]
    public TMP_Text orderText;
    public TMP_Text sizeText;

    [Header("Shortest Path")]
    public TMP_InputField startVertexInputField;
    public TMP_InputField endVertexInputField;
    public TMP_Text shortestPathText;

    [Header("File Name")]
    public string filePath = "easy";
    public string filePath2 = "calma";
    public string filePath3 = "EUDISSECALMA";
    public string filePath4 = "GrafoEmLote";

    // Manager
    [HideInInspector]
    private GraphManager graphManager;

    // Start is called before the first frame update
    void Start()
    {
        graphManager = FindObjectOfType<GraphManager>();

        // listeners buttons
        toggleUIButton.onClick.AddListener(ToggleUI);
        togglePhysicsButton.onClick.AddListener(OnTogglePhysicsButtonClicked);
        physicsConfigsButton.onClick.AddListener(TogglePhysicsUI);
        addVertexButton.onClick.AddListener(OnAddVertexButtonClicked);
        removeVertexButton.onClick.AddListener(OnRemoveVertexButtonClicked);
        addEdgeButton.onClick.AddListener(OnAddEdgeButtonClicked);
        removeEdgeButton.onClick.AddListener(OnRemoveEdgeButtonClicked);
        getVertexDegreeButton.onClick.AddListener(OnGetVertexDegreeButtonClicked);
        checkAdjacencyButton.onClick.AddListener(OnCheckAdjacencyButtonClicked);
        showTimeLoadFileButton.onClick.AddListener(OnLoadCSVButtonClicked);
        closeShowTimeLoadFileButton.onClick.AddListener(HideLoadFilePanel);
        loadfileButton.onClick.AddListener(ShowLoadFilePanel);
        startTimelapseButton.onClick.AddListener(OnStartTimelapseButtonClicked);
        findShortestPathButton.onClick.AddListener(OnCalculateShortestPathButtonClicked);
        clearGraphButton.onClick.AddListener(OnClearGraphButtonClicked);
        isDirectedToggle.onValueChanged.AddListener(OnIsDirectedToggleChanged);
        isWeightedToggle.onValueChanged.AddListener(OnIsWeightedToggleChanged);

        // listeners sliders
        centerForceSlider.onValueChanged.AddListener(OnCenterForceSliderChanged);
        repelForceSlider.onValueChanged.AddListener(OnRepelForceSliderChanged);
        linkForceSlider.onValueChanged.AddListener(OnLinkForceSliderChanged);

        // Physics!
        centerForceSlider.value = graphManager.centerForceMultiplier;
        repelForceSlider.value = graphManager.repelForceMultiplier;
        linkForceSlider.value = graphManager.linkForceStiffness;

        // Weighted Graph
        edgeWeightInputField.interactable = isWeightedToggle.isOn;

        // Filling the dropdown with the file names
        fileDropdown.options.Clear();
        fileDropdown.options.Add(new TMP_Dropdown.OptionData(filePath));
        fileDropdown.options.Add(new TMP_Dropdown.OptionData(filePath2));
        fileDropdown.options.Add(new TMP_Dropdown.OptionData(filePath3));
        fileDropdown.options.Add(new TMP_Dropdown.OptionData(filePath4));
    }

    // Show or Hide the Main UI
    public void ToggleUI()
    {
        interactionPanel.SetActive(!interactionPanel.activeSelf);
        if (interactionPanel.activeSelf)
        {
            UpdateGraphInfo();
        }
        else
        {
            physicsPanel.SetActive(false);
        }
    }

    // Hide the Load File Panel
    public void HideLoadFilePanel()
    {
        loadFilePanel.SetActive(false);
    }

    // Show the Load File Panel
    public void ShowLoadFilePanel()
    {
        loadFilePanel.SetActive(true);
    }

    // Show or Hide the Physics UI
    void TogglePhysicsUI()
    {
        physicsPanel.SetActive(!physicsPanel.activeSelf);
    }

    // Play or Stop the Physics!
    void OnTogglePhysicsButtonClicked()
    {
        graphManager.TogglePhysics();

    }

    // Adding a new Vertex
    void OnAddVertexButtonClicked()
    {
        int id;
        if (string.IsNullOrEmpty(vertexIdInputField.text))
        {
            // I put this so we don't have to keep typing a lot of numbers | Generates a random ID between 1 and 1000
            id = Random.Range(1, 1001);
            vertexIdInputField.text = id.ToString();
        }
        else if (int.TryParse(vertexIdInputField.text, out id))
        {
            // Valid ID entered by the user
        }
        else
        {
            // Invalid ID or Some had a typo | Only accept int | Show a message
            shortestPathText.text = "Invalid vertex ID.";
            return;
        }

        // Add the vertex to the graph
        graphManager.AddVertex(id);

        // Update the graph info text box
        UpdateGraphInfo();

        // Clear the input field
        vertexIdInputField.text = null;
    }

    // Removing a Vertex
    void OnRemoveVertexButtonClicked()
    {
        // Here we have to type a Vertex ID number to be able to remove it
        if (int.TryParse(vertexIdInputField.text, out int id))
        {
            graphManager.RemoveVertex(id);
            UpdateGraphInfo();
            vertexIdInputField.text = null;
        }
        else
        {
            // Typo or Invalid ID | Show a message
            shortestPathText.text = "Invalid vertex ID.";
        }
    }

    // Adding a new Edge
    void OnAddEdgeButtonClicked()
    {
        // Default weight
        float weight = 1f;

        // Get the list of vertices
        List<Vertex> vertices = graphManager.graph.vertices;

        // If there are less than 2 vertices, we can't create an edge (duhh)
        if (vertices.Count < 2)
        {
            shortestPathText.text = "Not enough vertices to create an edge.";
            return;
        }

        // Parse the input fields | If the ID is invalid, we pick a random Vertex
        int idA;
        int idB;

        bool idAParsed = int.TryParse(edgeVertexAInputField.text, out idA) && vertices.Exists(v => v.id == idA);
        bool idBParsed = int.TryParse(edgeVertexBInputField.text, out idB) && vertices.Exists(v => v.id == idB);

        // If Id "A" is not parsed or invalid, we pick a random vertex
        if (!idAParsed)
        {
            idA = vertices[Random.Range(0, vertices.Count)].id;
        }

        // Creating a list of possible ID "B's" (excluding ID "A")
        List<int> possibleIdBs = vertices.Select(v => v.id).Where(id => id != idA).ToList();

        // If ID "B" is not parsed or invalid, or ID "B" == ID "A", so... we pick a random vertex different from ID "A"
        if (!idBParsed || idB == idA)
        {
            // If there are no possible ID "B's", we can't create an edge. So.... we show a message = D
            idB = possibleIdBs[Random.Range(0, possibleIdBs.Count)];
        }

        // If the graph is weighted, we parse the weight
        if (graphManager.graph.isWeighted)
        {
            // If the weight is invalid, we use the default weight
            if (!float.TryParse(edgeWeightInputField.text, out weight))
            {
                shortestPathText.text = "Invalid weight, using default weight 1.";
                weight = 1f;
            }
        }

        // Add the edge to the graph
        graphManager.AddEdge(idA, idB, weight);

        // Update the graph info text box
        UpdateGraphInfo();

        // Clear the input fields
        edgeVertexAInputField.text = null;
        edgeVertexBInputField.text = null;
        edgeWeightInputField.text = null;
    }

    // Removing an Edge
    void OnRemoveEdgeButtonClicked()
    {
        // Parse the input fields
        if (int.TryParse(edgeVertexAInputField.text, out int idA) && int.TryParse(edgeVertexBInputField.text, out int idB))
        {
            // Remove the edge from the graph
            graphManager.RemoveEdge(idA, idB);

            // Update the graph info text box
            UpdateGraphInfo();

            // Clear the input fields
            edgeVertexAInputField.text = null;
            edgeVertexBInputField.text = null;
        }
        else
        {
            // Invalid IDs or Typo | Show a message
            shortestPathText.text = "Invalid vertex IDs.";
        }
    }

    // Load a CSV file (The file must be in the Resources folder inside the project)
    void OnLoadCSVButtonClicked()
    {
        // Load the CSV file from the Resources folder (We can choose the file from the dropdown)
        string csvFileName = fileDropdown.options[fileDropdown.value].text;
        TextAsset csvFile = Resources.Load<TextAsset>(csvFileName);

        // Show a message while loading the file
        shortestPathText.text = "Loading CSV file: " + csvFileName;
        
        // If the file is found, we import the graph
        if (csvFile != null)
        {
            string fileContent = csvFile.text;

            // Activate the toggles
            isWeightedToggle.isOn = true;
            isDirectedToggle.isOn = true;

            // Update graph properties
            graphManager.graph.isWeighted = true;
            graphManager.graph.isDirected = true;

            // Update UI elements
            OnIsWeightedToggleChanged(true);
            OnIsDirectedToggleChanged(true);

            // Let's just enjoy the show | hide the UIs
            ToggleUI();
            HideLoadFilePanel();

            // Import the graph from CSV content
            graphManager.ImportGraphFromCSV(fileContent);
            UpdateGraphInfo();
        }
        else
        {
            shortestPathText.text = "CSV file not found in Resources.";
        }
    }

    // Start the timelapse
    void OnStartTimelapseButtonClicked()
    {
        // Start the coroutine
        StartCoroutine(graphManager.PlayTimelapse());
    }

    // Toggle the Directed Graph
    void OnIsDirectedToggleChanged(bool isDirected)
    {
        graphManager.graph.isDirected = isDirected;
        graphManager.UpdateEdgesDirection();
    }

    // Toggle the Weighted Graph
    void OnIsWeightedToggleChanged(bool isWeighted)
    {
        graphManager.graph.isWeighted = isWeighted;
        edgeWeightInputField.interactable = isWeighted;
    }

    // Update the graph info text boxes
    void UpdateGraphInfo()
    {
        orderText.text = graphManager.GetOrder().ToString();
        sizeText.text = graphManager.GetSize().ToString();
    }

    // Calculate the shortest path between two vertices
    void OnCalculateShortestPathButtonClicked()
    {
        if (int.TryParse(startVertexInputField.text, out int startId) && int.TryParse(endVertexInputField.text, out int endId))
        {
            var result = graphManager.GetShortestPath(startId, endId);

            if (result.Item1 != null)
            {
                // Show the shortest path in the text box (IDs of the vertices)
                string pathString = "Shortest Path: ";
                foreach (Vertex v in result.Item1)
                {
                    pathString += v.id + " -> ";
                }
                // Remove the last " -> "
                pathString = pathString.Remove(pathString.Length - 4);

                // Show the total cost of the path
                pathString += "\nTotal cost: " + result.Item2;
                shortestPathText.text = pathString;

                // Highlight the path in the graph (It just changes colors)
                graphManager.HighlightPath(result.Item1);
            }
            else
            {
                shortestPathText.text = "No Path found.";
            }
        }
        else
        {
            shortestPathText.text = "Invalid IDs.";
        }
    }

    // Get the degree of a vertex
    void OnGetVertexDegreeButtonClicked()
    {
        // Parse the input field
        if (int.TryParse(degreeVertexInputField.text, out int vertexId))
        {
            // Get the degree of the vertex
            var (degree, inDegree, outDegree) = graphManager.GetVertexDegree(vertexId);

            // Show the degree in the text box
            if (degree != -1)
            {
                // Show the degree of the vertex in the text box if it exists and if the graph is directed show the in and out degree
                if (graphManager.graph.isDirected)
                {
                    shortestPathText.text = $"Vertex {vertexId} Degree: {degree}\nIn-Degree: {inDegree}\nOut-Degree: {outDegree}";
                }
                else
                {
                    // If the graph is not directed, we only show the degree
                    shortestPathText.text = $"Vertex {vertexId} Degree: {degree}";
                }
            }
            else
            {
                shortestPathText.text = "Vertex not found.";
            }
        }
        else
        {
            shortestPathText.text = "Invalid vertex ID.";
        }
    }

    // Check if two vertices are adjacent
    void OnCheckAdjacencyButtonClicked()
    {
        // Parse the input fields
        if (int.TryParse(adjacentyAVertexInputField.text, out int idA) && int.TryParse(adjacentyBVertexInputField.text, out int idB))
        {
            // Check if the vertices are adjacent
            bool adjacent = graphManager.AreVerticesAdjacent(idA, idB);

            // Show the result in the text box
            if (adjacent)
            {
                shortestPathText.text = $"Vertices {idA} and {idB} are adjacent.";
            }
            else
            {
                shortestPathText.text = $"Vertices {idA} and {idB} are not adjacent.";
            }
        }
        else
        {
            shortestPathText.text = "Invalid vertex IDs.";
        }
    }

    // Clear the graph
    void OnClearGraphButtonClicked()
    {
        graphManager.ClearGraph();
        UpdateGraphInfo();
    }

    // Physics Configs
    void OnCenterForceSliderChanged(float value)
    {
        graphManager.centerForceMultiplier = value;
    }

    void OnRepelForceSliderChanged(float value)
    {
        graphManager.repelForceMultiplier = value;
    }

    void OnLinkForceSliderChanged(float value)
    {
        graphManager.linkForceStiffness = value;
    }

}
