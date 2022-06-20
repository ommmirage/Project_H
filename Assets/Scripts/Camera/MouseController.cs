using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour
{
	[SerializeField] LayerMask layerMask;

	// Variables for units selecting
	Vector3 lastMousePosition;

	// Camera moving variables
    Vector3 lastMouseGroundPlanePosition;
	Vector3 cameraTargetOffset;
	int mouseDragThreshold = 2;

	delegate void UpdateFunc();
	UpdateFunc Update_CurrentFunc;

	Unit selectedUnit = null;
	Hex selectedHex = null;
	HexMap hexMap;
	Hex previousEndHex;
	List<Hex> path = new List<Hex>();

	void Start()
	{
		hexMap = Object.FindObjectOfType<HexMap>();
		Update_CurrentFunc = Update_DetectModeStart;
	}

	void Update()
	{
		Update_CurrentFunc();
		Update_Zoom();

		lastMousePosition = Input.mousePosition;
	}

	void CancelUpdateFunc()
	{
		Update_CurrentFunc = Update_DetectModeStart;
	}

	void Update_DetectModeStart()
	{
		float mousePositionDiff = Vector3.Distance(Input.mousePosition, lastMousePosition);

		if (Input.GetMouseButtonUp(0))
		{
			SelectUnit();
		}
		else if ( Input.GetMouseButton(0) && (mousePositionDiff > mouseDragThreshold) )
		{
			Update_CurrentFunc = Update_CameraDrag;
			lastMouseGroundPlanePosition = MouseToGroundPlane(Input.mousePosition);
			Update_CurrentFunc();
		}
		else if ( (selectedUnit != null) && (!selectedUnit.IsOnPath) )
        {
            ProceedSelectedUnit();
        }
    }

    void ProceedSelectedUnit()
    {
        Hex endHex = MouseToHex();
        Pathfinding pathfinding = new Pathfinding();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            selectedUnit = null;
            selectedHex.SetSelected(false);
            pathfinding.ClearPath(path);
			return;
        }

        if (previousEndHex != endHex)
            CursorMovedToAnotherHex(endHex, pathfinding);

		if (Input.GetMouseButton(1))
		{
			if (selectedUnit.IsOnPath)
			{
				
			}
			selectedUnit.Move(path);
			selectedUnit.IsOnPath = true;
		}
    }

    void CursorMovedToAnotherHex(Hex endHex, Pathfinding pathfinding)
    {
        previousEndHex = endHex;
        pathfinding.ClearPath(path);
        path = pathfinding.FindPath(selectedUnit, selectedHex, endHex);
        pathfinding.DrawPath(path, selectedUnit);
    }

    void Update_CameraDrag()
    {
		if (Input.GetMouseButtonUp(0))
		{
			CancelUpdateFunc();
			return;
		}

        Vector3 hitPos = MouseToGroundPlane(Input.mousePosition);

		Vector3 diff = lastMouseGroundPlanePosition - hitPos;
		Camera.main.transform.Translate (diff, Space.World);

		lastMouseGroundPlanePosition = MouseToGroundPlane(Input.mousePosition);
    }

	void Update_Zoom()
	{
		// Zoom to scrollwheel
		float scrollAmount = Input.GetAxis ("Mouse ScrollWheel");
		float minHeight = 2;
		float maxHeight = 20;
		// Move camera towards hitPos
        Vector3 hitPos = MouseToGroundPlane(Input.mousePosition);

		Vector3 dir = hitPos - Camera.main.transform.position;

		Vector3 p = Camera.main.transform.position;

			// Stop zooming out at a certain distance.
			// TODO: Maybe you should still slide around at 20 zoom?
			if (scrollAmount > 0 || p.y < (maxHeight - 0.1f)) {
				cameraTargetOffset += dir * scrollAmount;
			}
			Vector3 lastCameraPosition = Camera.main.transform.position;
			Camera.main.transform.position = Vector3.Lerp(
											Camera.main.transform.position, 
											Camera.main.transform.position + cameraTargetOffset, 
											Time.deltaTime * 5f
											);
			cameraTargetOffset -= Camera.main.transform.position - lastCameraPosition;

		p = Camera.main.transform.position;
		if ((p.y < minHeight) || (p.y > maxHeight)) 
		{
			Camera.main.transform.position = lastCameraPosition;
		}

		// Change camera angle
		float lowZoom = minHeight + 3;
		float highZoom = maxHeight - 10;

		Camera.main.transform.rotation = Quaternion.Euler (
			Mathf.Lerp (30, 75, Camera.main.transform.position.y / maxHeight),
			Camera.main.transform.rotation.eulerAngles.y,
			Camera.main.transform.rotation.eulerAngles.z
		);
	}

	Vector3 MouseToGroundPlane(Vector3 mousePos)
	{
		Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

		// What is the point at which the mouse ray intersects Y=0
		float rayLength = mouseRay.origin.y / mouseRay.direction.y;
		return mouseRay.origin - (mouseRay.direction * rayLength);
	}

    void SelectUnit()
	{
		Hex hex = MouseToHex();
		selectedUnit = hex.GetUnit();
		if (selectedUnit != null)
		{
			selectedHex = hex;
			hex.SetSelected(true);
		}
	}

	Hex MouseToHex()
	{
		Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hitInfo;

		if (Physics.Raycast(mouseRay, out hitInfo, Mathf.Infinity, layerMask.value))
		{
			GameObject hexGameObject = hitInfo.transform.parent.gameObject;
			Hex hex = hexMap.GameObjectToHexDictionary[hexGameObject];

			return hex;
		}

		return null;
	}
}
