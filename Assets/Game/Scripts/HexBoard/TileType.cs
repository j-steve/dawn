using System.Collections.Generic;
using Newtonsoft.Json;

public class TileType
{
    static public HashSet<TileType> Values = new HashSet<TileType>();

    static public TileType GetByName(string name)
    {
        foreach (TileType b in Values) { if (b.name == name) { return b; } }
        throw new System.ArgumentException(string.Format("TileType \"{0}\" does not exist.", name));
    }

    public readonly string name;
    public readonly TileType baseType;
    public readonly Biome biome;
    public readonly string description;
    public readonly IDictionary<ResourceType, TileResourceInfo> resources;
    public readonly float frequency;
    public readonly float elevationChange;
    public readonly float movementSpeed;
    public readonly float defensiveBonus;
    public readonly float visibility;

    private TileType() { }

    [JsonConstructor]
    public TileType(string name, string baseType, string biome, string description, float? frequency, float? elevationChange, IDictionary<string, float[]> resources, float? movementSpeed, float? defensiveBonus, float? visibility)
    {
        this.name = name;
        this.baseType = baseType == null ? null : GetByName(baseType);
        var parent = this.baseType ?? new TileType();
        this.biome = biome == null ? null : Biome.GetByName(biome);
        this.description = description;
        this.frequency = frequency ?? parent.frequency;
        this.elevationChange = elevationChange ?? parent.elevationChange;
        this.resources = parseResources(resources);
        this.movementSpeed = movementSpeed.GetValueOrDefault();
        this.defensiveBonus = defensiveBonus.GetValueOrDefault();
        this.visibility = visibility.GetValueOrDefault();
        Values.Add(this);
    }

    IDictionary<ResourceType, TileResourceInfo> parseResources(IDictionary<string, float[]> input)
    {
        var output = new Dictionary<ResourceType, TileResourceInfo>();
        if (input != null) {
            foreach (var kvp in input) {
                var resourceType = ResourceType.GetByName(kvp.Key);
                output[resourceType] = new TileResourceInfo() {
                    quantity = kvp.Value[0],
                    regenRate = kvp.Value[0],
                    quality = kvp.Value[0],
                };
            }
        }
        return output;
    }

    public override string ToString()
    {
        return name;
    }
}

public struct TileResourceInfo
{
    public float quantity;
    public float regenRate;
    public float quality;
}
