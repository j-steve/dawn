using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class CardScript : MonoBehaviour
{

    [SerializeField] public Text cardTitle;
    [SerializeField] public Text cardDescription;
    [SerializeField] public Sprite cardImage;
    [SerializeField] public Image cardBackground;
    [SerializeField] public Button cardButton;

    // Update is called once per frame
    void Update()
    {

    }
}