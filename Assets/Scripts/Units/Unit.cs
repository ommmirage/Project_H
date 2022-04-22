using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit
{
    protected int hp;
    protected int strength;
    protected int movement;
    protected int movementRemaining;
    
    HexMap hexMap;

    public delegate void UnitMovedDelegate(Hex oldHex, Hex newHex);
    public event UnitMovedDelegate UnitMoved;

    Hex hex;
    public GameObject UnitGameObject;
    Queue<Hex> path;

    public Unit()
    {
        hexMap = Object.FindObjectOfType<HexMap>();
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

    public void DoTurn()
    {
        if ( path == null || path.Count == 0 )
        {
            return;
        }

        while (movementRemaining > 0)
        {
            Hex newHex = path.Dequeue();
            SetHex(newHex);
        }
    }

    // public float TurnsToEnterHex(Hex hex, float turnsToDate)
    // {
    //     float turnsRemaining = movementRemaining / movement;
    // }
}
