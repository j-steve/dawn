using UnityEngine;
using System.Collections;

public class Biome
{
    public static readonly Biome[] Values = new Biome[] {
        new Biome("Ocean", TerrainTexture.BLUEWATER, 20, Range.Of(0, 0), 0f, Range.Of(0, 100)),
        new Biome("Ocean", TerrainTexture.BLUEWATER, 100, Range.Of(0, 0), 0f, Range.Of(0, 100)),
        //new Biome("Lake", TerrainTexture.TEALWATER, 2, Range.Of(0, 0), 0, Range.Of(0, 100)),
        new Biome("Ice Caps", TerrainTexture.GLACIER, 20, Range.Of(1, 3), .2f, Range.Of(95, 100)),
        new Biome("Tundra", TerrainTexture.ROCKY_SNOW, 30, Range.Of(1, 4), .3f, Range.Of(70, 90)),
        new Biome("Taiga", TerrainTexture.GREENTREES, 30, Range.Of(1, 4), .5f, Range.Of(50, 85)),
        new Biome("Temperate Forest", TerrainTexture.TEMPERATE_FOREST, 30, Range.Of(1, 4), .5f, Range.Of(15, 75)),
        new Biome("Grassland", TerrainTexture.TWISTED_GRASS, 50, Range.Of(1, 3), .4f, Range.Of(15, 75)),
        new Biome("Rainforest", TerrainTexture.MIXEDTREES, 30, Range.Of(1, 5), .75f, Range.Of(0, 50)),
        new Biome("Savannah", TerrainTexture.SAVANNAH, 50, Range.Of(1, 2), .25f, Range.Of(0, 50)),
        new Biome("Shrubland", TerrainTexture.STEPPE, 20, Range.Of(1, 5), .75f, Range.Of(0, 50)),
        new Biome("Desert", TerrainTexture.SAND_DUNES, 10, Range.Of(1, 3), .3f, Range.Of(0, 30)),
    };

    public readonly string name;
    public readonly TerrainTexture terrainTexture;
    public int minSize;
    public Range<int> elevation;
    public float bumpiness;
    public Range<int> latitude;

    private Biome(string name, TerrainTexture terrainTexture, int minSize, Range<int> elevation, float bumpiness, Range<int> latitude)
    {
        this.name = name;
        this.terrainTexture = terrainTexture;
        this.minSize = minSize;
        this.elevation = elevation;
        this.bumpiness = bumpiness;
        this.latitude = latitude;
    }

    public override string ToString() { return name; }
}
