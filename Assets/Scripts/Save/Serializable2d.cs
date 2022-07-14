// We need Serializable2d struct to serialize 2d array
// in xml. 

[System.Serializable]
public struct Serializable2d<Hex>
{
    public int Dimension1;
    public int Dimension2;
    public Hex hex;

    public Serializable2d(int x, int y, Hex hex)
    {
        Dimension1 = x;
        Dimension2 = y;
        this.hex = hex;
    }
}
