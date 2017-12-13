using UnityEngine;
/// <summary>
/// Represents the various terrain textures stored in the texture array,
/// which contains all the basic textures used for HexCell tiles.
/// The enum value must correlate with the index of the terrain type within the
/// texture array.
/// </summary>
public enum TerrainTexture
{
    TEALWATER,
    BLUEWATER,
    GRASS,
    TEMPERATE_FOREST,
    SAND_DUNES,
    GLACIER,
    TWISTED_GRASS,
    ROCKY_SNOW,
    GREENTREES,
    MIXEDTREES,
    SAVANNAH,
    STEPPE,
    SAND,
    CLIFF,
    SNOW,
    THINGRASS,
    PRETTYICE,
    FOILAGE,
}

public class TexturedEdge : Edge
{
    public TerrainTexture texture;


    public TexturedEdge(Edge e1, TerrainTexture texture) : base(e1.vertex1, e1.vertex2)
    {
        this.texture = texture;
    }

    public TexturedEdge(Vector3 vertex1, Vector3 vertex2, TerrainTexture texture) : base(vertex1, vertex2)
    {
        this.texture = texture;
    }

    new public TexturedEdge Reversed()
    {
        return new TexturedEdge(vertex2, vertex1, texture);
    }
}