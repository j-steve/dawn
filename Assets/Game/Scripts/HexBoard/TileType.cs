using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public class TileType
{
    static public HashSet<TileType> Values = new HashSet<TileType>();

    static public TileType GetByName(string name)
    {
        foreach (TileType b in Values) { if (b.name == name) { return b; } }
        throw new System.ArgumentException(string.Format("TileType \"{0}\" does not exist.", name));
    }

    static public TileType GetForBiome(Biome biome)
    {
        var biomeTiles = new Dictionary<float, TileType>();
        float totalProbability = 0;
        foreach (TileType tile in Values) {
            if (tile.biome != null && tile.biome == biome) { // TODO: Remove prefab count check.
                totalProbability += tile.frequency;
                biomeTiles[totalProbability] = tile;
            }
        }
        if (totalProbability == 0) {
            // TODO: Uncomment the following line once all biomes have tile types:
            //throw new System.ArgumentException(string.Format("No valid tile types for biome \"{0}\".", biome)); 
            return null;
        }
        float randomValue = Random.value * totalProbability;
        float chosenKey = biomeTiles.Keys.Where(k => k > randomValue).Min();
        return biomeTiles[chosenKey];
    }

    public readonly string name;
    public readonly TileType baseType;
    public readonly Biome biome;
    public readonly string description;
    public readonly IDictionary<ResourceType, TileResourceInfo> resources;
    public readonly float frequency;
    public readonly float movementSpeed;
    public readonly float defensiveBonus;
    public readonly float visibility;
    public readonly float elevationChange;
    public readonly int treeCount;

    private TileType() { }

    [JsonConstructor]
    public TileType(string name, string baseType, string biome, string description, float? frequency, IDictionary<string, float[]> resources, float? movementSpeed, float? defensiveBonus, float? visibility, float? elevationChange, int? treeCount)
    {
        this.name = name;
        this.baseType = baseType == null ? null : GetByName(baseType);
        var parent = this.baseType ?? new TileType();
        this.biome = biome == null ? null : Biome.GetByName(biome);
        this.description = description;
        this.frequency = frequency ?? parent.frequency;
        this.resources = parseResources(resources);
        this.movementSpeed = movementSpeed.GetValueOrDefault();
        this.defensiveBonus = defensiveBonus.GetValueOrDefault();
        this.visibility = visibility.GetValueOrDefault();
        this.elevationChange = elevationChange ?? parent.elevationChange;
        this.treeCount = treeCount ?? parent.treeCount;
        Values.Add(this);
    }

    IDictionary<ResourceType, TileResourceInfo> parseResources(IDictionary<string, float[]> input)
    {
        var output = baseType == null ? new Dictionary<ResourceType, TileResourceInfo>() : new Dictionary<ResourceType, TileResourceInfo>(baseType.resources);
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
