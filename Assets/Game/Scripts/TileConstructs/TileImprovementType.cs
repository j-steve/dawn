using UnityEngine;
using System.Collections;

public class TileImprovementType: EnumClass
{
    static public readonly TileImprovementType
        LumberCamp = new TileImprovementType("Lumber Camp"),
        Mine = new TileImprovementType("Mine");
    
    private TileImprovementType(string name) : base(name) { }
}
