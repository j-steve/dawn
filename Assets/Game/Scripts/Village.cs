using UnityEngine;
using System.Collections;

public class Village : MonoBehaviour
{
    static public Village CreateVillage(Unit unit, string villageName)
    {
        // Create cube object.
        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.GetComponent<Renderer>().material.color = Color.red;
        cube.transform.localScale *= 5;
        cube.transform.parent = unit.Location.transform;
        var newpos = unit.Location.transform.position;
        newpos.y += cube.transform.localScale.y / 2;
        cube.transform.position = newpos;
        // Add box collider.
        var collider = cube.AddComponent<BoxCollider>();
        collider.isTrigger = true;
        collider.transform.position = cube.transform.position;
        collider.transform.localScale = cube.transform.localScale;
        // Add village info.
        var village = cube.AddComponent<Village>();
        village.Name = villageName;
        village.Select();
        // Eliminate the unit which created this village..
        Destroy(unit.gameObject);
        return village;
    }

    public string Name { get; private set; }
    private bool isSelected = false;

    void OnMouseDown() {
        if (isSelected) { UnSelect(); } else { Select(); }
    }

    void Select()
    {
        isSelected = true;
        GetComponent<Renderer>().material.color = Color.green;
        InGameUI.Instance.ShowVillageUI(this);
        InGameUI.Instance.SelectionChanged += UnSelect;
    }

    void UnSelect()
    {
        isSelected = false;
        GetComponent<Renderer>().material.color = Color.red;
        InGameUI.Instance.HideVillageUI();
        InGameUI.Instance.SelectionChanged -= UnSelect;
    }

    public void OnFocus()
    {
        throw new System.NotImplementedException();
    }

    public void OnBlur()
    {
        throw new System.NotImplementedException();
    }
}
