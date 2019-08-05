using UnityEngine;
using System.Collections;

public class Village : MonoBehaviour, ISelectable
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
        village.Population = 5;
        // Eliminate the unit which created this village..
        Destroy(unit.gameObject);
        // Set the village to "Selected" state.
        InGameUI.Instance.SetSelected(village);
        return village;
    }

    public string Name { get; private set; } 
    public float Population { get; private set; }

    void OnMouseDown() {
        bool isSelected = InGameUI.Instance.IsSelected(this);
        InGameUI.Instance.SetSelected(isSelected ? null : this);
    }
    
    #region ISelectable

    void ISelectable.OnFocus(InGameUI ui)
    {
        GetComponent<Renderer>().material.color = Color.green;
        ui.villagePanel.SetActive(true);
    }

    void ISelectable.OnBlur(InGameUI ui)
    {
        GetComponent<Renderer>().material.color = Color.red;
        ui.villagePanel.SetActive(false);
    }

    void ISelectable.OnUpdateWhileSelected(InGameUI ui)
    {
        ui.labelVillageName.text = Name;
        ui.labelVillagePop.text = ui.villagePopFormat.Format(Population);
    }

    #endregion
}
