using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMouseController : MonoBehaviour
{
    bool isDraggingCamera = false;
    Vector3 lastMousePosition;

    void Update()
    {
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        // What is the point at which the mouse ray intersects y=0
        float rayLength = mouseRay.origin.y / mouseRay.direction.y;
        Vector3 hitPosition = mouseRay.origin - (mouseRay.direction * rayLength);

        if (Input.GetMouseButtonDown(0))
        {
            isDraggingCamera = true;
            lastMousePosition = hitPosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDraggingCamera = false;
        }

        if (isDraggingCamera)
        {
            Vector3 diff = lastMousePosition - hitPosition;
            Camera.main.transform.Translate(diff, Space.World);
            mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            // What is the point at which the mouse ray intersects y=0
            rayLength = mouseRay.origin.y / mouseRay.direction.y;
            lastMousePosition = mouseRay.origin - (mouseRay.direction * rayLength);
        }
    }
}
