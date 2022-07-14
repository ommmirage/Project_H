using System.Runtime.Serialization;

// We need PathHexes to build units' paths. Every unit has
// it's PathHexes map

[DataContract]
public class PathHex
{
    [DataMember]
    int q;
    public int Q { get { return q; } }
    [DataMember]
    int r;
    public int R { get { return r; } }

    [DataMember]
    public float Elevation;

    [DataMember]
    public int MovementCost;

    // Walking cost from the start hex
    public float GCost = int.MaxValue;
    public PathHex CameFromPathHex = null;
    public bool IsWalkable = true;
    public bool Embark = false;
    public int MovesRemaining;

    public PathHex(Hex hex)
    {
        q = hex.Q;
        r = hex.R;
        Elevation = hex.Elevation;
        MovementCost = hex.MovementCost;
    }

    public override string ToString()
    {
        return Q + ", "+ R;
    }

    public int CalculateSteps()
    {
        int steps = 0;

        while (CameFromPathHex != null)
            steps++;

        return steps;
    }
}
