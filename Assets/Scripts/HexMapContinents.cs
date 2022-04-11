using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMapContinents : HexMap
{
    [Range(1, 3)]
    [SerializeField] int minContinents = 2;
    [Range(1, 3)]
    [SerializeField] int maxContinents = 2;

    int numContinents;

    void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        GenerateOcean();
        GenerateContinents();
        // ElevateArea(GetHexAt(0, 20), 5, 0);

        UpdateHexVisuals();
    }

    void GenerateContinents()
    {
        numContinents = Random.Range(minContinents, maxContinents);

        for (int i = 0; i < numContinents; i++)
        {
            GenerateContinent(i);
        }
    }

    void GenerateContinent(int continentNumber)
    {
        CentralHex continentCenter = GenerateContinentCenterHex(continentNumber);

        int numSplats = Random.Range(4, 15 - 3 * numContinents);

        while (numSplats > 0)
        {
            if (GenerateTerrainAroundContinentCenter(continentCenter, continentNumber))
            {
                numSplats--;
            }
        }
    }

    CentralHex GenerateContinentCenterHex(int continentNumber)
    {
        Hex hex = GetHexAt(Width / numContinents * continentNumber, Height / 2);

        CentralHex centralHex = new CentralHex(hex, 20);
        centralHex.ContinentNumber = continentNumber;
        ElevateArea(centralHex, continentNumber);

        return centralHex;
    }

    bool GenerateTerrainAroundContinentCenter(CentralHex continentCenter, int continentNumber)
    {
        int q = Random.Range(
            continentCenter.Q - continentCenter.Range, 
            continentCenter.Q + continentCenter.Range
            );
        int r = Random.Range(
            continentCenter.R - continentCenter.Range, 
            continentCenter.R + continentCenter.Range
            );

        Hex augmentationCenterHex = GetHexAt(q, r);

        if ((augmentationCenterHex == null) || (augmentationCenterHex.Elevation > 0))
        {
            return false;
        }

        int range = (Random.Range(7, 23 - 3 * numContinents));

        ElevateArea(augmentationCenterHex, range, continentNumber);
        return true;
    }

    void ElevateArea(CentralHex centralHex, int continentNumber)
    {
        ElevateArea(centralHex, centralHex.Range, continentNumber);
    }

    void ElevateArea(Hex elevationCenterHex, int range, int continentNumber, float centerHeight = 1.5f)
    {
        
        Hex[] areaHexes = GetHexesWithinRangeOf(elevationCenterHex, range);

        foreach (Hex hex in areaHexes)
        {
            if (hex != null)
            {
                // if another continent
                // if ((hex.ContinentNumber != continentNumber) && (hex.ContinentNumber != -1))
                // {
                //     hex.Elevation = -0.5f;
                //     hex.ContinentNumber = -1;
                // }
                // else
                {
                    hex.Elevation += Mathf.Lerp(
                        0f, 
                        centerHeight, 
                        range / Mathf.Pow(Distance(elevationCenterHex, hex), 2)
                        );
                    Debug.Log("(" + hex.Q + ", " + hex.R + ") " + hex.Elevation + "\n"+
                        Distance(elevationCenterHex, hex));
                    float noise = Random.Range(0, 0.5f);
                    hex.Elevation += noise;
                    hex.ContinentNumber = continentNumber;
                }
                
            }
        }
    }
}
