using System.Collections;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace UI.Animation
{
    [CreateAssetMenu(fileName = "Window Scale In-Out", menuName = "serginian/UI/Animation/Window Scale In-Out")]
    public class ScaleInOutAnimation : UiWindowAnimation
    {
        public float animationDuration = 0.3f;
        public float scaleMin = 0.0f;
        public float scaleMax = 1.0f;
        public Ease scaleEase = Ease.OutBack;
        public Ease fadeEase = Ease.OutBack;
        
        public override async Task ShowAsync(UiWindow window)
        {
            window.RectTransform.localScale = new Vector3(scaleMin, scaleMin, scaleMin);
            window.RectTransform.DOKill();
            window.CanvasGroup.alpha = 0.0f;
            window.CanvasGroup.DOKill();
            window.CanvasGroup.DOFade(1f, animationDuration).SetEase(fadeEase);
            await window.RectTransform.DOScale(scaleMax, animationDuration).SetEase(scaleEase).AsyncWaitForCompletion();
        }

        public override async Task HideAsync(UiWindow window)
        {
            window.RectTransform.DOKill();
            window.CanvasGroup.DOKill();
            window.CanvasGroup.DOFade(0f, animationDuration).SetEase(fadeEase);
            await window.transform.DOScale(scaleMin, animationDuration).SetEase(scaleEase).AsyncWaitForCompletion();
        }

        public override IEnumerator Show(UiWindow window)
        {
            window.RectTransform.localScale = new Vector3(scaleMin, scaleMin, scaleMin);
            window.RectTransform.DOKill();
            window.CanvasGroup.DOKill();
            window.CanvasGroup.alpha = 0.0f;
            window.CanvasGroup.DOFade(1f, animationDuration).SetEase(fadeEase);
            yield return window.transform.DOScale(scaleMax, animationDuration).SetEase(scaleEase).WaitForCompletion();
        }

        public override IEnumerator Hide(UiWindow window)
        {
            window.RectTransform.DOKill();
            window.CanvasGroup.DOKill();
            window.CanvasGroup.DOFade(0f, animationDuration).SetEase(fadeEase);
            yield return window.transform.DOScale(scaleMin, animationDuration).SetEase(scaleEase).WaitForCompletion();
        }
        
    } // end of class
}