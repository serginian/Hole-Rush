using UnityEngine;
using UnityEngine.Events;

namespace Pooling
{
    public abstract class PooledMonoBehaviour : MonoBehaviour
    {
        public UnityAction<PooledMonoBehaviour> onReturnToPoolRequest;

        public abstract void Free();

        public abstract void OnSpawned();

        public void Return()
        {
            onReturnToPoolRequest?.Invoke(this);
        }
        
    } // end of class
}