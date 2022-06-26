using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TestHex
{
    public int id;
    public TestHex camefromHex;

    public TestHex(int id, TestHex camefromHex)
    {
        this.id = id;
        this.camefromHex = camefromHex;
    }
}
