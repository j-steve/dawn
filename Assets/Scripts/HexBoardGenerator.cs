using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class HexBoardGenerator : MonoBehaviour
{
    private HexBoard hexBoard;

    public HexBoardGenerator(HexBoard hexBoard)
    {
        this.hexBoard = hexBoard;
    }

    public void CreateMap()
    {
        hexBoard.hexCells.Clear();
        var hexChunks = GetHexMeshChunks();
        GenerateTerrain();
        // Triangulate each HexMeshChunk to make the map visible.
        foreach (var entry in hexChunks) {
            entry.Key.Triangulate(entry.Value);
        }

    }

    Dictionary<HexMeshChunk, IEnumerable<HexCell>> GetHexMeshChunks()
    {
        var chunks = new Dictionary<HexMeshChunk, IEnumerable<HexCell>>();
        for (int row = 0; row < hexBoard.mapSize.Height; row++) {
            for (int column = 0; column < hexBoard.mapSize.Width; column++) {
                var chunk = HexMeshChunk.Create(hexBoard.hexMeshChunkPrefab, row, column);
                chunks[chunk] = GetChunkHexCells(row, column);
            }
        }
        return chunks;
    }

    IEnumerable<HexCell> GetChunkHexCells(int chunkRow, int chunkColumn)
    {
        var chunkCells = new List<HexCell>();
        int rowStart = chunkRow * HexConstants.CELLS_PER_CHUNK_ROW;
        int rowEnd = rowStart + HexConstants.CELLS_PER_CHUNK_ROW;
        int colStart = chunkColumn * HexConstants.CELLS_PER_CHUNK_ROW;
        int colEnd = colStart + HexConstants.CELLS_PER_CHUNK_ROW;
        for (int row = rowStart; row < rowEnd; row++) {
            for (int column = colStart; column < colEnd; column++) {
                var cell = HexCell.Create(hexBoard.hexCellPrefab, row, column);
                hexBoard.hexCells[cell.Coordinates] = cell;
                chunkCells.Add(cell);
            }
        }
        return chunkCells;
    }

    void GenerateTerrain()
    {
        //var biomes = Biome.Values.Where(b => b.terrainTexture != TerrainTexture.TEALWATER);
        int continents = Mathf.FloorToInt(hexBoard.continentsPerChunk * hexBoard.mapSize.Area);
        var oceanCells = new HashSet<HexCell>();
        var blankCells = new HashSet<HexCell>(hexBoard.hexCells.Values);
        Debug.Log(continents + "for" + hexBoard.mapSize.Area);
        for (int continent = 0; continent < continents; continent++) {
            if (blankCells.Count == 0) { break; }
            var biome = Biome.Values.GetRandom();
            var potentialCells = blankCells.Where(c => LatitudeInRange(c, biome));
            if (potentialCells.Count() == 0) { continue; }
            var firstCell = potentialCells.GetRandom();
            Debug.Log("{0} is in range of {1}-{2}".Format(System.Math.Abs(firstCell.Latitude * 100), biome.latitude.Min, biome.latitude.Max));
            int targetSize = Random.Range(biome.minSize, biome.minSize * 4 + 1);
            var continentCells = new HashSet<HexCell>() { firstCell };
            var frontierCells = new HashSet<HexCell>(NeighborsInBiome(firstCell, 0));
            while (continentCells.Count < targetSize && frontierCells.Count > 0 && blankCells.Count > 0) {
                HexCell nextCell = frontierCells.GetRandom();
                EdgeDirection direction = EnumClass.GetAll<EdgeDirection>().GetRandom();
                if (blankCells.Remove(nextCell)) {
                    continentCells.Add(nextCell);
                    var neighbors = nextCell.GetNeighbors().Where(c => c.Biome == 0);
                    foreach (HexCell neighbor in neighbors) {
                        if (Random.value * 100 < biome.bumpiness) {
                            // int minChange = biome.elevation.Min - neighbor.Elevation;
                            // int maxChange = biome.elevation.Max - neighbor.Elevation;
                            // neighbor.Elevation += Random.Range(minChange, maxChange);
                        }
                    }
                    frontierCells.UnionWith(neighbors);
                }
                frontierCells.Remove(nextCell);
            }
            if (continentCells.Count >= biome.minSize) {
                foreach (HexCell cell in continentCells) {
                    cell.TerrainType = biome.terrainTexture;
                    cell.Biome = continent + 1;
                    // cell.Elevation = Mathf.Clamp(cell.Elevation, biome.elevation.Min, biome.elevation.Max);
                    cell.Elevation = cell.TerrainType == TerrainTexture.BLUEWATER ? 0 : 2;
                }
                if (biome.terrainTexture == TerrainTexture.BLUEWATER) {
                    oceanCells.UnionWith(continentCells);
                }
                SetContinentElevation(biome, continentCells);
            }
        }
        SpreadOceans(oceanCells);
    }

    void SpreadOceans(HashSet<HexCell> oceanCells)
    {
        Debug.Log("Spreading oceans...");
        while (oceanCells.Count > 0) {
            var tile = oceanCells.First();
            foreach (HexCell neighbor in NeighborsInBiome(tile, 0)) {
                neighbor.Biome = tile.Biome;
                neighbor.TerrainType = tile.TerrainType;
                oceanCells.Add(neighbor);
            }
            oceanCells.Remove(tile);
        }
    }

    void SetContinentElevation(Biome biome, HashSet<HexCell> continentCells)
    {
        Debug.Log("Setting continent elevation...");
        //var numOfElevationChanges = continentCells.Count * biome.bumpiness * Random.value * 0.15f;
        var numOfElevationChanges = continentCells.Count * biome.bumpiness * 2 * Random.value;
        UnityUtils.Log("Biome {0} (bumpiness {3}): Elevation Chages={1} for {2}", biome, numOfElevationChanges, continentCells.Count, biome.bumpiness);
        for (int i = 0; i < numOfElevationChanges; i++) {
            continentCells.GetRandom().Elevation += 1;
            //    var cellsToChange = continentCells.Count.DividedBy(numOfElevationChanges) * Random.value * 0.15f;
            //    UnityUtils.Log("Cells To Change  {0}", cellsToChange);
            //    var frontierCells = new Queue<HexCell>();
            //    frontierCells.Enqueue(continentCells.GetRandom());
            //    for (int j = 0; j < cellsToChange; j++) {
            //        var nextCell = frontierCells.Dequeue();
            //        nextCell.Elevation += Random.Range(0, 2) == 0 ? -1 : 1;
            //        frontierCells.EnqueueAll(NeighborsInBiome(nextCell, nextCell.Biome));
            //    }
        }
        foreach (var cell in continentCells) {
            var oldEl = cell.Elevation;
            cell.Elevation = Mathf.Clamp(cell.Elevation, biome.elevation.Min - 1, biome.elevation.Max + 1);
            if (oldEl != cell.Elevation) { UnityUtils.Log("Changed {0} to {1}", oldEl, cell.Elevation); }
        }
    }



    static bool LatitudeInRange(HexCell cell, Biome biome)
    {
        var cellLatitude = System.Math.Abs(cell.Latitude * 100);
        return cellLatitude >= biome.latitude.Min && cellLatitude <= biome.latitude.Max;
    }

    static IEnumerable<HexCell> NeighborsInBiome(HexCell cell, int biomeId)
    {
        foreach (var neighbor in cell.GetNeighbors()) {
            if (neighbor.Biome == biomeId) {
                yield return neighbor;
            }
        }
    }
}