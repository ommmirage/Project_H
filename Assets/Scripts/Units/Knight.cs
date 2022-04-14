using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Unit
{
    void Start()
    {
        name = "Knight";
        hp = 100;
        strength = 8;
        movement = 2;
        movementRemaining = movement;
    }
}
