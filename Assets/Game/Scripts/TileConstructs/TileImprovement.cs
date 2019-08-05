using UnityEngine;
using System.Collections;

public class TileImprovement : MonoBehaviour, ITileCenterConstruct
{
    static public TileImprovement CreateTileImprovement(HexCell cell, TileImprovementType tileImprovementType)
    {
        // Create cube object.
        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.GetComponent<Renderer>().material.color = Color.blue;
        cube.transform.localScale *= 2;
        cube.transform.parent = cell.transform;
        var newpos = cell.transform.position;
        newpos.y += cube.transform.localScale.y / 2;
        cube.transform.position = newpos;
        // Add box collider.
        var collider = cube.AddComponent<BoxCollider>();
        collider.isTrigger = true;
        collider.transform.position = cube.transform.position;
        collider.transform.localScale = cube.transform.localScale;
        // Add tile improvement info.
        var tileImprovement = cube.AddComponent<TileImprovement>();
        tileImprovement.Type = tileImprovementType;
        // Add the tile improvement to the tile it occupies.
        cell.tileConstruct = tileImprovement;
        return tileImprovement;
    }

    public TileImprovementType Type { get; private set; }

    string ITileConstruct.Name { get { return Type.Name; } }
}
