using System.Collections.Generic;

class MultiDict<TKey, TValue> : Dictionary<TKey, List<TValue>>
{
    public void Add(TKey key, TValue value)
    {
        List<TValue> list;
        if (!TryGetValue(key, out list)) {
            this[key] = list = new List<TValue>();
        }
        list.Add(value);
    }

}