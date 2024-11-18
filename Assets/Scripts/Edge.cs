using System.Collections;
using UnityEngine;

public class Edge : MonoBehaviour
{
    // Default Configs
    public Vertex vertexA;
    public Vertex vertexB;
    public float weight = 1f;
    private LineRenderer lineRenderer;

    // Directed Graph Flag
    public bool isDirected;

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // If LineRenderer is not set on the Inspector, try to get it from the GameObject
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        // Configure LineRenderer to have two positions
        lineRenderer.positionCount = 2;

        // Initialize positions
        lineRenderer.SetPosition(0, vertexA.transform.position);
        lineRenderer.SetPosition(1, vertexA.transform.position); // Start from vertexA

        StartCoroutine(DrawEdgeAnimation());

        // Configurar o LineRenderer para usar cores por vértice
        lineRenderer.colorGradient = new Gradient();

        UpdatePosition();
        UpdateColor();
    }

    void LateUpdate()
    {
        // Update the positions of the edge every frame
        UpdatePosition();
    }

    public void UpdatePosition()
    {
        if (vertexA != null && vertexB != null)
        {
            lineRenderer.SetPosition(0, vertexA.transform.position);
            lineRenderer.SetPosition(1, vertexB.transform.position);

            UpdateColor();
        }
    }

    public void SetLineWidth(float width)
    {
        if (lineRenderer != null)
        {
            lineRenderer.startWidth = width;
            lineRenderer.endWidth = width;
        }
    }

    public void UpdateColor()
    {
        if (isDirected)
        {
            // Criar um gradiente de vermelho para laranja
            Gradient gradient = new();
            gradient.SetKeys(
                new GradientColorKey[] {
                    new(Color.black, 0.0f),
                    new(Color.magenta, 1.0f)
                },
                new GradientAlphaKey[] {
                    new(1.0f, 0.0f),
                    new(1.0f, 1.0f)
                }
            );
            lineRenderer.colorGradient = gradient;
        }
        else
        {
            // Cor sólida para grafos não direcionados (por exemplo, branco)
            Gradient gradient = new();
            gradient.SetKeys(
                new GradientColorKey[] {
                    new(Color.red, 0.0f),
                    new(Color.red, 1.0f)
                },
                new GradientAlphaKey[] {
                    new(1.0f, 0.0f),
                    new(1.0f, 1.0f)
                }
            );
            lineRenderer.colorGradient = gradient;
        }
    }

    // Animation to draw the edge from vertexA to vertexB
    IEnumerator DrawEdgeAnimation()
    {
        // Total duration of the animation
        float duration = 1f;
        // Total time passed since the animation started
        float elapsedTime = 0f;

        // Start and end points of the edge
        Vector3 startPoint = vertexA.transform.position;
        Vector3 endPoint = vertexB.transform.position;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            Vector3 currentPoint = Vector3.Lerp(startPoint, endPoint, t);
            lineRenderer.SetPosition(1, currentPoint);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Just to make sure the edge is drawn completely at the end of the animation
        lineRenderer.SetPosition(1, endPoint);
    }
}
