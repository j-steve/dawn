using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardContainerScript : MonoBehaviour
{
  [SerializeField] CardScript cardPrefab;
  [SerializeField] Text cardContainerTitle;
  [SerializeField] GridLayoutGroup gridLayoutGroup;

  // Start is called before the first frame update
  void Start()
    {
    cardContainerTitle.text = "Some Title";
    //foreach (CardScript cardScript in gameObject.GetComponentsInChildren<CardScript>()) {
    //  cardScript.gameObject.SetActive(false);
    //}
    createCard("Some Card", "fakeo");
    createCard("Some Card2", "fakeo2");
    cardPrefab.gameObject.SetActive(false);
  }

    // Update is called once per frame
    void Update()
    {
        
    }


  public  void createCard( string title, string description)
  {
    CardScript cardScript = Instantiate(cardPrefab, gridLayoutGroup.transform);
    cardScript.cardTitle.text = title;
    cardScript.cardDescription.text = description;
  }
}
