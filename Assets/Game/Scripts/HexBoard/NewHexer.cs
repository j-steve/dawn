using UnityEngine;
using System.Collections;
using System.Collections.Generic;
 
public class NewHexer : MonoBehaviour
{
    public HexChunk hexChunkPrefab;
    public HexCell hexCellPrefab;
    public UnityEngine.UI.Text hexLabelPrefab;
    public Texture2D noiseSource;

    // Use this for initialization
    void Start()
    {
        HexBoard.Active = new HexBoard() {
            hexLabelPrefab = hexLabelPrefab,
            noiseSource = noiseSource,
            mapSize = new RectangleInt(8, 8),
            continentsPerChunk = 0.5f
        };
        var hexChunk = HexChunk.Create(hexChunkPrefab, transform, 0, 0);
        AddCell(hexChunk, 0, 0, Biome.JUNGLE);
        AddCell(hexChunk, 0, 1, Biome.PLAINS);
        AddCell(hexChunk, 1, 0, Biome.SAVANNAH);
        AddCell(hexChunk, 1, 1, Biome.SCRUBLAND);
        hexChunk.Triangulate(HexBoard.Active.hexCells.Values);
    }

    void AddCell(HexChunk chunk, int row, int col, Biome biome)
    {

        var cell = HexCell.Create(hexCellPrefab, chunk, row, col);
        cell.Biome = biome;
        cell.BiomeNumber = 1;
        cell.Elevation = cell.TerrainType == TerrainTexture.BLUEWATER ? 0 : 2;
        HexBoard.Active.hexCells.Add(cell.Coordinates, cell);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
