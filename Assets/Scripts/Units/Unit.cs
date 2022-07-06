using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.Runtime.Serialization;

[DataContract]
[KnownType(typeof(Knight))]
public class Unit
{
    [DataMember]
    protected int hp;
    [DataMember]
    protected int strength;
    [DataMember]
    protected int moves;
    public int Moves { get { return moves; } }
    [DataMember]
    protected int navalMoves;
    public int NavalMoves { get { return navalMoves; } }
    [DataMember]
    public int MovesRemaining;
    [DataMember]
    public bool IsEmbarked;
    [DataMember]
    protected string type;
    public string Type { get { return type; } }
    [DataMember]
    public bool FinishedMove = false;
    
    HexMap hexMap;

    protected Hex hex;

    public GameObject UnitGameObject;

    public LinkedList<PathHex> Path = null;
    PathHex[,] pathMap;

    public Unit(){}
    
    public Unit(HexMap hexMap)
    {
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

    public List<PathHex> GetPath()
    {
        if (Path != null)
        {
            if (Path.Count > 0)
            {
                return new List<PathHex>(Path);
            }
        }
            
        return null;
    }


    // public void DoTurn()
    // {
    //     SetMovesRemaining();
    //     if ( path == null || path.Count == 0 )
    //     {
    //         return;
    //     }

    //     while (MovesRemaining > 0)
    //     {
    //         Hex newHex = path.Dequeue();
    //         SetHex(newHex);
    //     }

            // if (movesRemaining <= 0)
            //     {
            //         if (hex.Elevation > 0)
            //         {
            //             movesRemaining = unit.Moves;
            //         }
            //         else
            //         {
            //             movesRemaining = unit.NavalMoves;
            //         }
            //     }
            // FinishedMove = false;
    // }

    // Returns hex, on which unit finished move
    public Hex Move(List<PathHex> pathList)
    {   
        if (pathList.Count == 0)
            return null;

        PathHex pathHex = new PathHex(hex);
        Path = new LinkedList<PathHex>(pathList);

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
                break;

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
        for (int i = 0; i < hexMap.Width; i++)
        {
            for (int j = 0; j < hexMap.Height; j++)
            {
                pathMap[i, j] = new PathHex(hexMap.GetHexAt(i, j));
            }
        }
    }
}
