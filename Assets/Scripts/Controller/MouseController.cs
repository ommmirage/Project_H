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

	Unit unit = null;
	Hex selectedHex = null;
	public Hex SelectedHex { get { return selectedHex; } }
	HexMap hexMap;
	Hex previousTargetHex;
	LinkedList<PathHex> path = new LinkedList<PathHex>();
	Pathfinding pathfinding;
	public bool OnPause = false;

	void Start()
	{
		hexMap = Object.FindObjectOfType<HexMap>();
		Update_CurrentFunc = Update_DetectModeStart;
		pathfinding = new Pathfinding();
	}

	void Update()
	{
		if (OnPause)
			return;

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
		// We dragged a map
		else if ( Input.GetMouseButton(0) && (mousePositionDiff > mouseDragThreshold) )
		{
			Update_CurrentFunc = Update_CameraDrag;
			lastMouseGroundPlanePosition = MouseToGroundPlane(Input.mousePosition);
			Update_CurrentFunc();
		}
		else if (unit != null)
        {
			ProceedUnit();
        }
    }

	void ProceedUnit()
	{
		Hex targetHex = MouseToHex();

		// If we hold right mouse button and change the target hex
        if ( (Input.GetMouseButton(1)) && (previousTargetHex != targetHex) )
        {
			previousTargetHex = targetHex;
            path = pathfinding.RedrawPath(path, unit, targetHex);
        }
        else if (Input.GetMouseButtonUp(1) && (!unit.FinishedMove))
        {
        	previousTargetHex = targetHex;
			path = pathfinding.FindPath(unit, unit.GetHex(), targetHex);
            MoveUnit();
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
			unit.IsSelected = false;
            unit = null;
            selectedHex.SetSelected(false);
			selectedHex = null;
            pathfinding.ClearPath(path);
        }
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

	// Light version of zoom
	void Update_Zoom()
	{
		float scrollAmount = Input.GetAxis ("Mouse ScrollWheel");
		float minHeight = 2;
		float maxHeight = 20;
		float zoomSpeed = 500;

		if (scrollAmount == 0)
			return;

		Vector3 lastCameraPosition = Camera.main.transform.position;

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        float zoomDistance = zoomSpeed * scrollAmount * Time.deltaTime;
        
		Camera.main.transform.Translate(ray.direction * zoomDistance, Space.World);
		Vector3 cameraPosition = Camera.main.transform.position;

		if ( (cameraPosition.y < minHeight) || (cameraPosition.y > maxHeight) )
		{
			Camera.main.transform.position = lastCameraPosition;
		}
	}

	// Nicer zoom, but calls every frame
	/*void Update_Zoom2()
	{
		// Zoom to scrollwheel
		float scrollAmount = Input.GetAxis ("Mouse ScrollWheel");
		float minHeight = 2;
		float maxHeight = 20;

		// Move camera towards hitPos
        Vector3 hitPos = MouseToGroundPlane(Input.mousePosition);

		// Debug.Log(hitPos);

		Vector3 dir = hitPos - Camera.main.transform.position;

		Vector3 cameraPosition = Camera.main.transform.position;

        // Stop zooming out at a certain distance.
        if (scrollAmount > 0 || cameraPosition.y < maxHeight)
            cameraTargetOffset += dir * scrollAmount;

        Transform lastCameraTransform = Camera.main.transform;
        Camera.main.transform.position = Vector3.Lerp(
                                        Camera.main.transform.position,
                                        Camera.main.transform.position + cameraTargetOffset,
                                        Time.deltaTime * 5f
                                        );
        cameraTargetOffset -= Camera.main.transform.position - lastCameraTransform.position;

		// Change camera angle
		float minAngle = 50;
		float maxAngle = 75;

		Camera.main.transform.rotation = Quaternion.Euler (
			Mathf.Lerp (minAngle, maxAngle, Camera.main.transform.position.y / maxHeight),
			Camera.main.transform.rotation.eulerAngles.y,
			Camera.main.transform.rotation.eulerAngles.z
		);

		Ray downRay = Camera.main.ViewportPointToRay(new Vector3());
		Ray upRay = Camera.main.ViewportPointToRay(new Vector3(0, 1, 0));
		cameraPosition = Camera.main.transform.position;

		if ( !Physics.Raycast(downRay) || !Physics.Raycast(upRay) ||
			 (cameraPosition.y < minHeight) || (cameraPosition.y > maxHeight) )
		{
			Camera.main.transform.position = lastCameraTransform.position;
			Camera.main.transform.rotation = lastCameraTransform.rotation;
		}
	}*/

	Vector3 MouseToGroundPlane(Vector3 mousePos)
	{
		Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

		// What is the point at which the mouse ray intersects Y=0
		float rayLength = mouseRay.origin.y / mouseRay.direction.y;
		return mouseRay.origin - mouseRay.direction * rayLength;
	}

    void SelectUnit()
	{
		UnselectPreviousUnit();

		Hex hex = MouseToHex();

		if (hex != null)
			unit = hex.GetUnit();
		
		if (unit != null)
		{
			unit.IsSelected = true;
			pathfinding.ClearPath(path);
            ChangeSelectedHex(hex);
			path = unit.Path;

			if (path != null)
			{
				pathfinding.DrawPath(path, unit);
			}
		}
    }

	void UnselectPreviousUnit()
	{
		if (unit != null)
			unit.IsSelected = false;
	}

    void ChangeSelectedHex(Hex hex)
    {
        if (selectedHex != null)
            selectedHex.SetSelected(false);

        selectedHex = hex;
        hex.SetSelected(true);
    }

    Hex MouseToHex()
	{
		Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		return RayToHex(mouseRay);
	}

	Hex RayToHex(Ray ray)
	{
		RaycastHit hitInfo;

		if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, layerMask.value))
		{
			GameObject hexGameObject = hitInfo.transform.parent.gameObject;
			Hex hex = hexMap.GameObjectToHexDictionary[hexGameObject];

			return hex;
		}

		return null;
	}

	void MoveUnit()
	{
		Hex newSelectedHex = unit.Move(path);

		if (newSelectedHex == null)
            return;

        ChangeSelectedHex(newSelectedHex);
        path = unit.Path;

        if (path != null)
            pathfinding.DrawPath(path, unit);
	}
}
