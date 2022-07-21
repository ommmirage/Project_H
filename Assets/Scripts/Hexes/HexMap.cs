using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// HexMap scripts is attached to the main game object 
// on the scene. It generates or loads Hexes, has dictionaries
// to link Hexes and their GameObjects. HexMap holds the
// information about all the hexes and units on the map.

// There will be options on the type of the map you want to generate.
// The only one option for now is "Continents".
// HexMapContinents is HexMap's derived class for this purpose.

// You should attach hex and forest prefabs and 
// ocean and grasslands materials to HexMap game object.

public class HexMap : MonoBehaviour
{
    // You can setup width and height of the map.
    int width = 85;
    public int Width { get { return width; } }
    int height = 50;
    public int Height { get { return height; } }
    int snowWidthUp = 8;
    public int SnowWidthUp { get { return snowWidthUp; } }
    int snowWidthDown = 2;
    public int SnowWidthDown { get { return snowWidthDown; } }

    // Each piece of land is a territory of approximately
    // territorySize hexes.
    protected int territorySize = 40;

    Hex[,] hexes;
    public Hex[,] Hexes { get { return hexes; } }
    List<Unit> units = new List<Unit>();
    public List<Unit> Units { get { return units; } }
    public Dictionary<Hex, GameObject> HexToGameObjectDictionary = new Dictionary<Hex, GameObject>();
    public Dictionary<GameObject, Hex> GameObjectToHexDictionary = new Dictionary<GameObject, Hex>();
    
    [SerializeField] GameObject hexPrefab;
    [SerializeField] GameObject forestPrefab;
    [SerializeField] Material matOcean;
    // [SerializeField] Material matGrasslands;
    [SerializeField] Material matSnow;

    UnitsPrefabs unitsPrefabs;

    public void Start()
    {
        height += (snowWidthUp + snowWidthDown);
        unitsPrefabs = Object.FindObjectOfType<UnitsPrefabs>();
    }

    public void LoadMap(GameData gameData, Hex[,] hexes)
    {
        LoadTiles(hexes);

        UpdateHexVisuals();
        DrawSnow();
        SetLabels();
        DrawBorders();
        SpawnUnits(gameData.Units);
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

    // Hex coordinate labels for debug purposes.
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

    // Update real-world hex positions when camera moves.
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
            for (int y = snowWidthDown; y < height - snowWidthUp; y++)
            {
                Hex hex = hexes[x, y];

                GameObject hexGameObject = HexToGameObjectDictionary[hex];
                MeshRenderer meshRenderer = hexGameObject.GetComponentInChildren<MeshRenderer>();
                // MeshFilter meshFilter = hexGameObject.GetComponentInChildren<MeshFilter>();

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
    }

    public Hex GetHexAt(PathHex pathHex)
    {
        return GetHexAt(pathHex.Q, pathHex.R);
    }

    public Hex GetHexAt(int x, int y)
    {
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

    void SpawnUnitAt(Unit unit)
    {
        Hex hex = unit.GetHex();
        SpawnUnitAt(unit, hex.Q, hex.R, false);
    }

    void SpawnUnitAt(Unit unit, int x, int y, bool setMovesRemaining = true)
    {
        Hex hex = GetHexAt(x, y);
        unit.SetHex(hex);

        if (setMovesRemaining)
            unit.SetMovesRemaining();

        GameObject hexGameObject = HexToGameObjectDictionary[hex];

        GameObject unitGameObject = Instantiate(
                unitsPrefabs.GetPrefab(unit), 
                hexGameObject.transform.position, 
                new Quaternion(), 
                hexGameObject.transform
                );
        
        units.Add(unit);
        unit.UnitGameObject = unitGameObject;
    }

    public void SpawnUnits()
    {
        SpawnUnitAt(new Knight(this), 84, 0);
        SpawnUnitAt(new Knight(this), 84, 15);
        SpawnUnitAt(new Knight(this), 3, 12);
        SpawnUnitAt(new Knight(this), 8, 9);
    }

    public void SpawnUnits(List<Unit> units)
    {
        foreach (Unit unit in units)
        {
            SpawnUnitAt(unit);
        }
    }

    public void DoTurn()
    {
        if (units != null)
        {
            foreach (Unit unit in units)
            {
                unit.DoTurn();
            }
        }
    }

    public void ClearMap()
    {
        foreach (Transform hexPrefab in transform) 
        {
            GameObject.Destroy(hexPrefab.gameObject);
        }
        HexToGameObjectDictionary = new Dictionary<Hex, GameObject>();
        GameObjectToHexDictionary = new Dictionary<GameObject, Hex>();
        units = new List<Unit>();
    }

    protected void DrawSnow()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < snowWidthDown; y++)
                ProceedSnowHex(x, y);
    
            for (int y = height - snowWidthUp; y < height; y++)
                ProceedSnowHex(x, y);
        }
    }

    void ProceedSnowHex(int x, int y)
    {
        Hex hex = hexes[x, y];

        GameObject hexGameObject = HexToGameObjectDictionary[hex];
        MeshRenderer meshRenderer = hexGameObject.GetComponentInChildren<MeshRenderer>();

        meshRenderer.material = matSnow;
    }
}
