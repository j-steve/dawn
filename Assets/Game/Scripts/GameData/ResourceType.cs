using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

public class ResourceType
{
    static public HashSet<ResourceType> Values = new HashSet<ResourceType>();

    static public ResourceType GetByName(string name)
    {
        foreach (ResourceType b in Values) {if (b.name == name) { return b; }}
        throw new System.ArgumentException(string.Format("ResourceType \"{0}\" does not exist.", name));
    }

    public readonly string name;
    public readonly ResourceType baseType;
    public readonly float deteriorationRate;

    private ResourceType() { }

    [JsonConstructor]
    public ResourceType(string name, string baseType, float? deteriorationRate)
    {
        this.name = name;
        this.baseType = baseType == null ? null : GetByName(baseType);
        var parent = this.baseType ?? new ResourceType();
        this.deteriorationRate = deteriorationRate ?? parent.deteriorationRate;
        Values.Add(this);
    }

    public override string ToString()
    {
        return name;
    }
}



