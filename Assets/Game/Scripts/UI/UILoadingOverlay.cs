﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace DawnX.UI
{
    /// <summary>
    /// Loading Overlay
    /// This script requires to be attached to a canvas.
    /// The canvas must have the highest sorting order.
    /// The canvas must have a \caster.
    /// The visibility of the overlay is controlled by a CanvasGroup alpha.
    /// </summary>
    [RequireComponent(typeof(Canvas)), RequireComponent(typeof(GraphicRaycaster))]
    public class UILoadingOverlay : MonoBehaviour
    {
        public static UILoadingOverlay Instance {
            get { return _Instance ?? (_Instance = FindObjectOfType<UILoadingOverlay>()); }
        }
        static UILoadingOverlay _Instance;

        enum FadeType { IN, OUT }

        [SerializeField] float fadeDuration;

        [SerializeField] UIProgressBar progressBar;

        [SerializeField] private CanvasGroup canvasGroup;

        [SerializeField] private Text loadStatusText;

        void OnEnable()
        {
            EnsureTopLayer();
            canvasGroup.alpha = 1;
        }

        void EnsureTopLayer()
        {
            Canvas[] canvases = FindObjectsOfType<Canvas>();
            Canvas currentCanvas = gameObject.GetComponent<Canvas>();

            foreach (Canvas canvas in canvases) {
                // Make sure it's not our canvas1
                if (!canvas.Equals(currentCanvas)) {
                    if (canvas.sortingOrder > currentCanvas.sortingOrder)
                        currentCanvas.sortingOrder = canvas.sortingOrder + 1;
                }
            }
        }

        IEnumerator Fade(FadeType fadeType)
        {
            gameObject.SetActive(true);
            float startTime = Time.unscaledTime;
            //int alphaModifier = fadeType == FadeType.IN ? 1 : -1;
            float completion = 0;
            while (completion < 1) {
                completion = (Time.unscaledTime - startTime) / fadeDuration;
                canvasGroup.alpha = 1 - completion;
                yield return null;
            }
            gameObject.SetActive(false);
        }


        internal void UpdateLoad(float completeRatio, string message)
        {
            loadStatusText.text = message;
            UpdateLoad(completeRatio);
        }

        internal void UpdateLoad(float completeRatio)
        {
            progressBar.fillAmount = completeRatio;
            if (completeRatio >= 1) {
                StartCoroutine(Fade(FadeType.OUT));

            }
        }

    }
}