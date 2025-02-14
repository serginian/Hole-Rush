using Audio;
using DG.Tweening;
using UI.Animation;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(RectTransform))]
    public class UiButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("References")] 
        [SerializeField] private MaskableGraphic targetGraphic;
        [SerializeField] private UiButtonAnimation animationAsset;
        
        [Header("Settings")] 
        public string sound = "Click";
        public Color defaultColor = Color.white;
        public Color hoverColor = Color.white;
        public Color inactiveColor = Color.gray;
        public float colorChangeDuration = 0.5f;
        public bool isInteractable = true;

        public event UnityAction OnClick;
        public UnityEvent onClick;

        public RectTransform RectTransform => rectTransform;

        public bool IsActive
        {
            get => isInteractable;
            set
            {
                isInteractable = value;
                targetGraphic.DOKill();
                if (isInteractable)
                    targetGraphic.DOColor(IsHovered ? hoverColor : defaultColor, colorChangeDuration);
                else
                    targetGraphic.DOColor(inactiveColor, colorChangeDuration);
            }
        }

        public bool IsHovered { get; private set; }

        protected RectTransform rectTransform;


        protected virtual void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            IsActive = isInteractable;
        }


        public void Invoke()
        {
            OnPointerClick(new PointerEventData(EventSystem.current));
        }

        public virtual async void OnPointerEnter(PointerEventData eventData)
        {
            if (!IsActive)
                return;

            if (animationAsset != null)
                await animationAsset.Enter(this);


            targetGraphic.DOKill();
            targetGraphic.DOColor(hoverColor, colorChangeDuration);

            IsHovered = true;
        }

        public virtual async void OnPointerExit(PointerEventData eventData)
        {
            if (!IsActive)
                return;

            if (animationAsset != null)
                await animationAsset.Leave(this);

            targetGraphic.DOKill();
            targetGraphic.DOColor(defaultColor, colorChangeDuration);

            IsHovered = false;
        }

        public async virtual void OnPointerClick(PointerEventData eventData)
        {
            if (!IsActive)
                return;

            AudioPlayer.PlaySound(sound, AudioGroup.UI);

            if (animationAsset != null)
                await animationAsset.Click(this);

            onClick?.Invoke();
            OnClick?.Invoke();
        }

        public void SetActive(bool active)
        {
            IsActive = active;
        }
        
    } // end of class
}