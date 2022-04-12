using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMapContinents : HexMap
{
    [Range(1, 3)]
    [SerializeField] int minContinents = 2;
    [Range(1, 3)]
    [SerializeField] int maxContinents = 2;
    [SerializeField] int territorySize = 40;

    int numContinents;
    int territoryNumber = 1;
    Queue<Hex> territoryHexes;

    void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        GenerateTiles();

        GenerateContinents();

        UpdateHexVisuals();
        SetLabels();
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
        int numTerritories = Random.Range(5, 7);

        int startQ = Width / numContinents * continentNumber;
        int startR = Height / 2;
        Hex startHex = GetHexAt(startQ, startR);

        GenerateTerritory(startHex);
        numTerritories--;
        territoryNumber++;

        while (numTerritories > 0)
        {
            startHex = territoryHexes.Dequeue();
            while (!HasEmptyTerritoryAround(startHex))
            {
                startHex = territoryHexes.Dequeue();
            }
            // PrintCoordinates(startHex);
            GenerateTerritory(startHex);
            territoryNumber++;
            numTerritories--;
        }
    }

    void GenerateTerritory(Hex hex)
    {
        territoryHexes = new Queue<Hex>();

        GiveTerritoryNumber(hex);

        Expand(hex, territorySize);
    }

    void Expand(Hex hex, int jumpsCount)
    {
        List<Hex> neighbors = GetNeighbors(hex);

        ExpandOnNeighbors(hex, neighbors, 0, jumpsCount);

        while (jumpsCount > 0)
        {
            hex = territoryHexes.Peek();

            while (!HasEmptyTerritoryAround(hex))
            {
                territoryHexes.Dequeue();
                hex = territoryHexes.Peek();
            }

            neighbors = GetNeighbors(hex);
            ExpandOnNeighbors(hex, neighbors, 0, jumpsCount);

            jumpsCount--;
        }
    }

    void ExpandOnNeighbors(Hex hex, List<Hex> neighbors, int expanded, int jumpsCount)
    {
        if (neighbors.Count == 0)
            return;

        int neighbor = Random.Range(0, neighbors.Count - 1);

        Hex nextHex = neighbors[neighbor];

        if (nextHex.Territory == -1)
        {
            AddNextHex(hex, nextHex, neighbors, neighbor, expanded, jumpsCount);
        }
        else
        {
            neighbors.RemoveAt(neighbor);
            ExpandOnNeighbors(hex, neighbors, expanded, jumpsCount);
        }
    }

    void AddNextHex(Hex hex, Hex nextHex, List<Hex> neighbors, int neighbor,
        int expanded, int jumpsCount)
    {
        GiveTerritoryNumber(nextHex);

        neighbors.RemoveAt(neighbor);
        expanded++;
        // int expandAmount = Random.Range(22, 39) / 10;
        if (expanded < 3)
        {
            ExpandOnNeighbors(hex, neighbors, expanded, jumpsCount);
        }
    }

    // void Jump(Hex nextHex, int jumpsCount)
    // {
    //     jumpsCount--;

    //     Debug.Log(jumpsCount);
    //     Debug.Log(nextHex.Q + ", " + nextHex.R);

    //     List<Hex> nextNeighbors = GetNeighbors(nextHex);

    //     if (!HasEmptyTerritoryAround(nextNeighbors))
    //     {
    //         nextHex = territoryHexes[0];
    //         territoryHexes.RemoveAt(0);
    //         Debug.Log("Has no empty territories around\n" +
    //             "nextHex: " + nextHex.Q + ", " + nextHex.R);
    //     }

    //     ExpandOnNeighbors(nextHex, GetNeighbors(nextHex), 0, jumpsCount);
    // }

    bool HasEmptyTerritoryAround(Hex hex)
    {
        foreach (Hex neighbor in GetNeighbors(hex))
        {
            if (neighbor.Territory == -1)
            {
                return true;
            }
        }

        return false;
    }

    List<Hex> GetNeighbors(Hex hex)
    {
        int q = hex.Q;
        int r = hex.R;
        List<Hex> neighbors = new List<Hex>();

        neighbors.Add(GetHexAt(q - 1, r));

        if (r < Height)
        {
            neighbors.Add(GetHexAt(q - 1, r + 1));
            neighbors.Add(GetHexAt(q, r + 1));
        }

        if (r > 0)
        {
            neighbors.Add(GetHexAt(q, r - 1));
            neighbors.Add(GetHexAt(q + 1, r - 1));
        }

        neighbors.Add(GetHexAt(q + 1, r));

        return neighbors;
    }

    void GiveTerritoryNumber(Hex hex)
    {
        hex.Territory = territoryNumber;
        territoryHexes.Enqueue(hex);
        hex.Elevation = 1;
        // Debug.Log("Territory " + territoryNumber + " added: " + hex.Q + ", " + hex.R);
        Debug.Log(territoryHexes.Count);
    }

    void PrintCoordinates(Hex hex)
    {
        Debug.Log(hex.Q + ", " + hex.R);
    }
}
