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
    [SerializeField] Material matGrasslands;
    [SerializeField] GameObject forestPrefab;

    [SerializeField] GameObject unitKnightPrefab;

    Hex[,] hexes;
    public Hex[,] Hexes { get { return hexes; } }
    HashSet<Unit> units = new HashSet<Unit>();
    public Dictionary<Hex, GameObject> HexToGameObjectDictionary = new Dictionary<Hex, GameObject>();
    public Dictionary<GameObject, Hex> GameObjectToHexDictionary = new Dictionary<GameObject, Hex>();

    public void LoadMap(Hex[,] hexes)
    {
        LoadTiles(hexes);

        UpdateHexVisuals();
        SetLabels();
        DrawBorders();
    }

    void LoadTiles(Hex[,] hexes)
    {
        this.hexes = hexes;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GenerateTile(hexes[x, y], x, y);
            }
        }
    }

    protected void GenerateTiles()
    {
        hexes = new Hex[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Hex hex = new Hex(this, x, y);
                GenerateTile(hex, x, y);
            }
        }
    }

    void GenerateTile(Hex hex, int x, int y)
    {
        hexes[x, y] = hex;

        Vector3 inworldPos = hex.PositionFromCamera();

        GameObject hexGameObject = Instantiate(
            hexPrefab,
            inworldPos,
            new Quaternion(),
            transform
        );

        HexToGameObjectDictionary.Add(hex, hexGameObject);
        GameObjectToHexDictionary.Add(hexGameObject, hex);
    }

    protected void SetLabels()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject hexGameObject = HexToGameObjectDictionary[hexes[x, y]];
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
                GameObject hexGameObject = HexToGameObjectDictionary[hex];
                hexGameObject.transform.position = newPosition;
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

                GameObject hexGameObject = HexToGameObjectDictionary[hex];
                MeshRenderer meshRenderer = hexGameObject.GetComponentInChildren<MeshRenderer>();
                MeshFilter meshFilter = hexGameObject.GetComponentInChildren<MeshFilter>();

                if (hex.Elevation < 0)
                {
                    meshRenderer.material = matOcean;
                }

                if (hex.IsForest)
                {
                    GameObject.Instantiate(
                        forestPrefab, 
                        hexGameObject.gameObject.transform.position,
                        new Quaternion(),
                        hexGameObject.gameObject.transform
                        );
                }
            }
        }

        SpawnUnitAt(new Knight(this), unitKnightPrefab, 84, 0);
        SpawnUnitAt(new Knight(this), unitKnightPrefab, 84, 15);
        SpawnUnitAt(new Knight(this), unitKnightPrefab, 3, 12);
        SpawnUnitAt(new Knight(this), unitKnightPrefab, 8, 9);
    }

    public Hex GetHexAt(PathHex pathHex)
    {
        return GetHexAt(pathHex.Q, pathHex.R);
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

        GameObject hexGameObject = HexToGameObjectDictionary[hex];

        Hex upRightHex = GetHexAt(q, r + 1);
        if ((upRightHex != null) && (upRightHex.Territory != territory))
        {
            Transform upRightBorder = hexGameObject.transform.GetChild(2).GetChild(0);
            upRightBorder.gameObject.SetActive(true);
        }
        
        Hex right = GetHexAt(q + 1, r);
        if ((right != null) && (right.Territory != territory))
        {
            Transform rightBorder = hexGameObject.transform.GetChild(2).GetChild(1);
            rightBorder.gameObject.SetActive(true);
        }

        Hex downRight = GetHexAt(q + 1, r - 1);
        if ((downRight != null) && (downRight.Territory != territory))
        {
            Transform downRightBorder = hexGameObject.transform.GetChild(2).GetChild(2);
            downRightBorder.gameObject.SetActive(true);
        }
    }

    protected void SpawnUnitAt(Unit unit, GameObject prefab, int x, int y)
    {
        Hex hex = GetHexAt(x, y);
        unit.SetHex(hex);
        unit.SetMovesRemaining();

        GameObject hexGameObject = HexToGameObjectDictionary[hex];

        GameObject unitGameObject = Instantiate(
                prefab, 
                hexGameObject.transform.position, 
                new Quaternion(), 
                hexGameObject.transform
                );

        // Регистрируем функцию UnitView.OnUnitMoved() в событие Unit.UnitMoved 
        unit.UnitMoved += unitGameObject.GetComponent<UnitView>().OnUnitMoved;
        
        units.Add(unit);
        unit.UnitGameObject = unitGameObject;
    }

    public void DoTurn()
    {
        // if (units != null)
        // {
        //     foreach (Unit unit in units)
        //     {
        //         unit.DoTurn();
        //     }
        // }
    }
}
