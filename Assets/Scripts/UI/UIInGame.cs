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

    /// <summary>
    /// Sets active In-Game UI, on initialization or after script recompilation. 
    /// </summary>
    void OnEnable()
    {
        ActiveInGameUI = this;
    }

    void Start()
    {
        unitInfoPanel.SetActive(false);
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
        selection = newSelection;
        if (newSelection != null) {
            selection.OnFocus();
            unitInfoTitle.text = newSelection.InGameUITitle;
            unitInfoName.text = newSelection.InGameUIDescription;
        }
    }
}

public interface ISelectable
{
    void OnFocus();
    void OnBlur();
    string InGameUITitle { get; }
    string InGameUIDescription { get; }
}