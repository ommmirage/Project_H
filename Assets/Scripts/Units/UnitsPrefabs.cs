using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitsPrefabs : MonoBehaviour
{
    [SerializeField] GameObject knightPrefab;
    GameObject Prefab {get { return knightPrefab; } }

    void Start()
    {
        
    }
}
