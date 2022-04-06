using System;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    static public InGameUI Instance {
        get { return _Instance ?? (_Instance = FindObjectOfType<InGameUI>()); }
    }
    static InGameUI _Instance;

    [SerializeField] public GameObject selectionInfoPanel = null;
    [SerializeField] public Text labelTitle = null;
    [SerializeField] public Text labelDescription = null;
    [SerializeField] public Text labelDetails = null;
    [SerializeField] public Text turnNumber = null;
    [SerializeField] public Button createVillageButton = null;
    [SerializeField] public Button addTileBuildingButton = null;
    [SerializeField] public Dialog createVillageDialog = null;
    [SerializeField] public InputField createVillageName = null;
    [SerializeField] public GameObject villagePanel = null;
    [SerializeField] public Text labelVillageName = null;
    [SerializeField] public Text labelVillagePop = null;
    [SerializeField] public GameObject resourcePanel = null;
    [SerializeField] public OneResourcePanel oneResourcePrefab = null;
    [SerializeField] public CardContainerScript cardContainer = null;

    string turnNumberFormat;
    public string villagePopFormat;

    public ISelectable selection { get; private set; }

    void Start()
    {
        // Store initial label text as format patterns.
        turnNumberFormat = turnNumber.text;
        villagePopFormat = labelVillagePop.text;
        // Set the base initial display with no selection.
        selectionInfoPanel.SetActive(false);
        resourcePanel.SetActive(false);
        villagePanel.SetActive(false);
        createVillageButton.gameObject.SetActive(false);
        addTileBuildingButton.gameObject.SetActive(false);
        // Listen for turn event and increment the turn number.
        GameTime.Instance.GameTurnEvent += (GameDate date) => { turnNumber.text = turnNumberFormat.Format(date.year, date.season, date.day); };
    }

    void Update()
    {
        if (selection != null) {
            selection.OnUpdateWhileSelected(this);
        }
    }

    public void SetSelected(ISelectable newSelection)
    {
        if (!selectionInfoPanel) { return; /* Prevent potential null exception on game termination. */ }

        if (selection != null) {
            selection.OnBlur(this);
        }
        selection = newSelection;
        if (newSelection != null) {
            selection.OnFocus(this);
            selection.OnUpdateWhileSelected(this); // Set initial properties.
        }
    }

    public bool IsSelected(ISelectable target)
    {
        return selection == target;
    }
}

public interface ISelectable
{
    /// <summary>
    /// Triggered when this entity first becomes selected by the player.  
    /// Should set up the requisite UI display and update the entity gameobject itself to display as selected.
    /// </summary>
    void OnFocus(InGameUI ui);

    /// <summary>
    /// Triggered when this entity was previously selected but is becoming unselected, either because the user
    /// has selected a different entity or because they have unselected this entity.  
    /// Should revert any changes made in the <see cref="OnFocus(InGameUI)">OnFocus</see> call to restore the
    /// entity and the UI to their un-selected states.
    /// </summary>
    void OnBlur(InGameUI ui);

    /// <summary>
    /// Triggered during Unity "Update" calls for the entity which is currently selected, if any.
    /// Should update any dynamic UI elements which are based on this entity (e.g. a unit's health display).
    /// </summary>
    void OnUpdateWhileSelected(InGameUI ui);
}