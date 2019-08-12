
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HexMeshTerrain : HexMesh
{
    [NonSerialized] public List<Vector3> terrainTypes = new List<Vector3>();

    private MeshCollider meshCollider;

    protected override void Initialize()
    {
        base.Initialize();
        meshCollider = gameObject.AddComponent<MeshCollider>();
        meshCollider.convex = true;
        meshCollider.isTrigger = true;
    }

    public override void InitOrReset()
    {
        base.InitOrReset();
        terrainTypes.Clear();
    }

    public override void Apply()
    {

        base.Apply();
        mesh.SetUVs(2, terrainTypes);
        meshCollider.sharedMesh = mesh;
    }

    void OnMouseDown()
    {
        if (!EventSystem.current.IsPointerOverGameObject()) { // Ensures cursor is not pointed at UI element.
            HexBoard.Active.OnMapClick();
        }
    }

    public void AddQuadWithTerrain(TexturedEdge e1, TexturedEdge e2)
    {
        AddQuad(e1, e2);
        AddColors(Color.red, Color.red, Color.green, Color.green);
        AddTerrainType(e1.texture, e2.texture, 0, 4);
    }

    public void AddTerrainType(
        TerrainTexture redChannel, TerrainTexture blueChannel,
        TerrainTexture greenChannel, int count)
    {
        var terrainType =
            new Vector3((int)redChannel, (int)blueChannel, (int)greenChannel);
        for (int i = 0; i < count; i++) {
            terrainTypes.Add(terrainType);
        }
    }
}