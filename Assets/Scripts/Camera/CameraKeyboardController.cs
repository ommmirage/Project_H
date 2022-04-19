using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraKeyboardController : MonoBehaviour
{
    HexMap hexMap;

    float moveSpeed = 20f;

    void Start()
    {
        hexMap = Object.FindObjectOfType<HexMap>();
    }

    void Update()
    {
        Vector3 translate = new Vector3(
            Input.GetAxis("Horizontal"),
            0,
            Input.GetAxis("Vertical")
            );

        transform.Translate( translate * moveSpeed * Time.deltaTime, Space.World);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            hexMap.DoTurn();
        }
    }
}
