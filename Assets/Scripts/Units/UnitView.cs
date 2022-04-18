using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitView : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    public void OnUnitMoved(Hex oldHex, Hex newHex)
    {
        this.transform.position = newHex.PositionFromCamera();
    }
}
