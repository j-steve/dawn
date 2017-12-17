using System;
using UnityEngine;
using UnityEngine.UI;

public class UIInGame : MonoBehaviour
{
    public static UIInGame ActiveInGameUI;

    [SerializeField] GameObject unitInfoPanel;
    [SerializeField] Text unitInfoTitle;
    [SerializeField] Text unitInfoName;

    Action onBlur;

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
        BlurSelected();
        unitInfoPanel.SetActive(true);
        unitInfoTitle.text = title;
        unitInfoName.text = description;
    }

    public void ShowUI(string title, string description, Action onBlur)
    {
        ShowUI(title, description);
        this.onBlur = onBlur;
    }

    public void HideUI()
    {
        BlurSelected();
        unitInfoPanel.SetActive(false);
    }

    void BlurSelected()
    {
        if (onBlur != null) {
            onBlur();
            onBlur = null;
        }
    }

}
