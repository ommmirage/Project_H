using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMap : MonoBehaviour
{
    [SerializeField] GameObject hexPrefab;

    int width = 85;
    public int Width { get { return width; } }

    int height = 50;
    public int Height { get { return height; } }

    [SerializeField] Material matOcean;
    // [SerializeField] Material matPlains;
    [SerializeField] Material matGrasslands;
    // [SerializeField] Material matMountains;
    [SerializeField] GameObject forestPrefab;

    [SerializeField] GameObject unitKnightPrefab;

    Hex[,] hexes;
    HashSet<Unit> units;
    Dictionary<Hex, Unit> hexUnitDictionary;

    // float MountainHeight = 1.3f;
    // float HillHeight = 0.75f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (units != null)
            {
                foreach (Unit unit in units)
                {
                    unit.DoTurn();
                }
            }
        }
    }

    protected void GenerateTiles()
    {
        hexes = new Hex[width, height];
        units = new HashSet<Unit>();
        hexUnitDictionary = new Dictionary<Hex, Unit>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Hex hex = new Hex(this, x, y);

                hexes[x, y] = hex;

                Vector3 inworldPos = hex.PositionFromCamera();

                GameObject hexObject = Instantiate(
                    hexPrefab, 
                    inworldPos,
                    new Quaternion(),
                    transform
                );

                hex.HexGameObject = hexObject;
            }
        }
    }

    protected void SetLabels()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject hexGameObject = hexes[x, y].HexGameObject;
                hexGameObject.GetComponentInChildren<TextMesh>().text = x + ", " + y;                
            }
        }
    }

    public void UpdateHexPositions()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Hex hex = hexes[x, y];
                Vector3 newPosition = hex.PositionFromCamera();
                hex.HexGameObject.transform.position = newPosition;
            }
        }
    }

    public void UpdateHexVisuals()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Hex hex = hexes[x, y];

                MeshRenderer meshRenderer = hex.HexGameObject.GetComponentInChildren<MeshRenderer>();
                MeshFilter meshFilter = hex.HexGameObject.GetComponentInChildren<MeshFilter>();

                if (hex.Elevation < 0)
                {
                    meshRenderer.material = matOcean;
                }

                if (hex.IsForest)
                {
                    GameObject.Instantiate(
                        forestPrefab, 
                        hex.HexGameObject.gameObject.transform.position,
                        new Quaternion(),
                        hex.HexGameObject.gameObject.transform
                        );
                }
            }
        }

        Unit knight = new Knight();
        SpawnUnitAt(knight, unitKnightPrefab, 0, 0);
    }

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
            Transform upRightBorder = hex.HexGameObject.transform.GetChild(2).GetChild(0);
            upRightBorder.gameObject.SetActive(true);
        }
        
        Hex right = GetHexAt(q + 1, r);
        if ((right != null) && (right.Territory != territory))
        {
            Transform rightBorder = hex.HexGameObject.transform.GetChild(2).GetChild(1);
            rightBorder.gameObject.SetActive(true);
        }

        Hex downRight = GetHexAt(q + 1, r - 1);
        if ((downRight != null) && (downRight.Territory != territory))
        {
            Transform downRightBorder = hex.HexGameObject.transform.GetChild(2).GetChild(2);
            downRightBorder.gameObject.SetActive(true);
        }
    }

    protected void SpawnUnitAt(Unit unit, GameObject prefab, int x, int y)
    {
        Hex hex = GetHexAt(x, y);
        unit.SetHex(hex);

        GameObject unitGameObject = Instantiate(
                prefab, 
                hex.HexGameObject.transform.position, 
                new Quaternion(), 
                hex.HexGameObject.transform
                );

        // Регистрируем функцию UnitView.OnUnitMoved() в событие Unit.UnitMoved 
        unit.UnitMoved += unitGameObject.GetComponent<UnitView>().OnUnitMoved;
        
        units.Add(unit);
        unit.SetUnitGameObject(unitGameObject);
    }
}
