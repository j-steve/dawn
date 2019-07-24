using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OneResourcePanel : MonoBehaviour
{
    [SerializeField] Image icon = null;
    [SerializeField] Text resourceName = null;
    [SerializeField] Text resourceQuantity = null;
    [SerializeField] Text resourceIncrease = null;

    public void Initialize(ResourceType resourceType, float quantity, float increase)
    {
        this.resourceName.text = resourceType.name;
        this.icon.sprite = resourceType.icon;
        this.resourceQuantity.text = quantity == 1000 ? "∞" : Mathf.Round(quantity).ToString();
        this.resourceIncrease.text = quantity == 1000 ? ""  : "   (+" + Mathf.Round(increase).ToString() + ")";
    }
}
