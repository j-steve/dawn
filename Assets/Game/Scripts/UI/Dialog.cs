using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class Dialog : MonoBehaviour
{
    [SerializeField] Button buttonCancel;
    [SerializeField] Button buttonSubmit;
    Action onSubmitCallback;
    
    void Start()
    {
        gameObject.SetActive(false);
        buttonSubmit.onClick.AddListener(onSubmitClick);
        buttonCancel.onClick.AddListener(() => { gameObject.SetActive(false); });
    }

    void onSubmitClick()
    {
        gameObject.SetActive(false);
        if (onSubmitCallback != null) {
            onSubmitCallback();
        }
    }

    public void Show(Action onSubmit = null)
    {
        onSubmitCallback = onSubmit;
        gameObject.SetActive(true);
    }

}
