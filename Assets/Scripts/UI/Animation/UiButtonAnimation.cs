using System.Threading.Tasks;
using UnityEngine;

namespace UI.Animation
{
    public abstract class UiButtonAnimation : ScriptableObject
    {
        public abstract Task Click(UiButton button);
        public abstract Task Enter(UiButton button);
        public abstract Task Leave(UiButton button);
    }
}