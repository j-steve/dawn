using System;
using System.Collections.Generic;
using System.Linq;

static public class Utils
{
    static public string Format(this string target, params object[] args)
    {
        return string.Format(target, args);
    }

    /// <summary>
    /// Returns the array index of the given value.
    /// </summary>
    static public int IndexOf<T>(this T[] list, T value)
    {
        return Array.IndexOf(list, value);
    }

    /// <summary>
    /// Returns a random element from the array.
    /// </summary>
    static public T GetRandom<T>(this T[] list)
    {
        int randomIndex = UnityEngine.Random.Range(0, list.Length);
        return list[randomIndex];
    }

    /// <summary>
    /// Returns a random element from the array.
    /// </summary>
    static public T GetRandom<T>(this IEnumerable<T> list)
    {
        int size = list.Count();
        int randomIndex = UnityEngine.Random.Range(0, size);
        return list.ElementAt(randomIndex);
    }

    /// <summary>
    /// Returns a random element from the array.
    /// </summary>
    static public T GetRandom<T>(this T[,] list)
    {
        int randomIndex1 = UnityEngine.Random.Range(0, list.GetLength(0));
        int randomIndex2 = UnityEngine.Random.Range(0, list.GetLength(1));
        return list[randomIndex1, randomIndex2];
    }

    static public T TryGet<K, T>(this Dictionary<K, T> dict, K key)
    {
        return dict.TryGet(key, default(T));
    }

    static public T TryGet<K, T>(this Dictionary<K, T> dict, K key, T defaultValue)
    {
        T result;
        if (!dict.TryGetValue(key, out result)) { result = defaultValue; }
        return result;
    }

    static public string Join<T>(this IEnumerable<T> list, string joinWith)
    {
        var stringVals = list.Select(x => x == null ? "" : x.ToString());
        return String.Join(joinWith, stringVals.ToArray());
    }

    public static void EnqueueAll<T>(this Queue<T> queue, IEnumerable<T> values)
    {
        foreach (T value in values) { queue.Enqueue(value); }
    }
}

static public class EnumUtil
{
    static public IEnumerable<T> GetValues<T>()
    {
        return Enum.GetValues(typeof(T)).Cast<T>();
    }
}

public static class HashUtil
{
    private const int SEED_FIELD_PRIME_NUMBER = 691;
    private const int FIELD_PRIME_NUMBER = 397;

    public static int GetHashCodeFromFields<T1, T2, T3, T4>(object obj, T1 obj1, T2 obj2)
    {
        return GetHashCodeFromFields(obj1, obj2, null, null);
    }

    public static int GetHashCodeFromFields<T1, T2, T3, T4>(object obj, T1 obj1, T2 obj2, T3 obj3)
    {
        return GetHashCodeFromFields(obj1, obj2, obj3, null);
    }

    public static int GetHashCodeFromFields<T1, T2, T3, T4>(object obj, T1 obj1, T2 obj2, T3 obj3, T4 obj4)
    {
        int hashCode = SEED_FIELD_PRIME_NUMBER;
        unchecked {  //unchecked to prevent throwing overflow exception
            if (obj1 != null)
                hashCode *= FIELD_PRIME_NUMBER + obj1.GetHashCode();
            if (obj2 != null)
                hashCode *= FIELD_PRIME_NUMBER + obj2.GetHashCode();
            if (obj3 != null)
                hashCode *= FIELD_PRIME_NUMBER + obj3.GetHashCode();
            if (obj4 != null)
                hashCode *= FIELD_PRIME_NUMBER + obj4.GetHashCode();
        }
        return hashCode;
    }

    public static int GetHashCodeFromFields(object obj, params object[] fields)
    {
        unchecked { //unchecked to prevent throwing overflow exception
            int hashCode = SEED_FIELD_PRIME_NUMBER;
            for (int i = 0; i < fields.Length; i++)
                if (fields[i] != null)
                    hashCode *= FIELD_PRIME_NUMBER + fields[i].GetHashCode();
            return hashCode;
        }
    }


}