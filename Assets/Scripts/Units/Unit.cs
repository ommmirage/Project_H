using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    
    HexMap hexMap;

    public delegate void UnitMovedDelegate(Hex oldHex, Hex newHex);
    public event UnitMovedDelegate UnitMoved;

    protected Hex hex;
    public GameObject UnitGameObject;
    public LinkedList<PathHex> Path = null;
    public PathHex[,] PathMap;

    public Unit()
    {
        hexMap = Object.FindObjectOfType<HexMap>();
        PathMap = new PathHex[hexMap.Width, hexMap.Height];
    }

    public Hex GetHex()
    {
        return hex;
    }

    public void SetHex(Hex newHex)
    {
        Hex oldHex = hex;

        hex = newHex;

        hex.AddUnit(this);

        // Объявляем всем зарегистрированным в событии делегатам,
        // что оно произошло. В данном случае, у нас 
        // зарегистрирована функция UnitView.OnUnitMoved().
        // Она выполнится в HexMap OnUnitMoved().
        if (UnitMoved != null)
        {
            UnitMoved(oldHex, newHex);
        }
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
            SetHex(hex);
            hex.Clear();
        }

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
