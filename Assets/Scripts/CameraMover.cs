using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMover : MonoBehaviour
{
    HexMap hexMap;

    Vector3 oldPosition;

    void Start()
    {
        hexMap = FindObjectOfType<HexMap>();

        oldPosition = transform.position;
    }

    void Update()
    {
        CheckIfCameraMoved();
    }

    void CheckIfCameraMoved()
    {
        if (oldPosition != transform.position)
        {
            oldPosition = transform.position;

            hexMap.UpdateHexPositions();
        }
    }
}
