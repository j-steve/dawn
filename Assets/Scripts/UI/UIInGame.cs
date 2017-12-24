using UnityEngine;
using UnityEngine.UI;

public class UIInGame : MonoBehaviour
{
    public static UIInGame Instance {
        get { return _Instance ?? (_Instance = FindObjectOfType<UIInGame>()); }
    }
    static UIInGame _Instance;

    [SerializeField] GameObject unitInfoPanel;
    [SerializeField] Text unitInfoTitle;
    [SerializeField] Text unitInfoName;
    [SerializeField] Text turnNumber;

    public ISelectable selection { get; private set; }

    void Start()
    {
        unitInfoPanel.SetActive(false);
        // Listen for turn event and increment the turn number.
        GameTime.Instance.GameTurnEvent += (turn) => turnNumber.text = turn.ToString();
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
        // Prevent potential null exception on game termination.
        if (!unitInfoPanel)
            return;

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

    public bool IsSelected(ISelectable target)
    {
        return selection == target;
    }
}

public interface ISelectable
{
    void OnFocus();
    void OnBlur();
    string InGameUITitle { get; }
    string InGameUIDescription { get; }
}