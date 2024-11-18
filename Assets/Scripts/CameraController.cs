using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraController : MonoBehaviour
{
    // Pan (Or movement) and Zoom parameters
    public float panSpeed = 1f;
    public float zoomSpeed = 5f;
    public float minZoom = 2f;
    public float maxZoom = 10f;

    // Variables for mouse position tracking
    private Vector3 lastMousePosition;
    private Vector3 dragOrigin;

    // Variables for camera bounds
    private Vector3 minCameraPosition;
    private Vector3 maxCameraPosition;

    // Reference to the Water Tilemap
    public Tilemap waterTilemap;

    void Start()
    {
        // Initializing the camera bounds based on the size of water tilemap
        if (waterTilemap != null)
        {
            // Get the bounds of the water tilemap in world space
            Bounds tilemapBounds = waterTilemap.localBounds;

            minCameraPosition = tilemapBounds.min;
            maxCameraPosition = tilemapBounds.max;

            // Adjust bounds based on initial zoom of the Camera
            AdjustCameraBounds();
        }
        else
        {
            Debug.LogError("Water Tilemap is not assigned in the CameraController.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        HandleZoom();
        HandlePan();
    }

    // Handle Zooming In and Out
    void HandleZoom()
    {
        // Zoom with mouse scroll wheel
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        // If the scroll is not zero
        if (scroll != 0f)
        {
            // Zoom the camera
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - scroll * zoomSpeed, minZoom, maxZoom);

            // Recalculate camera bounds after zooming
            AdjustCameraBounds();

            // Ensure the camera is within bounds after zoom
            Vector3 clampedPosition = transform.position;
            clampedPosition.x = Mathf.Clamp(clampedPosition.x, minCameraPosition.x, maxCameraPosition.x);
            clampedPosition.y = Mathf.Clamp(clampedPosition.y, minCameraPosition.y, maxCameraPosition.y);

            // Update the camera position
            transform.position = clampedPosition;
        }
    }

    // Handle Panning (Or movement of the camera inside the map)
    void HandlePan()
    {
        // Move with middle mouse button
        if (Input.GetMouseButtonDown(2))
        {
            // Saving the position where the dragging starts (In the case of the camera going out of bounds)
            dragOrigin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(2))
        {
            // Calculate the difference between the current mouse position and the origin
            Vector3 difference = dragOrigin - Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Move the camera by the difference positions
            Vector3 newPosition = transform.position + difference;

            // Clamping the new position within the camera bounds
            newPosition.x = Mathf.Clamp(newPosition.x, minCameraPosition.x, maxCameraPosition.x);
            newPosition.y = Mathf.Clamp(newPosition.y, minCameraPosition.y, maxCameraPosition.y);

            // Update the camera position (finally)
            transform.position = newPosition;
        }
    }

    // Adjust the camera bounds based on the zoom level
    void AdjustCameraBounds()
    {
        // We need to get the size of the camera in world units after zoom
        float cameraHeight = Camera.main.orthographicSize * 2f;
        float cameraWidth = cameraHeight * Camera.main.aspect;

        // And then adjust min and max camera positions based on the camera size
        Bounds tilemapBounds = waterTilemap.localBounds;

        // And finally we set the camera bounds based on the size of the water tilemap
        minCameraPosition.x = tilemapBounds.min.x + cameraWidth / 2f;
        maxCameraPosition.x = tilemapBounds.max.x - cameraWidth / 2f;
        minCameraPosition.y = tilemapBounds.min.y + cameraHeight / 2f;
        maxCameraPosition.y = tilemapBounds.max.y - cameraHeight / 2f;
    }
}
