using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMap : MonoBehaviour
{
    [SerializeField] GameObject hexPrefab;

    [SerializeField] int width = 110;
    public int Width { get { return width; } }

    [SerializeField] int height = 65;
    public int Height { get { return height; } }

    [SerializeField] Material matOcean;
    [SerializeField] Material matPlains;
    [SerializeField] Material matGrasslands;
    [SerializeField] Material matMountains;

    // [SerializeField] Mesh meshWater;

    GameObject[,] hexObjects;

    Hex[,] hexes;

    float MountainHeight = 1.3f;
    float HillHeight = 0.75f;

    public Hex GetHexAt(int x, int y)
    {
        if (hexes == null)
        {
            Debug.LogError("Hexes array is not yet instantiated.");
            return null;
        }

        if ((y < 0) || (y >= height))
            return null;

        if (x < 0)
            x += width;
        else
            x = x % width;
        
        return hexes[x, y];
    }

    protected void GenerateOcean()
    {
        hexObjects = new GameObject[width, height];
        hexes = new Hex[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Hex hex = new Hex(x, y);

                hexes[x, y] = hex;

                Vector3 inworldPos = hex.PositionFromCamera(width, height);

                GameObject hexObject = Instantiate(
                    hexPrefab, 
                    inworldPos,
                    new Quaternion(),
                    transform
                );

                hexObject.GetComponentInChildren<TextMesh>().text = x + ", " + y;

                hexObjects[x, y] = hexObject;
            }
        }
    }

    public void UpdateHexPositions()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 newPosition = hexes[x, y].PositionFromCamera(width, height);
                hexObjects[x, y].transform.position = newPosition;
            }
        }
    }

    public void UpdateHexVisuals()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject hexObject = hexObjects[x, y];
                Hex hex = hexes[x, y];

                MeshRenderer meshRenderer = hexObject.GetComponentInChildren<MeshRenderer>();

                if (hex.Elevation > MountainHeight)
                {
                    meshRenderer.material = matMountains;
                }
                else if (hex.Elevation > HillHeight)
                {
                    meshRenderer.material = matPlains;
                }
                else if (hex.Elevation > 0)
                {
                    meshRenderer.material = matGrasslands;
                }
                else
                {
                    meshRenderer.material = matOcean;
                }

                // MeshFilter meshFilter = hexObject.GetComponentInChildren<MeshFilter>();
                // meshFilter.mesh = meshWater;
            }
        }
    }

    public Hex[] GetHexesWithinRangeOf(Hex centralHex, int range)
    {
        List<Hex> results = new List<Hex>();

        for (int dx = -range; dx <= range; dx++)
        {
            for(int dy = Mathf.Max(-range, -dx-range); dy <= Mathf.Min(range, -dx+range); dy++)
            {
                results.Add(GetHexAt(centralHex.Q + dx, centralHex.R + dy));
            }
        }
        
        return results.ToArray();
    }

    public float Distance(Hex a, Hex b)
    {
        int dq = Mathf.Abs(a.Q - b.Q);
        if (dq > width / 2)
        {
            dq = width - dq;
        }

        int ds = Mathf.Abs(a.S - b.S);
        if (ds > width / 2)
        {
            ds = Mathf.Abs(width - ds);
        }

        return (dq + Mathf.Abs(a.R - b.R) + ds) / 2;
    }
}
