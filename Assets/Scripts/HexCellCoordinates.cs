using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
[SerializeField]
public struct HexCellCoordinates
{
    #region Static

    static public readonly Dictionary<EdgeDirection, HexCellCoordinates> OFFSET =
        new Dictionary<EdgeDirection, HexCellCoordinates>() {
            { EdgeDirection.NE, new HexCellCoordinates(0, 1) },
            { EdgeDirection.E, new HexCellCoordinates(1, 0) },
            { EdgeDirection.SE,new HexCellCoordinates(1, -1) },
            { EdgeDirection.SW, new HexCellCoordinates(0, -1) },
            { EdgeDirection.W,new HexCellCoordinates(-1, 0) },
            { EdgeDirection.NW, new HexCellCoordinates(-1, 1) },
        };

    /// <summary>
    /// Converts a column and row position of a hex cell into its hex coordinates. 
    /// </summary>
    /// <returns></returns>
    static public HexCellCoordinates FromOffsetCoordinates(int x, int z)
    {
        return new HexCellCoordinates(x - z / 2, z);
    }

    /// <summary>
    /// The slope of hexagons on a hexagon grid, equal to <code>(√3)/3</code>.
    /// </summary>
    static private readonly double HEX_SLOPE = Math.Sqrt(3) / 3;


    /// <summary>
    /// Converts a cartesian (x, y) position into its corresponding hexagon
    /// coordinate position (x, y, z) using a mathematical formula.
    /// </summary>
    static public HexCellCoordinates FromPosition(Vector3 position)
    {
        int X = 0, Y = 1, Z = 2;
        var SIZE = HexConstants.HEX_CELL_SEPERATION;

        // Compute the hexagon coordinates.
        var coords = new List<double>(new double[] {
            (HEX_SLOPE * position.x - position.z / 3) / SIZE,
           -(HEX_SLOPE * position.x + position.z / 3) / SIZE,
            2d / 3d * position.z / SIZE,
        });
        // Round them to their nearest integer values.
        int[] integers = coords.Select((v) => (int)Math.Round(v)).ToArray();

        // The three coordinates should always sum to zero.
        // If not, a rounding error has occured and must be corrected for.
        if (integers.Sum() != 0) {
            // Correct for rounding errors by finding the coordinate with the
            // highest deviation from a pure integer value, and dropping it,
            // recomputing it from the remaining two dimensions.
            double[] deltas = coords.Select((v, i) => Math.Abs(integers[i] - v)).ToArray();
            int maxDeltaCoordinate = deltas.IndexOf(deltas.Max());
            if (maxDeltaCoordinate == X) { integers[X] = -integers[Y] - integers[Z]; } else if (maxDeltaCoordinate == Z) { integers[Z] = -integers[Y] - integers[X]; }
            // If "Y" has the largest discrepency, ignore it, we are not using Y.
        }

        return new HexCellCoordinates(integers[X], integers[Z]);
    }

    #region Operator Overloads

    static public HexCellCoordinates operator +(HexCellCoordinates c1, HexCellCoordinates c2)
    {
        return new HexCellCoordinates(c1.x + c2.x, c1.z + c2.z);
    }

    static public bool operator ==(HexCellCoordinates c1, HexCellCoordinates c2)
    {
        return c1.Equals(c2);
    }

    static public bool operator !=(HexCellCoordinates c1, HexCellCoordinates c2)
    {
        return !c1.Equals(c2);
    }

    #endregion

    #endregion

    public int X { get { return x; } }
    public int Y { get { return -X - Z; } }
    public int Z { get { return z; } }

    [SerializeField]
    readonly int x;

    [SerializeField]
    readonly int z;

    public HexCellCoordinates(int x, int z)
    {
        this.x = x;
        this.z = z;
    }

    public int DistanceTo(HexCellCoordinates c2)
    {
        int sumOfDiff = Math.Abs(X - c2.X) + Math.Abs(Y - c2.Y) + Math.Abs(Z - c2.Z);
        return sumOfDiff / 2;
    }

    public override string ToString()
    {
        return string.Concat("(", X, ", ", Y, ", ", Z, ")");
    }

    public string ToStringOnSeparateLines()
    {
        return string.Concat(X, "\n", Y, "\n", Z);
    }

    #region HashCode

    public override int GetHashCode()
    {
        return HashUtil.GetHashCodeFromFields(x, z);
    }

    public override bool Equals(System.Object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;
        HexCellCoordinates hc = (HexCellCoordinates)obj;
        return hc.x == x && hc.z == z;
    }

    #endregion
}