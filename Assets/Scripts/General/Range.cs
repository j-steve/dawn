
public static class Range
{
    static public Range<int> Of(int min, int max) { return new RangeInt(min, max); }
    static public Range<float> Of(float min, float max) { return new RangeFloat(min, max); }
    static public Range<double> Of(double min, double max) { return new RangeDouble(min, max); }

    private class RangeInt : Range<int>
    {
        public override int Average { get { return (Min + Max) / 2; } }
        internal RangeInt(int min, int max) : base(min, max) { }
    }

    private class RangeFloat : Range<float>
    {
        public override float Average { get { return (Min + Max) / 2; } }
        internal RangeFloat(float min, float max) : base(min, max) { }
    }

    private class RangeDouble : Range<double>
    {
        public override double Average { get { return (Min + Max) / 2; } }
        internal RangeDouble(double min, double max) : base(min, max) { }
    }

}

abstract public class Range<T>
{
    public readonly T Min;
    public readonly T Max;
    abstract public T Average { get; }

    protected internal Range(T min, T max)
    {
        Min = min;
        Max = max;
    }
}
