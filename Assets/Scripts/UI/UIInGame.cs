using System;
using UnityEngine;
using UnityEngine.UI;

public class UIInGame : MonoBehaviour
{
    public static UIInGame ActiveInGameUI;

    [SerializeField] GameObject unitInfoPanel;
    [SerializeField] Text unitInfoTitle;
    [SerializeField] Text unitInfoName;

    public ISelectable selection { get; private set; }

    // Use this for initialization
    void Start()
    {
        ActiveInGameUI = this;
        HideUI();
    }

    void OnEnable()
    {
        ActiveInGameUI = this;
    }

    void Update()
    {
        if (selection != null) {
            unitInfoTitle.text = selection.InGameUITitle;
            unitInfoName.text = selection.InGameUIDescription;
        }
    }

    public void SetSelected(ISelectable newSelection)
    {
        if (selection != null) {
            selection.OnBlur();
        }
        unitInfoPanel.SetActive(newSelection != null);
        if (newSelection != null) {
            selection = newSelection;
            selection.OnFocus();
            unitInfoTitle.text = newSelection.InGameUITitle;
            unitInfoName.text = newSelection.InGameUIDescription;
        }
    }

    public void HideUI()
    {
        unitInfoPanel.SetActive(false);
    }
}

public interface ISelectable
{
    void OnFocus();
    void OnBlur();
    string InGameUITitle { get; }
    string InGameUIDescription { get; }
}