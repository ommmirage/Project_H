using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitsPrefabs : MonoBehaviour
{
    [SerializeField] GameObject knightPrefab;

    Dictionary<System.Type, GameObject> typeToPrefabDictionary;

    void Start()
    {
        typeToPrefabDictionary = new Dictionary<System.Type, GameObject>();

        typeToPrefabDictionary.Add(typeof(Knight), knightPrefab);
    }

    public GameObject GetPrefab(Unit unit)
    {
        return typeToPrefabDictionary[unit.GetType()];
    }
}
