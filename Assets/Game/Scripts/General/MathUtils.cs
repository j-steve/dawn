static public class MathUtils
{
    /// <summary>
    /// Retuns the m modulo of x, constraining the value to a positive result
    /// (unlike the native mod operator).
    /// </summary>
    /// <param name="x"></param>
    /// <param name="m"></param>
    /// <returns></returns>
    static public int Mod(int x, int m)
    {
        return (x % m + m) % m;
    }

    /// <summary>
    /// Zero-safe division: divides x by y, but returns 0 if y is 0.
    /// </summary>
    static public float DividedBy(this float x, float y)
    {
        return x.DividedBy(y, 0);
    }
    /// <summary>
    /// Zero-safe division: divides x by y, but returns the specified default
    /// value if y is 0.
    /// </summary>
    static public float DividedBy(this float x, float y, float defaultIfZero)
    {
        return y == 0f ? defaultIfZero : x / y;
    }

    /// <summary>
    /// Zero-safe division: divides x by y, but returns 0 if y is 0.
    /// </summary>
    static public float DividedBy(this int x, float y)
    {
        return x.DividedBy(y, 0);
    }
    /// <summary>
    /// Zero-safe division: divides x by y, but returns the specified default
    /// value if y is 0.
    /// </summary>
    static public float DividedBy(this int x, float y, float defaultIfZero)
    {
        return y == 0f ? defaultIfZero : x / y;
    }
}
