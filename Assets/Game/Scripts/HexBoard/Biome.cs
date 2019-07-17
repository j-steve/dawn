using UnityEngine;
using System.Collections;

public class Biome
{

    static public Biome SEA = new Biome(0, "Ocean", TerrainTexture.BLUEWATER, 20, Range.Of(0, 0), 0f, Range.Of(0, 100));
    static public Biome OCEAN = new Biome(1, "Ocean", TerrainTexture.BLUEWATER, 100, Range.Of(0, 0), 0f, Range.Of(0, 100));
    static public Biome ICE_CAPS = new Biome(2, "Ice Caps", TerrainTexture.GLACIER, 20, Range.Of(1, 3), .2f, Range.Of(95, 100));
    static public Biome LAKE = new Biome(-1, "Lake", TerrainTexture.TEALWATER, 2, Range.Of(0, 0), 0, Range.Of(0, 100));
    static public Biome TUNDRA = new Biome(3, "Tundra", TerrainTexture.ROCKY_SNOW, 30, Range.Of(1, 4), .3f, Range.Of(70, 90), 0.25f);
    static public Biome TAIGA = new Biome(4, "Taiga", TerrainTexture.GREENTREES, 30, Range.Of(1, 4), .5f, Range.Of(50, 85), 1);
    static public Biome FOREST = new Biome(5, "Forest", TerrainTexture.TEMPERATE_FOREST, 30, Range.Of(1, 4), .5f, Range.Of(15, 75), 1);
    static public Biome PLAINS = new Biome(6, "Plains", TerrainTexture.TWISTED_GRASS, 50, Range.Of(1, 3), .4f, Range.Of(15, 75), 0.15f);
    static public Biome JUNGLE = new Biome(7, "Jungle", TerrainTexture.MIXEDTREES, 30, Range.Of(1, 5), .75f, Range.Of(0, 50), 1, new Vector3(2, 3, 2));
    static public Biome SAVANNAH = new Biome(8, "Savannah", TerrainTexture.SAVANNAH, 50, Range.Of(1, 2), .25f, Range.Of(0, 50), .15f);
    static public Biome SCRUBLAND = new Biome(9, "Scrubland", TerrainTexture.STEPPE, 20, Range.Of(1, 5), .75f, Range.Of(0, 50), 1);
    static public Biome DESERT = new Biome(10, "Desert", TerrainTexture.SAND_DUNES, 10, Range.Of(1, 3), .3f, Range.Of(0, 30), .05f);

    static internal readonly Biome[] Values = new Biome[] {
        SEA,
        OCEAN,
        ICE_CAPS,
        TUNDRA,
        TAIGA,
        FOREST,
        PLAINS,
        JUNGLE,
        SAVANNAH,
        SCRUBLAND,
        DESERT,
    };


    static internal Biome GetByID(int id)
    {
        if (id == -1) { return LAKE; }
        return Values[id];
    }

    static internal Biome GetByName(string name)
    {
        foreach (Biome b in Values) {
            if (b.name == name) { return b; }
        }
        if (name == "Swamp" || name == "Alpine") { return null; } // TODO: remove and add these biomes.
        throw new System.ArgumentException(string.Format("Biome \"{0}\" does not exist.", name));
    }

    public readonly int id;
    public readonly string name;
    public readonly TerrainTexture terrainTexture;
    public readonly int minSize;
    public readonly Range<int> elevation;
    public readonly float bumpiness;
    public readonly Range<int> latitude;
    public readonly float treeProbability;
    public readonly Vector3 treeSizeModifier;
    public float animalProbability { get { return treeProbability / 100; } }

    private Biome(int id, string name, TerrainTexture terrainTexture, int minSize, Range<int> elevation, float bumpiness, Range<int> latitude, float treeProbability = 0, Vector3? treeSizeModifier = null)
    {
        this.id = id;
        this.name = name;
        this.terrainTexture = terrainTexture;
        this.minSize = minSize;
        this.elevation = elevation;
        this.bumpiness = bumpiness;
        this.latitude = latitude;
        this.treeProbability = treeProbability;
        this.treeSizeModifier = treeSizeModifier ?? Vector3.one;
    }

    public override string ToString() { return name; }

    public static explicit operator Biome(string biomeName)
    {
        return GetByName(biomeName);
    }
}
