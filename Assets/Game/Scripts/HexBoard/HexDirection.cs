﻿using System.Collections.Generic;
using UnityEngine;

public abstract class HexDirection : EnumClass
{
    protected HexDirection(string name) : base(name) { }
}

public class EdgeDirection : HexDirection
{
    static public readonly EdgeDirection
        NE = new EdgeDirection("NE"),
        E = new EdgeDirection("E"),
        SE = new EdgeDirection("SE"),
        SW = new EdgeDirection("SW"),
        W = new EdgeDirection("W"),
        NW = new EdgeDirection("NW");

    static public readonly EdgeDirection[] Values =
        new EdgeDirection[] { NE, E, SE, SW, W, NW };

    private EdgeDirection(string name) : base(name) { }

    public VertexDirection vertex1 {
        get {
            return FromValue<VertexDirection>(Value);
        }
    }
    public VertexDirection vertex2 {
        get {
            return FromValue<VertexDirection>((Value + 1) % 6);
        }
    }
}

public class VertexDirection : HexDirection
{
    public static readonly VertexDirection
        N = new VertexDirection("N"),
        ENE = new VertexDirection("ENE"),
        ESE = new VertexDirection("ESE"),
        S = new VertexDirection("S"),
        WNW = new VertexDirection("WNW"),
        WSW = new VertexDirection("WSW");

    private VertexDirection(string name) : base(name) { }
}


public static class HexDirectionExtensions
{
    /// <summary>
    /// Returns the opposite direction (e.g. E -> W).
    /// </summary>
    static public T Opposite<T>(this T direction) where T : HexDirection
    {
        return EnumClass.FromValue<T>((direction.Value + 3) % 6);
    }

    /// <summary>
    /// Returns the adjacent direction (clockwise).
    /// </summary>
    static public T Next<T>(this T direction) where T : HexDirection
    {
        return EnumClass.FromValue<T>((direction.Value + 1) % 6);
    }

    /// <summary>
    /// Returns the adjacent dirction (counterclockwise).
    /// </summary>
    static public T Previous<T>(this T direction) where T : HexDirection
    {
        return EnumClass.FromValue<T>(MathUtils.Mod(direction.Value - 1, 6));
    }

}