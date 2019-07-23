using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using DawnX.UI;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class HexBoardGenerator
{
    private HexBoard hexBoard;

    public HexBoardGenerator(HexBoard hexBoard)
    {
        this.hexBoard = hexBoard;
    }

    public IEnumerator CreateMap()
    {
        hexBoard.hexCells.Clear();
        UILoadingOverlay.Instance.UpdateLoad(.1f, "Generating mesh chunks...");
        yield return null;
        var hexChunks = GetHexMeshChunks();
        UILoadingOverlay.Instance.UpdateLoad(.25f, "Generating terrain...");
        yield return null;
        GenerateTerrain();
        UILoadingOverlay.Instance.UpdateLoad(.4f, "Triangulating Cells...");
        yield return null;
        // Triangulate each HexMeshChunk to make the map visible.
        var completion = .4f;
        var chunkValue = (.8f - completion) / hexChunks.Count;
        foreach (var entry in hexChunks) {
            entry.Key.Triangulate(entry.Value);
            completion += chunkValue;
            UILoadingOverlay.Instance.UpdateLoad(completion);
            yield return null;
        }
        UILoadingOverlay.Instance.UpdateLoad(.8f, "Planting forests...");
        yield return null;
        foreach (HexCell cell in hexBoard.hexCells.Values) {
            cell.tileType = TileType.GetForBiome(cell.Biome);
            if (cell.tileType == null) { continue; } // TODO: Delete this line once all biomes have valid tile types.
            var trees = Resources.LoadAll<GameObject>("Trees/" + cell.Biome.name);
            for (int i = 0; i < cell.tileType.treeCount; i++) {
                var offset = UnityExtensions.RandomPointOnCircle() * HexConstants.HEX_SIZE * .6f;  // Trees appear 40% from edge of hex.
                var spawn = cell.transform.position + new Vector3(offset.x, 0, offset.y);
                var tree = Object.Instantiate(trees.GetRandom(), spawn, Quaternion.identity, cell.transform);
                tree.transform.localScale = tree.transform.localScale.ScaledBy(cell.Biome.treeSizeModifier);
                if (Random.value > cell.Biome.treeProbability) { break; }
            }
        }
        UILoadingOverlay.Instance.UpdateLoad(.9f, "Moosifying...");
        yield return null;
        foreach (HexCell cell in hexBoard.hexCells.Values) {
            if (Random.value <= cell.Biome.animalProbability) {
                var animalType = AnimalType.GetForBiome(cell.Biome);
                if (animalType == null) { Debug.LogWarningFormat("Can't spawn animals on {0}", cell.Biome); continue; }
                UnitAnimal.Create(cell, animalType);
            }
        }
        //var spawnableTiles = new HashSet<HexCell>(hexBoard.hexCells.Values.Where(c => c.Elevation > 0 && c.GetNeighbors().FirstOrDefault(n => n.Elevation == 0) == null));
        //int desiredUnitCount = spawnableTiles.Count / 10;
        //var unitCount = 0;
        //while (spawnableTiles.Count > 0 && unitCount < 15) {
        //    var cell = spawnableTiles.GetRandom();
        //    var prefab = hexBoard.unitPrefabs.GetRandom();
        //    if (prefab.preferredBiomes.Contains(cell.Biome.name)) {
        //        Unit.Create(prefab, cell);
        //        unitCount++;
        //    }
        //    spawnableTiles.Remove(cell);
        //}
        // Create the human player's starting position.
        var humanSpawnableTiles = 
            new HashSet<HexCell>(hexBoard.hexCells.Values.Where(c => c.Elevation > 0 && c.units.Count == 0));
        var startTile = humanSpawnableTiles.GetRandom();
        var playerUnit = Unit.Create(hexBoard.playerPrefab, startTile);
        MapCamera.Active.CenterCameraOn(startTile);

        UILoadingOverlay.Instance.UpdateLoad(1);
    }

    public IEnumerator LoadMap(string path)
    {
        using (var reader = new BinaryReader(File.Open(path, FileMode.Open))) {
            hexBoard.mapSize.Height = reader.ReadInt16();
            hexBoard.mapSize.Width = reader.ReadInt16();
            hexBoard.hexCells.Clear();
            UILoadingOverlay.Instance.UpdateLoad(.1f, "Generating mesh chunks...");
            yield return null;
            var hexChunks = GetHexMeshChunks();

            foreach (var cell in hexBoard.hexCells.Values) {
                cell.Load(reader);
            }
            UILoadingOverlay.Instance.UpdateLoad(.4f, "Triangulating Cells...");
            yield return null;
            // Triangulate each HexMeshChunk to make the map visible.
            var completion = .4f;
            var chunkValue = (.8f - completion) / hexChunks.Count;
            foreach (var entry in hexChunks) {
                entry.Key.Triangulate(entry.Value);
                completion += chunkValue;
                UILoadingOverlay.Instance.UpdateLoad(completion);
                yield return null;
            }
            UILoadingOverlay.Instance.UpdateLoad(1);
        }
    }

    Dictionary<HexChunk, IEnumerable<HexCell>> GetHexMeshChunks()
    {
        var chunks = new Dictionary<HexChunk, IEnumerable<HexCell>>();
        for (int row = 0; row < hexBoard.mapSize.Height; row++) {
            for (int column = 0; column < hexBoard.mapSize.Width; column++) {
                var chunk = HexChunk.Create(hexBoard.hexChunkPrefab, row, column);
                chunks[chunk] = GetChunkHexCells(chunk);
            }
        }
        return chunks;
    }

    IEnumerable<HexCell> GetChunkHexCells(HexChunk chunk)
    {
        var chunkCells = new List<HexCell>();
        int rowStart = chunk.row * HexConstants.CELLS_PER_CHUNK_ROW;
        int rowEnd = rowStart + HexConstants.CELLS_PER_CHUNK_ROW;
        int colStart = chunk.column * HexConstants.CELLS_PER_CHUNK_ROW;
        int colEnd = colStart + HexConstants.CELLS_PER_CHUNK_ROW;
        for (int row = rowStart; row < rowEnd; row++) {
            for (int column = colStart; column < colEnd; column++) {
                var cell = HexCell.Create(hexBoard.hexCellPrefab, chunk, row, column);
                hexBoard.hexCells[cell.Coordinates] = cell;
                chunkCells.Add(cell);
            }
        }
        return chunkCells;
    }

    void GenerateTerrain()
    {
        int continents = Mathf.FloorToInt(hexBoard.continentsPerChunk * hexBoard.mapSize.Area);
        var oceanCells = new HashSet<HexCell>();
        var blankCells = new HashSet<HexCell>(hexBoard.hexCells.Values);
        for (int continent = 0; continent < continents; continent++) {
            if (blankCells.Count == 0) { break; }
            var biome = Biome.Values.GetRandom();
            var potentialCells = blankCells.Where(c => LatitudeInRange(c, biome));
            if (potentialCells.Count() == 0) { continue; }
            var firstCell = potentialCells.GetRandom();
            int biomeMinSize = Mathf.RoundToInt(biome.minSize * hexBoard.biomeMinSize);
            int targetSize = Random.Range(biomeMinSize, Mathf.RoundToInt(biomeMinSize * hexBoard.biomeMaxSize + 1));
            var continentCells = new HashSet<HexCell>() { firstCell };
            var frontierCells = new HashSet<HexCell>(NeighborsInBiome(firstCell, 0));
            while (continentCells.Count < targetSize && frontierCells.Count > 0 && blankCells.Count > 0) {
                HexCell nextCell = frontierCells.GetRandom();
                if (blankCells.Remove(nextCell)) {
                    continentCells.Add(nextCell);
                    var neighbors = nextCell.GetNeighbors().Where(c => c.BiomeNumber == 0);
                    frontierCells.UnionWith(neighbors);
                }
                frontierCells.Remove(nextCell);
            }
            if (continentCells.Count >= biome.minSize) {
                foreach (HexCell cell in continentCells) {
                    cell.Biome = biome;
                    cell.BiomeNumber = continent + 1;
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
        while (oceanCells.Count > 0) {
            var tile = oceanCells.First();
            foreach (HexCell neighbor in NeighborsInBiome(tile, 0)) {
                neighbor.BiomeNumber = tile.BiomeNumber;
                neighbor.Biome = tile.Biome;
                oceanCells.Add(neighbor);
            }
            oceanCells.Remove(tile);
        }
    }

    void SetContinentElevation(Biome biome, HashSet<HexCell> continentCells)
    {
        var numOfElevationChanges = continentCells.Count * biome.bumpiness * 2 * Random.value;
        for (int i = 0; i < numOfElevationChanges; i++) {
            continentCells.GetRandom().Elevation += 1;
        }
        foreach (var cell in continentCells) {
            var oldEl = cell.Elevation;
            cell.Elevation = Mathf.Clamp(cell.Elevation, biome.elevation.Min - 1, biome.elevation.Max + 1);
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
            if (neighbor.BiomeNumber == biomeId) {
                yield return neighbor;
            }
        }
    }
}