using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMap : MonoBehaviour
{
    [SerializeField] GameObject hexPrefab;

    [SerializeField] int width = 85;
    public int Width { get { return width; } }

    [SerializeField] int height = 50;
    public int Height { get { return height; } }

    [SerializeField] Material matOcean;
    // [SerializeField] Material matPlains;
    [SerializeField] Material matGrasslands;
    // [SerializeField] Material matMountains;
    [SerializeField] GameObject ForestPrefab;

    GameObject[,] hexObjects;

    Hex[,] hexes;

    // float MountainHeight = 1.3f;
    // float HillHeight = 0.75f;

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

    protected void GenerateTiles()
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

                hexObjects[x, y] = hexObject;
            }
        }
    }

    protected void SetLabels()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                hexObjects[x, y].GetComponentInChildren<TextMesh>().text = x + ", " + y;
                if (GetHexAt(x, y).Territory != -1)
                {
                    hexObjects[x, y].GetComponentInChildren<TextMesh>().text = 
                        GetHexAt(x, y).Territory.ToString();
                }
                
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

                // if (hex.Elevation > MountainHeight)
                // {
                //     meshRenderer.material = matMountains;
                // }
                // else if (hex.Elevation > HillHeight)
                // {
                //     meshRenderer.material = matPlains;
                // }
                // else 
                if (hex.Elevation > 0)
                {
                    meshRenderer.material = matGrasslands;
                }
                else
                {
                    meshRenderer.material = matOcean;
                }

                if (hex.IsForest)
                {
                    GameObject.Instantiate(
                        ForestPrefab, 
                        hexObject.gameObject.transform.position,
                        new Quaternion(),
                        hexObject.gameObject.transform
                        );
                }
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

    protected void DrawBorders()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                CheckRightBorders(GetHexAt(x, y));
            }
        }
    }

    void CheckRightBorders(Hex hex)
    {
        int territory = hex.Territory;
        int q = hex.Q;
        int r = hex.R;

        Hex upRightHex = GetHexAt(q, r + 1);
        if ((upRightHex != null) && (upRightHex.Territory != territory))
        {
            Transform upRightBorder = hexObjects[q, r].transform.GetChild(2).GetChild(0);
            upRightBorder.gameObject.SetActive(true);
        }
        
        Hex right = GetHexAt(q + 1, r);
        if ((right != null) && (right.Territory != territory))
        {
            Transform rightBorder = hexObjects[q, r].transform.GetChild(2).GetChild(1);
            rightBorder.gameObject.SetActive(true);
        }

        Hex downRight = GetHexAt(q + 1, r - 1);
        if ((downRight != null) && (downRight.Territory != territory))
        {
            Transform downRightBorder = hexObjects[q, r].transform.GetChild(2).GetChild(2);
            downRightBorder.gameObject.SetActive(true);
        }
    }
}
