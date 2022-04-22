using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMapContinents : HexMap
{
    int minContinents = 2;
    int maxContinents = 2;
    int territorySize = 40;

    int numContinents;
    int territoryNumber = 1;
    int continentNumber = 0;
    Queue<Hex> territoryHexes;
    Queue<Queue<Hex>> territories;

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
        DrawBorders();
    }

    void GenerateContinents()
    {
        numContinents = Random.Range(minContinents, maxContinents);

        while(numContinents > continentNumber)
        {
            GenerateContinent(continentNumber);
            continentNumber++;
        }
    }

    void GenerateContinent(int continentNumber)
    {
        territories = new Queue<Queue<Hex>>();

        int numTerritories = Random.Range(18, 23);

        int startQ = Width / numContinents * continentNumber;
        int startR = Height / 2;
        Hex startHex = GetHexAt(startQ, startR);

        GenerateTerritory(startHex);
        numTerritories--;
        territoryNumber++;

        while (numTerritories > 0)
        {
            startHex = GetStartHex();
            GenerateTerritory(startHex);
            territoryNumber++;
            numTerritories--;
        }
    }

    Hex GetStartHex()
    {
        territoryHexes = territories.Dequeue();

        try
        {
            Hex startHex = territoryHexes.Dequeue();
            while (!HasEmptyTerritoryAround(startHex))
            {
                startHex = territoryHexes.Dequeue();
            }

            if (!HasEmptyTerritoryAround(startHex))
            {
                return GetStartHex();
            }

            return startHex;
        }
        catch(System.InvalidOperationException) 
        {
            return GetStartHex();
        }
    }

    void GenerateTerritory(Hex hex)
    {
        territoryHexes = new Queue<Hex>();

        NumberAndSowHex(hex);

        Expand(hex, territorySize);

        territories.Enqueue(territoryHexes);
        territories.Enqueue(territoryHexes);
    }

    void Expand(Hex hex, int jumpsCount)
    {
        List<Hex> neighbors = GetNeighbors(hex);

        ExpandOnNeighbors(hex, neighbors, 0, jumpsCount);

        while (jumpsCount > 0)
        {
            hex = GetNextHex();

            if (hex != null)
            {
                neighbors = GetNeighbors(hex);
                ExpandOnNeighbors(hex, neighbors, 0, jumpsCount);
            }

            jumpsCount--;
        }
    }

    Hex GetNextHex()
    {
        if (territoryHexes.Count == 0)
            return null;

        Hex hex = territoryHexes.Peek();

        while (!HasEmptyTerritoryAround(hex))
        {
            territoryHexes.Dequeue();

            if (territoryHexes.Count == 0)
                return null;

            hex = territoryHexes.Peek();
        }

        return hex;
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
        else if ((nextHex.Continent != continentNumber) && (nextHex.Continent != -1))
        {
            // Separate continent

            hex.Territory = -1;
            hex.Elevation = -1;
            neighbors.RemoveAt(neighbor);
            ExpandOnNeighbors(hex, neighbors, expanded, jumpsCount);
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
        NumberAndSowHex(nextHex);

        neighbors.RemoveAt(neighbor);
        expanded++;
        // int expandAmount = Random.Range(22, 39) / 10;
        if (expanded < 3)
        {
            ExpandOnNeighbors(hex, neighbors, expanded, jumpsCount);
        }
    }

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

        if (r < Height - 1)
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

    void NumberAndSowHex(Hex hex)
    {
        hex.Territory = territoryNumber;
        territoryHexes.Enqueue(hex);
        hex.Elevation = 1;

        if (Random.Range(0, 2) == 1)
        {
            hex.IsForest = true;
            hex.MovementCost = 2;
        }
    }

    void PrintCoordinates(Hex hex)
    {
        Debug.Log(hex.Q + ", " + hex.R);
    }
}
