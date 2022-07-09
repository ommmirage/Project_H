using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardController : MonoBehaviour
{
    HexMap hexMap;

    float moveSpeed = 20f;

    void Start()
    {
        hexMap = Object.FindObjectOfType<HexMap>();
    }

    void Update()
    {
        CameraController();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            hexMap.DoTurn();
        }
    }

    private void CameraController()
    {
        Vector3 translate = new Vector3(
                    Input.GetAxis("Horizontal"),
                    0,
                    Input.GetAxis("Vertical")
                    );

        transform.Translate(translate * moveSpeed * Time.deltaTime, Space.World);
    }
}
