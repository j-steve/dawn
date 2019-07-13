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
    [SerializeField] Button createVillage;
    [SerializeField] Dialog createVillageDialog;

    string turnNumberFormat;

    public ISelectable selection { get; private set; }

    void Start()
    {
        // Hide the selection panel, it should only appear when something is selected.
        unitInfoPanel.SetActive(false);
        // Listen for turn event and increment the turn number.
        turnNumberFormat = turnNumber.text;
        GameTime.Instance.GameTurnEvent += (GameDate date) => {turnNumber.text = turnNumberFormat.Format(date.year, date.season, date.day);};
        // Listen for "create village" button click.
        createVillage.onClick.AddListener(() => {
            if (selection.GetType() == typeof(UnitPlayer)) {
                createVillageDialog.Show(() => { ((UnitPlayer)selection).CreateVillage(); });
            } else {
                Debug.LogErrorFormat("Cannot create village, active selection is {0}.", selection);
            }
        });
    }

    void Update()
    {
        if (selection != null) {
            labelTitle.text = selection.InfoPanelTitle;
            labelDescription.text = selection.InfoPanelDescription;
            labelDetails.text = selection.InfoPanelDetails;
            createVillage.gameObject.SetActive(selection.GetType() == typeof(UnitPlayer));
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