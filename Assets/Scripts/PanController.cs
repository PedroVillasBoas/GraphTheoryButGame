using UnityEngine;

public class PanController : MonoBehaviour
{
    public float panSpeed = 20f;

    void Update()
    {
        if (Input.GetMouseButton(2)) // Botão do meio do mouse
        {
            float h = -Input.GetAxis("Mouse X") * panSpeed * Time.deltaTime;
            float v = -Input.GetAxis("Mouse Y") * panSpeed * Time.deltaTime;

            Camera.main.transform.Translate(h, v, 0);
        }
    }
}
