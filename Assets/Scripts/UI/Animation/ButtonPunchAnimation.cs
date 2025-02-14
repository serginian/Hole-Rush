using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace UI.Animation
{
    [CreateAssetMenu(menuName = "serginian/UI/Animation/Button Punch", fileName = "Button Punch", order = 1)]
    public class ButtonPunchAnimation : UiButtonAnimation
    {
        [Header("Animation")] 
        public float clickScale = 1.05f;
        public float defaultScale = 1f;
        public float clickDuration = 0.2f;
        public Ease clickEase = Ease.InOutQuad;

        public override async Task Click(UiButton button)
        {
            float halfDuration = clickDuration * 0.5f;
            var targetTransform = button.RectTransform;
            targetTransform.DOKill();
            targetTransform.DOScale(clickScale, halfDuration).SetEase(clickEase);
            await Awaitable.WaitForSecondsAsync(halfDuration);
            targetTransform.DOScale(defaultScale, halfDuration).SetEase(clickEase);
        }

        public override Task Enter(UiButton button)
        {
            return Task.CompletedTask;
        }

        public override Task Leave(UiButton button)
        {
            return Task.CompletedTask;
        }
    }
}