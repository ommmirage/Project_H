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
    public int NavalMovesRemaining;
    public bool IsEmbarked;
    protected string type;
    public string Type { get { return type; } }
    public bool FinishedMove = false;
    // public bool HasPathDrawn = false;
    
    HexMap hexMap;

    public delegate void UnitMovedDelegate(Hex oldHex, Hex newHex);
    public event UnitMovedDelegate UnitMoved;

    private Hex hex;
    public GameObject UnitGameObject;
    Queue<Hex> path;

    public Unit()
    {
        hexMap = Object.FindObjectOfType<HexMap>();
        MovesRemaining = moves;
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

    public void Move(List<Hex> pathList)
    {   
        Queue<Hex> path = new Queue<Hex>(pathList);

        if (MovesRemaining > 0)
        {
            path.Dequeue();

            hex.SetSelected(false);
            hex.Clear();
        }

        while (path.Count > 0)
        {
            if (MovesRemaining <= 0)
            {
                Debug.Log("Finished Move");
                FinishedMove = true;
                break;
            }

            hex = path.Dequeue();
            MovesRemaining -= hex.MovementCost;
            // Debug.Log(MovesRemaining);

            if (hex.Embark)
                MovesRemaining = 0;

            SetHex(hex);
            hex.Clear();
        }

        hex.SetSelected(true);
    }
}
