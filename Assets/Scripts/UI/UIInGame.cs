using UnityEngine;
using UnityEngine.UI;

public class UIInGame : MonoBehaviour
{
    public static UIInGame ActiveInGameUI;

    [SerializeField] GameObject unitInfoPanel;
    [SerializeField] Text unitInfoTitle;
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

    public void ShowUI(string title, string description)
    {
        unitInfoPanel.SetActive(true);
        unitInfoTitle.text = title;
        unitInfoName.text = description;
    }

    public void HideUI()
    {
        unitInfoPanel.SetActive(false);
    }

}
