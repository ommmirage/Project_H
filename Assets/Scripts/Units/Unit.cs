using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit
{
    protected string name;
    protected int hp;
    protected int strength;
    protected int movement;
    protected int movementRemaining;
    
    Hex hex;
    GameObject unitGameObject;

    public void SetHex(Hex hex)
    {
        this.hex = hex;
    }

    public void SetUnitGameObject(GameObject unitGameObject)
    {
        this.unitGameObject = unitGameObject;
    }

    public void DoTurn()
    {

    }
}
