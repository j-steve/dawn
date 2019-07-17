using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class AnimalType
{
    static public HashSet<AnimalType> Values = new HashSet<AnimalType>();

    public string name;
    public string baseType;
    public List<Biome> biomes;
    public float ferocity;
    public float domesticity;
    public float spawnRate;
    public List<string> behavior;

    public AnimalType()
    {
        Values.Add(this);
    }

    public override string ToString()
    {
        return name + biomes.Join(",");
    }
}
