using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public abstract class EnumClass : IComparable<EnumClass>, IEquatable<EnumClass>
{
    #region Static

    static private readonly Dictionary<Type, List<EnumClass>> AllEnums =
        new Dictionary<Type, List<EnumClass>>();

    static public IEnumerable<T> GetAll<T>() where T : EnumClass
    {
        List<EnumClass> typeValues = AllEnums.TryGet(typeof(T));
        return typeValues.Cast<T>() ?? new List<T>();
    }

    static public int AbsoluteDifference(EnumClass firstValue, EnumClass secondValue)
    {
        return Math.Abs(firstValue.Value - secondValue.Value);
    }

    #region Parsing From Name/Value

    static public T FromValue<T>(int value) where T : EnumClass
    {
        List<EnumClass> typeValues = AllEnums.TryGet(typeof(T));
        if (value < 0 || value >= typeValues.Count) {
            throw new IndexOutOfRangeException(
                "Invalid value for {0}: '{1}' is out of range (max value={2}).".Format(typeof(T), value, typeValues.Count));
        }
        return (T)typeValues[value];
    }

    static public T FromName<T>(string displayName) where T : EnumClass
    {
        return parse<T, string>(displayName, "display name", item => item.Name == displayName);
    }

    static private T parse<T, K>(K value, string description, Func<T, bool> predicate) where T : EnumClass
    {
        T matchingItem = GetAll<T>().FirstOrDefault(predicate);
        if (matchingItem == null) {
            throw new ApplicationException(
                "'{0}' is not a valid {1} in {2}".Format(value, description, typeof(T)));
        }
        return matchingItem;
    }

    #endregion

    #region Operator Overloads

    //static public bool operator ==(EnumClass c1, EnumClass c2)
    //{
    //    return c1.Value == c2.Value;
    //}

    //static public bool operator !=(EnumClass c1, EnumClass c2)
    //{
    //    return c1.Value != c2.Value;
    //}

    static public bool operator >(EnumClass c1, EnumClass c2)
    {
        return c1.Value > c2.Value;
    }

    static public bool operator <(EnumClass c1, EnumClass c2)
    {
        return c1.Value < c2.Value;
    }

    static public bool operator >=(EnumClass c1, EnumClass c2)
    {
        return c1.Value >= c2.Value;
    }

    static public bool operator <=(EnumClass c1, EnumClass c2)
    {
        return c1.Value <= c2.Value;
    }

    static public implicit operator int(EnumClass target)
    {
        return target.Value;
    }

    static public implicit operator string(EnumClass target)
    {
        return target.Name;
    }

    #endregion

    #endregion

    public readonly int Value;
    public readonly string Name;

    protected EnumClass(string name)
    {
        Name = name;
        Value = AddToDictionary();
    }

    private int AddToDictionary()
    {
        Type type = this.GetType();
        if (!AllEnums.ContainsKey(type)) {
            AllEnums[type] = new List<EnumClass>();
        }
        AllEnums[type].Add(this);
        return AllEnums[type].Count - 1;
    }

    public override string ToString()
    {
        return Name;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    #region Object Comparison

    public override bool Equals(object obj)
    {
        EnumClass otherValue = obj as EnumClass;

        if (otherValue == null) { return false; }

        bool typeMatches = GetType().Equals(obj.GetType());
        bool valueMatches = Value.Equals(otherValue.Value);
        return typeMatches && valueMatches;
    }

    int IComparable<EnumClass>.CompareTo(EnumClass other)
    {
        return Value.CompareTo(other.Value);
    }

    bool IEquatable<EnumClass>.Equals(EnumClass other)
    {
        return Value == other.Value;
    }

    #endregion
}