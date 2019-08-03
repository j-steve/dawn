using System;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    static public InGameUI Instance {
        get { return _Instance ?? (_Instance = FindObjectOfType<InGameUI>()); }
    }
    static InGameUI _Instance;

    [SerializeField] GameObject selectionInfoPanel = null;
    [SerializeField] Text labelTitle = null;
    [SerializeField] Text labelDescription = null;
    [SerializeField] Text labelDetails = null;
    [SerializeField] Text turnNumber = null;
    [SerializeField] Button createVillage = null;
    [SerializeField] Dialog createVillageDialog = null;
    [SerializeField] InputField createVillageName = null;
    [SerializeField] GameObject villagePanel = null;
    [SerializeField] Text labelVillageName = null;
    [SerializeField] Text labelVillagePop = null;
    [SerializeField] GameObject resourcePanel = null;
    [SerializeField] OneResourcePanel oneResourcePrefab = null;

    string turnNumberFormat;
    string villagePopFormat;

    public ISelectable selection { get; private set; }

    public event Action SelectionChanged;

    void Start()
    {
        // Hide the selection panel, it should only appear when something is selected.
        selectionInfoPanel.SetActive(false);
        HideVillageUI();
        // Listen for turn event and increment the turn number.
        turnNumberFormat = turnNumber.text;
        villagePopFormat = labelVillagePop.text;
        GameTime.Instance.GameTurnEvent += (GameDate date) => {turnNumber.text = turnNumberFormat.Format(date.year, date.season, date.day);};
        // Listen for "create village" button click.
        createVillage.onClick.AddListener(() => {
            Debug.LogWarning("Clicked");
            if (selection.GetType() == typeof(UnitPlayer)) {
                createVillageDialog.Show(() => { Village.CreateVillage((UnitPlayer)selection, createVillageName.text); });
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
        resourcePanel.SetActive(false);
        foreach (var obj in resourcePanel.GetComponentsInChildren<OneResourcePanel>()) { Destroy(obj.gameObject); }
        if (!selectionInfoPanel) { return; /* Prevent potential null exception on game termination. */ }

        if (SelectionChanged != null) { SelectionChanged.Invoke(); }
        villagePanel.SetActive(false);
        if (selection != null) {
            selection.OnBlur();
        }
        selectionInfoPanel.SetActive(newSelection != null);
        selection = newSelection;
        if (newSelection != null) {
            selection.OnFocus();
            labelTitle.text = newSelection.InfoPanelTitle;
            labelDescription.text = newSelection.InfoPanelDescription;
            if (newSelection.GetType() == typeof(HexCell)) {
                resourcePanel.SetActive(true);
                var cell = (HexCell)newSelection;
                if (cell.tileType == null) { return;  } //TODO: Remove this once all biomes have valid tile types.
                foreach(var resource in cell.tileType.resources) {
                    var oneResourcePanel = Instantiate(oneResourcePrefab, resourcePanel.transform);
                    oneResourcePanel.Initialize(resource.Key, resource.Value.quantity, resource.Value.regenRate);
                }
            }
        }
    }

    public void ShowVillageUI(Village village)
    {
        SetSelected(null);
        villagePanel.SetActive(true);
        labelVillageName.text = village.Name;
        labelVillagePop.text = villagePopFormat.Format(5);

    }

    public void HideVillageUI()
    { 
        villagePanel.SetActive(false);

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