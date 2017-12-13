using UnityEngine;
using UnityEngine.UI;

public class UIInGame : MonoBehaviour
{
    public static UIInGame ActiveInGameUI;

    [SerializeField] GameObject unitInfoPanel;
    [SerializeField] Text unitInfoName;

    // Use this for initialization
    void Start()
    {
        ActiveInGameUI = this;
        HideUI();
    }

    void OnEnable()
    {
        ActiveInGameUI = this;
        HideUI();
    }

    public void ShowUI(string unitName)
    {
        unitInfoPanel.SetActive(true);
        unitInfoName.text = unitName;
    }

    public void HideUI()
    {
        unitInfoPanel.SetActive(false);
    }

}
