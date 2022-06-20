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
    public bool IsOnPath = false;
    
    HexMap hexMap;

    public delegate void UnitMovedDelegate(Hex oldHex, Hex newHex);
    public event UnitMovedDelegate UnitMoved;

    Hex hex;
    public GameObject UnitGameObject;
    Queue<Hex> path;

    public Unit()
    {
        hexMap = Object.FindObjectOfType<HexMap>();
        MovesRemaining = moves;
    }

    public void SetHex(Hex newHex)
    {
        Hex oldHex = hex;

        hex = newHex;

        hex.AddUnit(this);

        // Объявляем всем зарегистрированным в событии делегатам,
        // что оно произошло. В данном случае, у нас 
        // зарегистрирована функция UnitView.OnUnitMoved().
        // Выполняем её.
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
    // }

    public void Move(List<Hex> path)
    {   
        path[0].SetSelected(false);

        foreach (Hex hex in path)
        {
            SetHex(hex);
            GameObject hexGameObject = hexMap.HexToGameObjectDictionary[hex].gameObject;
            Transform circleAroundTurn1 = hexGameObject.transform.GetChild(6).transform.GetChild(0);
            if (circleAroundTurn1.gameObject.activeSelf)
            {
                FinishedMove = true;
                hex.Clear();
                hex.SetSelected(true);
                return;
            }
            hex.Clear();
        }
    }
}
