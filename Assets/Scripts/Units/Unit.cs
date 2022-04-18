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

    Hex hex;
    GameObject unitGameObject;

    public delegate void UnitMovedDelegate(Hex oldHex, Hex newHex);
    public event UnitMovedDelegate UnitMoved;

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

    public void SetUnitGameObject(GameObject unitGameObject)
    {
        this.unitGameObject = unitGameObject;
    }

    public void DoTurn()
    {
        Hex oldHex = hex;
        Hex newHex = hexMap.GetHexAt(hex.Q + 1, hex.R);

        SetHex(newHex);
    }
}
