using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine;
using TMPro;

public class Vertex : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    // Default Configs
    public int id;
    public Vector2 position;
    public List<Edge> edges = new();
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    // Vertex Text ID
    private TextMeshPro textMesh;

    // Directed Graphs
    public List<Edge> incomingEdges = new();
    public List<Edge> outgoingEdges = new();

    // Mouse Event
    public bool isDragging = false;

    // Physics variables
    [HideInInspector]
    public Rigidbody2D rb;
    public float mass = 0.1f;

    // Start is called before the first frame update
    void Start()
    {

        // Getting Components
        spriteRenderer = GetComponent<SpriteRenderer>();
        textMesh = GetComponentInChildren<TextMeshPro>();

        // Initial Configs
        originalColor = spriteRenderer.color;

        if (textMesh != null)
        {
            textMesh.text = id.ToString();
            textMesh.enabled = false;
        }

        // Physics!
        AddPhysics();

        // Pop! Animation
        StartCoroutine(PopAnimation());
    }

    // Update is called once per frame
    void Update()
    {
        if (isDragging)
        {
            // Convert mouse position to world space
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            rb.MovePosition(new Vector2(mousePosition.x, mousePosition.y));

            UpdateEdges();
        }
    }

    // Set the scale of the vertex
    public void SetScale(float scale)
    {
        transform.localScale = new Vector3(scale, scale, 1f);
    }

    // Mouse Left Click | Dragging Vertex
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            isDragging = true;
            
            // Disable physics
            rb.isKinematic = true;
        }
    }

    // Drag Event | Moving Vertex
    public void OnDrag(PointerEventData eventData)
    {
        // Handled in OnPointerDown and OnMouseUp
    }

    // Mouse Right Click | Copy Vertex ID to Clipboard
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            CopyIDToClipboard();
        }
    }

    // Copy Vertex ID to Clipboard | Just a Quality of Life Improvement
    private void CopyIDToClipboard()
    {
        GUIUtility.systemCopyBuffer = id.ToString();
    }

    // Mouse Hover | Change Vertex Color
    public void OnPointerEnter(PointerEventData eventData)
    {
        spriteRenderer.color = Color.black;

        // Show ID text
        if (textMesh != null)
        {
            textMesh.enabled = true;
        }
    }

    // Mouse Exit | Change Back the Vertex Color
    public void OnPointerExit(PointerEventData eventData)
    {
        spriteRenderer.color = originalColor;

        // Hide ID text
        if (textMesh != null)
        {
            textMesh.enabled = false;
        }
    }

    // Left Mouse Release | Stop Dragging Vertex
    void OnMouseUp()
    {
        isDragging = false;
        
        // Re-enable physics
        rb.isKinematic = false;
    }

    // Update the position of the edges connected to this vertex
    // This method is called every frame while dragging the vertex
    void UpdateEdges()
    {
        foreach (Edge edge in edges)
        {
            edge.UpdatePosition();
        }
    }

    // Return the count of edges connected to a vertex
    public int GetDegree()
    {
        return edges.Count;
    }

    // Return the count of incoming edges connected to a vertex
    public int GetInDegree()
    {
        return incomingEdges.Count;
    }

    // Return the count of outgoing edges connected to a vertex
    public int GetOutDegree()
    {
        return outgoingEdges.Count;
    }

    // Pop! Pop! Pop! Pop! Pop! Pop! Pop!
    // Animation to make the vertex Pop! when it's Spawned
    IEnumerator PopAnimation()
    {
        // Total duration of the animation
        float duration = 0.4f; 
        float elapsedTime = 0f;

        // Initial size of the vertex
        Vector3 startScale = Vector3.zero;
        // Overshoot scale to make the vertex Pop!
        Vector3 overshootScale = transform.localScale * 1.4f;
        // Original size of the Vertex
        Vector3 targetScale = transform.localScale;

        // Setting the initial size to zero
        transform.localScale = startScale;

        // First phase: Scale up to overshoot scale
        while (elapsedTime < duration / 2f)
        {
            transform.localScale = Vector3.Lerp(startScale, overshootScale, elapsedTime / (duration / 2f));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Reset the time for the second phase
        elapsedTime = 0f;

        // Second phase: Scale down to target scale
        while (elapsedTime < duration / 2f)
        {
            transform.localScale = Vector3.Lerp(overshootScale, targetScale, elapsedTime / (duration / 2f));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Just to make sure the size is set to the default size at the end of the animation
        transform.localScale = targetScale;
    }

    // Physics!
    public void AddPhysics()
    {
        // Adding a Rigidbody2D component to the vertex
        rb = gameObject.AddComponent<Rigidbody2D>();
        // Setting the Rigidbody2D Default properties
        rb.mass = mass;
        rb.gravityScale = 0f;
        // Damping to stabilize the movement and make it more natural
        rb.drag = 2f;
        // I got dizzy watching this, so... No rotation to the Vertex
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; 
    }

    // Turn ON the Physics :)
    public void EnablePhysics()
    {
        rb.isKinematic = false;
    }

    // Turn OFF the Physics :(
    public void DisablePhysics()
    {
        // Stoping all movement from the Vertex
        rb.isKinematic = true;
        rb.velocity = Vector2.zero; 
        rb.angularVelocity = 0f;
    }
}
