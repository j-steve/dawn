using UnityEngine;
using System.Collections;
using System.Collections.Generic;

static public class UnitMaster
{
    static readonly MultiDict<string, Unit> units = new MultiDict<string, Unit>();

    public static IEnumerator TakeAITurn()
    {
        foreach (var type in units.Keys) {
            foreach (var unit in units[type]) {
                unit.TakeTurn();
            }
            yield return null;
        }
    }

    public static void AddUnit(Unit unit)
    {
        units.Add(unit.UnitName, unit);
    }

}
