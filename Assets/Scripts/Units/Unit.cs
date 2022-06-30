using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Unit
{
    protected int hp;
    protected int strength;
    protected int moves;
    public int Moves { get { return moves; } }
    protected int navalMoves;
    public int NavalMoves { get { return navalMoves; } }
    public int MovesRemaining;
    // public int NavalMovesRemaining;
    public bool IsEmbarked;
    protected string type;
    public string Type { get { return type; } }
    public bool FinishedMove = false;
    
    [System.NonSerialized]
    HexMap hexMap;

    protected Hex hex;

    [System.NonSerialized]
    public GameObject UnitGameObject;

    public LinkedList<PathHex> Path = null;
    public PathHex[,] PathMap;

    public Unit(HexMap hexMap)
    {
        this.hexMap = hexMap;
        PathMap = new PathHex[hexMap.Width, hexMap.Height];
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

    public PathHex GetPathHex(Hex hex)
    {
        return PathMap[hex.Q, hex.R];
    }

    public PathHex GetPathHexAt(int x, int y)
    {
        if ((y < 0) || (y >= hexMap.Height))
            return null;

        if (x < 0)
            x += hexMap.Width;
        else
            x = x % hexMap.Width;
        
        return PathMap[x, y];
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
        PathHex pathHex = new PathHex(hex);
        Path = new LinkedList<PathHex>(pathList);

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
                FinishedMove = true;
                break;
            }

            pathHex = Path.First.Value;
            Path.RemoveFirst();
            MovesRemaining -= hex.MovementCost;

            if (pathHex.Embark)
                MovesRemaining = 0;

            hex = hexMap.GetHexAt(pathHex);
            hex.Clear();
        }

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
}
