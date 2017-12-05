using UnityEngine;
using System.Collections;
using System;

public static class HexConstants
{

    #region Settings

    public const float HEX_SIZE = 10f;

    /// <summary>
    /// <para>The amount of spacing between each <code>HexCell</code>.</para> 
    /// </para>Increase this value to increase the gap between cells.</para>
    /// </summary>
    public const float HEX_CELL_SEPERATION = 1.15f * HEX_SIZE;

    public const float ELEVATION_STEP = 0.35f * HEX_SIZE;

    /// <summary>
    /// The number of HexCells per HexMeshChunk row/column.  Since each chunk
    /// is square in shape, this is equal to the square root of the total number
    /// of cells per chunk.
    /// </summary>
    public const int CELLS_PER_CHUNK_ROW = 5;

    #endregion

    #region Mathmatical Constants

    /// <summary>
    /// <para>
    /// The minimum hex radius as a ratio of its maximum radius, which is equal
    /// to <code>√3/2</code>.
    /// </para>
    /// <para>
    /// A hexagon whose longest radius (i.e. the radius from any of its vertices
    /// to the center) is <code>1</code> will have a minimum radius (i.e. the
    /// radius from the center point of any of its edges to the hexagon center) 
    /// of <code>(√3)/2</code>.
    /// </para>
    /// </summary>
    public static readonly double HEX_RADIUS = Mathf.Sqrt(3) / 2;

    /// <summary>
    /// <para>
    /// The minimum hex diameter as a ratio of its maximum diameter.
    /// </para>
    /// <see cref="HEX_RADIUS"/>
    /// </summary>
    public static readonly double HEX_DIAMETER = HEX_RADIUS * 2;

    #endregion

    #region Derived Values

    //public static readonly double EDGE_RADIUS = HEX_SIZE * HEX_RADIUS;

    ///// <summary>
    ///// Equal to the distance bewtween the center points of any two adjacent
    ///// hex cells.
    ///// </summary>
    //public static readonly double CENTER_DISTANCE =
    //    HEX_CELL_SEPERATION * HEX_DIAMETER;

    ///// <summary>
    ///// The height of each row of HexCells, i.e., the distance from the center
    ///// if any one HexCell to the midpoint of the centers of the two HexCells
    ///// on the row above it.
    ///// </summary>
    //public static readonly double ROW_HEIGHT = Math.Sqrt(
    //    Math.Pow(CENTER_DISTANCE, 2) -
    //    Math.Pow(CENTER_DISTANCE / 2, 2)); //18.75;

    #endregion

}
