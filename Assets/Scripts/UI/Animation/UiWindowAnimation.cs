using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace UI.Animation
{
    public abstract class UiWindowAnimation : ScriptableObject
    {
        public abstract Task ShowAsync(UiWindow window);
        public abstract Task HideAsync(UiWindow window);
        public abstract IEnumerator Show(UiWindow window);
        public abstract IEnumerator Hide(UiWindow window);
    }
}