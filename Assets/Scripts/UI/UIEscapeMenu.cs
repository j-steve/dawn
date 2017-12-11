using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DawnX.UI
{
    public class UIEscapeMenu : MonoBehaviour
    {

        public Canvas GuiMenuCanvas;
        public UnityEngine.UI.Button ReturnToGameButton;
        public UnityEngine.UI.Button QuitToDesktopButton;
        public UnityEngine.UI.Button SaveButton;

        // Use this for initialization
        void Start()
        {
            HideMenu();
            ReturnToGameButton.onClick.AddListener(HideMenu);
            QuitToDesktopButton.onClick.AddListener(QuitToDesktop);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetButtonDown("Cancel")) {
                ToggleMenu(!GuiMenuCanvas.gameObject.activeInHierarchy);
            }
        }

        void ToggleMenu(bool doShow)
        {
            GuiMenuCanvas.gameObject.SetActive(doShow);
        }

        void HideMenu()
        {
            ToggleMenu(false);
        }

        void QuitToDesktop()
        {
            Application.Quit();
        }
    }
}
