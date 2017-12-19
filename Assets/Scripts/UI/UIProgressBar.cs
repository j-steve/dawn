using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace DawnX.UI
{
    [AddComponentMenu("UI/Bars/Progress Bar")]
    public class UIProgressBar : MonoBehaviour
    {
        [Serializable] public class ChangeEvent : UnityEvent<float> { }

        public enum Type
        {
            Filled,
            Resize,
            Sprites
        }

        public enum FillSizing
        {
            Parent,
            Fixed
        }

        [SerializeField] private Type m_Type = Type.Filled;
        [SerializeField] private Image m_TargetImage;
        [SerializeField] private Sprite[] m_Sprites;
        [SerializeField] private RectTransform m_TargetTransform;
        [SerializeField] private FillSizing m_FillSizing = FillSizing.Parent;
        [SerializeField] private float m_MinWidth = 0f;
        [SerializeField] private float m_MaxWidth = 100f;
        [SerializeField] [Range(0f, 1f)] private float m_FillAmount = 1f;
        [SerializeField] private int m_Steps = 0;
        public ChangeEvent onChange = new ChangeEvent();

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public Type type {
            get { return m_Type; }
            set { m_Type = value; }
        }

        /// <summary>
        /// Gets or sets the target image.
        /// </summary>
        /// <value>The target image.</value>
        public Image targetImage {
            get { return m_TargetImage; }
            set { m_TargetImage = value; }
        }

        /// <summary>
        /// Gets or sets array with the animation sprites.
        /// </summary>
        public Sprite[] sprites {
            get { return m_Sprites; }
            set { m_Sprites = value; }
        }

        /// <summary>
        /// Gets or sets the target transform.
        /// </summary>
        /// <value>The target transform.</value>
        public RectTransform targetTransform {
            get { return m_TargetTransform; }
            set { m_TargetTransform = value; }
        }

        /// <summary>
        /// Gets or sets the minimum width (Used for the resize type bar).
        /// </summary>
        /// <value>The minimum width.</value>
        public float minWidth {
            get { return m_MinWidth; }
            set {
                m_MinWidth = value;
                UpdateBarFill();
            }
        }

        /// <summary>
        /// Gets or sets the maximum width (Used for the resize type bar).
        /// </summary>
        /// <value>The maximum width.</value>
        public float maxWidth {
            get { return m_MaxWidth; }
            set {
                m_MaxWidth = value;
                UpdateBarFill();
            }
        }

        /// <summary>
        /// Gets or sets the fill amount.
        /// </summary>
        /// <value>The fill amount.</value>
        public float fillAmount {
            get {
                return m_FillAmount;
            }
            set {
                if (m_FillAmount != Mathf.Clamp01(value)) {
                    m_FillAmount = Mathf.Clamp01(value);
                    UpdateBarFill();
                    onChange.Invoke(m_FillAmount);
                }
            }
        }

        /// <summary>
        /// Gets or sets the steps (Zero for no stepping).
        /// </summary>
        /// <value>The steps.</value>
        public int steps {
            get { return m_Steps; }
            set { m_Steps = value; }
        }

        /// <summary>
        /// Gets or sets the current step.
        /// </summary>
        /// <value>The current step.</value>
        public int currentStep {
            get {
                if (m_Steps == 0)
                    return 0;

                float perStep = 1f / (m_Steps - 1);
                return Mathf.RoundToInt(fillAmount / perStep);
            }
            set {
                if (m_Steps > 0) {
                    float perStep = 1f / (m_Steps - 1);
                    fillAmount = Mathf.Clamp(value, 0, m_Steps) * perStep;
                }
            }
        }

        protected virtual void Start()
        {
            // Make sure the fill anchor reflects the pivot
            if (m_Type == Type.Resize && m_FillSizing == FillSizing.Parent && m_TargetTransform) {
                float height = m_TargetTransform.rect.height;
                m_TargetTransform.anchorMin = m_TargetTransform.pivot;
                m_TargetTransform.anchorMax = m_TargetTransform.pivot;
                m_TargetTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            }

            // Update the bar fill
            UpdateBarFill();
        }

        protected virtual void OnRectTransformDimensionsChange()
        {
            // Update the bar fill
            UpdateBarFill();
        }

#if UNITY_EDITOR
        protected void OnValidate()
        {
            // Make sure the fill anchor reflects the pivot
            if (m_Type == Type.Resize && m_FillSizing == FillSizing.Parent && m_TargetTransform) {
                float height = m_TargetTransform.rect.height;
                m_TargetTransform.anchorMin = m_TargetTransform.pivot;
                m_TargetTransform.anchorMax = m_TargetTransform.pivot;
                m_TargetTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            }

            // Update the bar fill
            UpdateBarFill();
        }

        protected void Reset()
        {
            onChange = new ChangeEvent();
        }
#endif

        /// <summary>
        /// Updates the bar fill.
        /// </summary>
        public void UpdateBarFill()
        {
            if (!isActiveAndEnabled)
                return;

            if (m_Type == Type.Filled && m_TargetImage == null)
                return;

            if (m_Type == Type.Resize && m_TargetTransform == null)
                return;

            if (m_Type == Type.Sprites && m_Sprites.Length == 0)
                return;

            // Get the fill amount
            float fill = m_FillAmount;

            // Check for steps
            if (m_Steps > 0)
                fill = Mathf.Round(m_FillAmount * (m_Steps - 1)) / (m_Steps - 1);

            if (m_Type == Type.Resize) {
                // Update the bar fill by changing it's width
                // we are doing it this way because we are using a mask on the bar and have it's fill inside with static width and position
                if (m_FillSizing == FillSizing.Fixed) {
                    m_TargetTransform.SetSizeWithCurrentAnchors(
                        RectTransform.Axis.Horizontal,
                        (m_MinWidth + ((m_MaxWidth - m_MinWidth) * fill))
                    );
                } else {
                    m_TargetTransform.SetSizeWithCurrentAnchors(
                        RectTransform.Axis.Horizontal,
                        ((m_TargetTransform.parent as RectTransform).rect.width * fill)
                    );
                }
            } else if (m_Type == Type.Sprites) {
                int spriteIndex = Mathf.RoundToInt(fill * (float)m_Sprites.Length) - 1;

                if (spriteIndex > -1) {
                    targetImage.overrideSprite = m_Sprites[spriteIndex];
                    targetImage.canvasRenderer.SetAlpha(1f);
                } else {
                    targetImage.overrideSprite = null;
                    targetImage.canvasRenderer.SetAlpha(0f);
                }
            } else {
                // Update the image fill amount
                m_TargetImage.fillAmount = fill;
            }
        }

        /// <summary>
        /// Adds to the fill (Used for buttons).
        /// </summary>
        public void AddFill()
        {
            if (m_Steps > 0) {
                currentStep = currentStep + 1;
            } else {
                fillAmount = fillAmount + 0.1f;
            }
        }

        /// <summary>
        /// Removes from the fill (Used for buttons).
        /// </summary>
        public void RemoveFill()
        {
            if (m_Steps > 0) {
                currentStep = currentStep - 1;
            } else {
                fillAmount = fillAmount - 0.1f;
            }
        }
    }
}
