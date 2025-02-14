using System.Collections;
using System.Threading.Tasks;
using UI.Animation;
using UnityEngine;
using UnityEngine.Events;

namespace UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UiWindow : MonoBehaviour
    {
        public UiWindowAnimation animationAsset;
        public object meta;

        public CanvasGroup CanvasGroup => canvasGroup;
        public RectTransform RectTransform => rectTransform;
        public Vector2 Size => new Vector2(rectTransform.rect.width, rectTransform.rect.height);
        public bool IsVisible { get; protected set; }

        public event UnityAction<UiWindow> OnRelease;

        protected CanvasGroup canvasGroup;
        protected RectTransform rectTransform;


        /********************** MONO BEHAVIOUR **********************/

        protected virtual void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            rectTransform = GetComponent<RectTransform>();
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        protected void OnDestroy()
        {
            OnRelease?.Invoke(this);
        }


        /********************** PUBLIC INTERFACE **********************/
        
        public void ShowAndForget()
        {
            ShowAsync();
        }

        public void CloseAndForget()
        {
            CloseAsync();
        }
        
        public virtual async Task ShowAsync()
        {
            if (IsVisible)
                return;

            if (gameObject.activeInHierarchy)
            {
                await animationAsset.ShowAsync(this);
                if (!canvasGroup)
                    return;

                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
                IsVisible = true;
            }
            else
            {
                ShowWithoutAnimation();
            }
        }

        public virtual async Task CloseAsync()
        {
            if (!IsVisible)
                return;

            if (gameObject.activeInHierarchy)
            {
                if (!canvasGroup)
                    return;

                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
                await animationAsset.HideAsync(this);
                IsVisible = false;
            }
            else
            {
                CloseWithoutAnimation();
            }
        }

        public void ShowWithoutAnimation()
        {
            if (!canvasGroup)
                return;

            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            IsVisible = true;
        }
        
        public void CloseWithoutAnimation()
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            IsVisible = false;
        }

        public virtual IEnumerator Show()
        {
            if (IsVisible)
                yield break;

            if (gameObject.activeInHierarchy)
            {
                yield return animationAsset.Show(this);
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
                IsVisible = true;
            }
            else
            {
                ShowWithoutAnimation();
            }
        }

        public virtual IEnumerator Close()
        {
            if (!IsVisible)
                yield break;

            if (gameObject.activeInHierarchy)
            {
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
                yield return animationAsset.Hide(this);
                IsVisible = false;
            }
            else
            {
                CloseWithoutAnimation();
            }
        }
        
    } // end of class
}