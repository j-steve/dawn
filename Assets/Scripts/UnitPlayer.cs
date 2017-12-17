using UnityEngine;
using System.Collections;

public class UnitPlayer : Unit
{
    public override void Select()
    {
        base.Select();
        HexBoard.ActiveBoard.HexCellClickedEvent += OnHexCellClick;
    }

    void OnHexCellClick(HexCellClickedEventArgs e)
    {
        Debug.LogFormat("{0} knows you clicked {1}", UnitName, e.Cell);
    }

    protected override void onBlur()
    {
        HexBoard.ActiveBoard.HexCellClickedEvent -= OnHexCellClick;
    }
}
