using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardContainerScript : MonoBehaviour
{
    [SerializeField] CardScript cardPrefab;
    [SerializeField] Text cardContainerTitle;
    [SerializeField] GridLayoutGroup gridLayoutGroup;
    [SerializeField] public Button okButton;
    [SerializeField] public Button cancelButton;
    public CardScript selectedCard = null;

    List<CardScript> cardOptions = new List<CardScript>();

    // Start is called before the first frame update
    void Start()
    {
        cardPrefab.gameObject.SetActive(false);
        cancelButton.onClick.AddListener(this.Hide);
        okButton.onClick.AddListener(this.Hide);
    }


    public void Show(string title)
    {
        gameObject.SetActive(true);
        cardContainerTitle.text = title;
        okButton.interactable = false;
    }

    public void Hide()
    {
        selectedCard = null;
        gameObject.SetActive(false);
        foreach(CardScript cardScript in cardOptions) {
            Destroy(cardScript.gameObject);
        }
        cardOptions.Clear();
    }

    public void AddCard(string title, string description)
    {
        CardScript cardScript = Instantiate(cardPrefab, gridLayoutGroup.transform);
        cardScript.cardTitle.text = title;
        cardScript.cardDescription.text = description;
        cardScript.gameObject.SetActive(true); // TODO: why is this necessary?
        cardScript.cardButton.onClick.AddListener(delegate () {
            okButton.interactable = true;
            cardScript.cardBackground.color = Color.yellow;
            selectedCard = cardScript;
        });
        cardOptions.Add(cardScript);
    }

}
