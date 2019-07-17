using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class AnimalType
{
    static public HashSet<AnimalType> Values = new HashSet<AnimalType>();

    static public AnimalType GetForBiome(Biome biome)
    {
        var validAnimals = new Dictionary<float, AnimalType>();
        float totalProbability = 0;
        foreach (AnimalType animalType in Values) {
            if (animalType.biomes != null && animalType.biomes.Contains(biome) && animalType.prefabs != null && animalType.prefabs.Count > 0) { // TODO: Remove prefab count check.
                totalProbability += animalType.spawnRate;
                validAnimals[totalProbability] = animalType;
            }
        }
        if (totalProbability == 0) { return null; }
        float randomValue = Random.value * totalProbability;
        float chosenKey = 0;
        foreach (float key in validAnimals.Keys) {
            if (key > chosenKey && key >= randomValue) { chosenKey = key; }
        }
        if (!validAnimals.ContainsKey(chosenKey)) {
            Debug.LogErrorFormat("{0} doesn't have {1}, rand is {2}", validAnimals.Keys.Join(","), chosenKey, randomValue);
            return null;
        }
        return validAnimals[chosenKey];
    }

    public string name;
    public string baseType;
    public List<Biome> biomes;
    public float ferocity;
    public float domesticity;
    public float spawnRate;
    public List<Behavior> behavior;
    public List<string> prefabs;

    public AnimalType()
    {
        Values.Add(this);
    }

    public override string ToString()
    {
        return name + biomes.Join(",");
    }
}

public enum Behavior
{
    Grazer,
    Predator,
    SmallGameHunter,
    Maneater,
    Hibernator,
    Territorial,
}
