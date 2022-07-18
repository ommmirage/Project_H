using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization;

// Unit is a base class for all the units

[DataContract]
[KnownType(typeof(Knight))]
public class Unit
{
    protected int hp;
    protected int strength;
    protected int moves;
    public int Moves { get { return moves; } }
    protected int navalMoves;
    public int NavalMoves { get { return navalMoves; } }
    [DataMember]
    public int MovesRemaining;
    [DataMember]
    public bool IsEmbarked;
    protected string type;
    public string Type { get { return type; } }
    [DataMember]
    public bool FinishedMove = false;
    public bool IsSelected = false;

    HexMap hexMap;

    protected Hex hex;

    public GameObject UnitGameObject;

    [DataMember]
    public LinkedList<PathHex> Path = null;
    PathHex[,] pathMap;

    public Unit(){}
    
    public Unit(HexMap hexMap)
    {
        // Debug.Log(hexMap);
        this.hexMap = hexMap;
        pathMap = new PathHex[hexMap.Width, hexMap.Height];
    }

    public Hex GetHex()
    {
        return hex;
    }

    public void SetHex(Hex newHex)
    {
        hex = newHex;
        hex.AddUnit(this);
    }

    public void SetHexMap(HexMap hexMap)
    {
        this.hexMap = hexMap;
    }

    public PathHex GetPathHex()
    {
        return pathMap[hex.Q, hex.R];
    }

    public PathHex GetPathHex(Hex hex)
    {
        return pathMap[hex.Q, hex.R];
    }

    public PathHex GetPathHexAt(int x, int y)
    {
        if ((y < 0) || (y >= hexMap.Height))
            return null;

        if (x < 0)
            x += hexMap.Width;
        else
            x = x % hexMap.Width;
        
        return pathMap[x, y];
    }

    public Hex Move(List<PathHex> pathList)
    { 
        Path = new LinkedList<PathHex>(pathList);
        return Move(Path);
    }

    // Returns hex, on which unit finished move
    public Hex Move(LinkedList<PathHex> pathList)
    {   
        if (pathList.Count == 0)
            return null;

        PathHex pathHex = new PathHex(hex);

        // First path node is set on the tile, on which
        // unit stands

        if (MovesRemaining > 0)
        {
            Path.RemoveFirst();

            hex.SetSelected(false);
            hex.Clear();
            hex.RemoveUnit();
        }

        while (Path.Count > 0)
        {
            if (MovesRemaining <= 0)
            {
                break;
            }

            pathHex = Path.First.Value;
            Path.RemoveFirst();
            hex = hexMap.GetHexAt(pathHex);
            MovesRemaining -= hex.MovementCost;

            if (pathHex.Embark)
                MovesRemaining = 0;

            hex.Clear();
        }

        if (MovesRemaining <= 0)
            FinishedMove = true;

        SetHex(hex);

        GameObject hexGameObject = hexMap.HexToGameObjectDictionary[hex];
        UnitGameObject.transform.position = hexGameObject.transform.position;
            
        Path.AddFirst(pathHex);
        return hex;
    }

    public void SetMovesRemaining()
    {
        if (hex.Elevation > 0)
        {
            MovesRemaining = moves;
        }
        else
        {
            MovesRemaining = navalMoves;
        }
    }

    public void PreparePathMap()
    {
        for (int x = 0; x < hexMap.Width; x++)
        {
            for (int y = 0; y < hexMap.Height; y++)
            {
                pathMap[x, y] = new PathHex(hexMap.GetHexAt(x, y));
            }
            
            for (int y = 0; y < hexMap.SnowWidth; y++)
            {
                pathMap[x, y].IsWalkable = false;
            }
            
            for (int y = hexMap.Height - hexMap.SnowWidth; y < hexMap.Height; y++)
            {
                pathMap[x, y].IsWalkable = false;
            }
        }
    }

    public void DoTurn()
    {
        SetMovesRemaining();
        FinishedMove = false;

        if ((Path == null) || (Path.Count == 0) )
            return;

        Hex endHex = hexMap.GetHexAt(Path.Last.Value);

        if (endHex != hex)
        {   
            Pathfinding pathfinding = new Pathfinding();

            // LinkedList<PathHex> newPath = pathfinding.FindPath(this, hex, endHex);

            // Debug.Log(newPath.Last.Value.GCost);

            // if (newPath.Last.Value.GCost)

            if (IsSelected)
            {
                Path = pathfinding.RedrawPath(Path, this, endHex);
            }
            else
            {
                Path = pathfinding.FindPath(this, hex, endHex);            
            }

            Move(Path);
        }
    }
}
