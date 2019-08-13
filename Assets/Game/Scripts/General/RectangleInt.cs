[System.Serializable]
public struct RectangleInt
{
    static public RectangleInt operator /(RectangleInt r1, RectangleInt r2)
    {
        return new RectangleInt(r1.Height / r2.Height, r1.Width / r2.Width);
    }
    static public RectangleInt operator /(RectangleInt r1, int r2)
    {
        return new RectangleInt(r1.Height / r2, r1.Width / r2);
    }

    public int Height;

    public int Width;

    public int Area { get { return Height * Width; } }

    public RectangleInt(int height, int width)
    {
        Height = height;
        Width = width;
    }

}