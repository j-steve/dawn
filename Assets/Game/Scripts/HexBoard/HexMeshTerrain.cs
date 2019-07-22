﻿
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HexMeshTerrain : HexMesh
{
    [NonSerialized] public List<Vector3> terrainTypes = new List<Vector3>();

    private MeshCollider meshCollider;

    protected override void Awake()
    {
        base.Awake();
        meshCollider = gameObject.AddComponent<MeshCollider>();
        meshCollider.convex = true;
        meshCollider.isTrigger = true;
    }

    public override void Clear()
    {
        base.Clear();
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
        if (!EventSystem.current.IsPointerOverGameObject()) { 
            HexBoard.Active.OnMapClick();
        }
    }

    public void AddQuadWithTerrain(TexturedEdge e1, TexturedEdge e2)
    {
        AddQuad(e1.vertex2, e2.vertex2, e2.vertex1, e1.vertex1);
        AddColors(Color.red, Color.green, Color.green, Color.red);
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