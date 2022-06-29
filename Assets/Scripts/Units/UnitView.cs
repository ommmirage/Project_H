using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitView : MonoBehaviour
{ 
    public void OnUnitMoved(Hex oldHex, Hex newHex)
    {
        this.transform.position = newHex.PositionFromCamera();
    }
}
