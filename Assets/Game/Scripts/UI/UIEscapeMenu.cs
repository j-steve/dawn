using UnityEngine;

namespace DawnX.UI
{
    public class UIEscapeMenu : MonoBehaviour
    {
        public static UIEscapeMenu Instance {
            get { return _Instance ?? (_Instance = FindObjectOfType<UIEscapeMenu>()); }
        }
        static UIEscapeMenu _Instance;

        public Canvas GuiMenuCanvas;
        public UnityEngine.UI.Button ReturnToGameButton;
        public UnityEngine.UI.Button QuitToDesktopButton;
        public UnityEngine.UI.Button SaveButton;
        public UnityEngine.UI.Button LoadButton;

        // Use this for initialization
        void Start()
        {
            HideMenu();
            ReturnToGameButton.onClick.AddListener(HideMenu);
            QuitToDesktopButton.onClick.AddListener(QuitToDesktop);
            SaveButton.onClick.AddListener(Game.Save);
            LoadButton.onClick.AddListener(Game.Load);
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
            Game.Paused = doShow;
            GuiMenuCanvas.gameObject.SetActive(doShow);
        }

        public void HideMenu()
        {
            ToggleMenu(false);
        }

        void QuitToDesktop()
        {
            Application.Quit();
        }
    }
}
