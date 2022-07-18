using System;
using UnityEngine;
using System.Runtime.Serialization;

// The Hex class contains hexes' coordinates and can
// calculate the world-space position of a hex on a screen.

[DataContract]
public class Hex
{
    [DataMember]
    int q;
    public int Q { get { return q; } }

    [DataMember]
    int r;
    public int R { get { return r; } }

    [DataMember]
    public float Elevation = -0.5f;

    [DataMember]
    public int Continent = -1;
    [DataMember]
    public int Territory = -1;

    [DataMember]
    public int MovementCost = 1;

    [DataMember]
    public bool IsForest = false;

    public bool IsWalkable = true;

    HexMap hexMap;

    [DataMember]
    Unit unit;

    // static means that const belongs to the type, not the object
    static readonly float WIDTH_MULTIPLIER = Mathf.Sqrt(3) / 2;
    static readonly float RADIUS = 0.5f;

    public Hex(){}

    public Hex(HexMap hexMap, int q, int r) 
    {
        this.hexMap = hexMap;
        this.q = q;
        this.r = r;
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
        return RADIUS * 2;
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
