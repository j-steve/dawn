using UnityEngine;
using UnityEngine.UI;

public class SelectionInfoPanel : MonoBehaviour
{
    static public SelectionInfoPanel Instance {
        get { return _Instance ?? (_Instance = FindObjectOfType<SelectionInfoPanel>()); }
    }
    static SelectionInfoPanel _Instance;

    [SerializeField] GameObject unitInfoPanel;
    [SerializeField] Text labelTitle;
    [SerializeField] Text labelDescription;
    [SerializeField] Text labelDetails;
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
            labelTitle.text = selection.InfoPanelTitle;
            labelDescription.text = selection.InfoPanelDescription;
            labelDetails.text = selection.InfoPanelDetails;
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
            labelTitle.text = newSelection.InfoPanelTitle;
            labelDescription.text = newSelection.InfoPanelDescription;
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
    string InfoPanelTitle { get; }
    string InfoPanelDescription { get; }
    string InfoPanelDetails { get; }
}