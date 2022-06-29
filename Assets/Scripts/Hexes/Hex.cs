using System;
using UnityEngine;


// The hex class defines the grid position, world position, size,
// neighbors of a Hex Tile. 

[Serializable]
public class Hex
{
    // readonly means that variable is only set in the contructor
    public readonly int Q;
    public readonly int R;
    // public readonly int S;

    float radius = 0.5f;

    public float Elevation = -0.5f;

    public int Continent = -1;
    public int Territory = -1;

    public int MovementCost = 1;

    public bool IsForest = false;

    [NonSerialized]
    HexMap hexMap;
    [NonSerialized]
    Unit unit;

    // static means that const belongs to the type, not the object
    static readonly float WIDTH_MULTIPLIER = Mathf.Sqrt(3) / 2;

    public Hex(HexMap hexMap, int q, int r) {
        this.hexMap = hexMap;

        this.Q = q;
        this.R = r;
        // S = -(q + r);
    }

    public void SetHexMap(HexMap hexMap)
    {
        this.hexMap = hexMap;
    }

    // Returns the world-space position of this hex
    public Vector3 Position()
    {
        float horizontalSpacing = Width();
        float verticalSpacing = Height() * 0.75f;

        return new Vector3(
            horizontalSpacing * (Q + R / 2f),
            0,
            verticalSpacing * R
        );
    }

    public float Height()
    {
        return radius * 2;
    }

    public float Width()
    {
        return WIDTH_MULTIPLIER * Height();
    }

    public float VerticalSpacing()
    {
        return Height() * 0.75f;
    }

    public float HorizontalSpacing()
    {
        return Width();
    }

    public Vector3 PositionFromCamera()
    {
        Vector3 position = Position();

        float mapWidth = hexMap.Width * HorizontalSpacing();

        float cameraPosX = Camera.main.transform.position.x;

        float mapWidthsFromCenter = (position.x - cameraPosX) / mapWidth;

        // We want widthAmountFromCamera to be in [-0,5; 0,5]
        while (mapWidthsFromCenter > 0.5f)
        {
            mapWidthsFromCenter -= 1f;
            position.x = mapWidthsFromCenter * mapWidth + cameraPosX;
        }
        while (mapWidthsFromCenter < -0.5f)
        {
            mapWidthsFromCenter += 1f;
            position.x = mapWidthsFromCenter * mapWidth  + cameraPosX;
        }

        return position;
    }

    public Unit GetUnit()
    {
        return unit;
    }

    public void AddUnit(Unit unit)
    {
        this.unit = unit;
    }

    public void RemoveUnit()
    {
        unit = null;
    }

    public void SetSelected(bool isSelected)
    {
		GameObject HexGameObject = hexMap.HexToGameObjectDictionary[this];
        HexGameObject.transform.GetChild(3).gameObject.SetActive(isSelected);
    }

    public override string ToString()
    {
        return Q + ", "+ R;
    }

    public void Clear()
    {
        GameObject hexGameObject = hexMap.HexToGameObjectDictionary[this].gameObject;
        Transform longLines = hexGameObject.transform.GetChild(4);
        foreach (Transform road in longLines)
        {
            road.gameObject.SetActive(false);
        }

        Transform shortLines = hexGameObject.transform.GetChild(5);
        foreach (Transform road in shortLines)
        {
            road.gameObject.SetActive(false);
        }

        GameObject unitMovesGameObject = hexGameObject.transform.GetChild(6).gameObject;
        unitMovesGameObject.GetComponentInChildren<TextMesh>().text = "";
        unitMovesGameObject.transform.GetChild(0).gameObject.SetActive(false);
    }
}
