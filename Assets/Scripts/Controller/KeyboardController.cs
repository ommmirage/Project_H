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

    void CameraController()
    {
        Vector3 translate = new Vector3(Input.GetAxis("Horizontal"),
                                        0,
                                        Input.GetAxis("Vertical"));

        Ray downRay = Camera.main.ViewportPointToRay(new Vector3());
		Ray upRay = Camera.main.ViewportPointToRay(new Vector3(0, 1, 0));

		if ( ( Physics.Raycast(downRay) && Physics.Raycast(upRay) ) ||
             ( !Physics.Raycast(downRay) && (translate.z > 0) ) ||
             ( !Physics.Raycast(upRay) && (translate.z < 0) ) )
        {
		    transform.Translate(translate * moveSpeed * Time.deltaTime, Space.World);
		}
        else if (translate.x != 0)
        {
            translate.z = 0;
			transform.Translate(translate * moveSpeed * Time.deltaTime, Space.World);
        }
    }
}
