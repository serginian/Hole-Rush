using UnityEngine;

namespace Pooling
{
    public class ObjectPool<T> where T : PooledMonoBehaviour
    {
        private readonly GameObject _objectPrototype;
        private UnityEngine.Pool.ObjectPool<T> _pool;

        
        /******************** PUBLIC  INTERFACE ********************/
        
        public ObjectPool(GameObject objectToPool, int initialPoolSize = 10, int maxPoolSize = 1000)
        {
            _objectPrototype = objectToPool;
            _pool = new UnityEngine.Pool.ObjectPool<T>(OnCreatePooledObject, OnObjectTookFromPool,
                OnObjectReturnedToPool,
                OnObjectDestroyedFromPool, true, initialPoolSize, maxPoolSize);
        }

        public T Get()
        {
            return _pool.Get();
        }
        
        
        /********************** INNER LOGIC **********************/
        
        private void Release(PooledMonoBehaviour obj)
        {
            T gObj = obj as T;
            _pool.Release(gObj);
        }

        private T OnCreatePooledObject()
        {
            GameObject bullet = Object.Instantiate(_objectPrototype);
            T component = bullet.GetComponent<T>();
            component.onReturnToPoolRequest = Release;
            return component;
        }

        private void OnObjectReturnedToPool(T obj)
        {
            obj.Free();
            obj.gameObject.SetActive(false);
        }

        private void OnObjectTookFromPool(T obj)
        {
            obj.gameObject.SetActive(true);
            obj.OnSpawned();
        }

        private void OnObjectDestroyedFromPool(T obj)
        {
            Object.Destroy(obj.gameObject);
        }
        
        
    } // end of class
}