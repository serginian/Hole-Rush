using System.Collections;
using UnityEngine;

namespace Pooling
{
    [RequireComponent(typeof(ParticleSystem))]
    public class ShurikenPoolableObject : PooledMonoBehaviour
    {
        private ParticleSystem _particleSystem;

        
        /********************** MONO BEHAVIOUR **********************/
        private void Awake()
        {
            _particleSystem = GetComponent<ParticleSystem>();
        }

        
        /******************** PUBLIC  INTERFACE ********************/
        
        // Stop
        public override void Free()
        {
            StopAllCoroutines();
            _particleSystem.Stop();
            _particleSystem.Clear();
        }
        
        // Play
        public override void OnSpawned()
        {
            _particleSystem.Play();
            StartCoroutine(CheckIfAlive());
        }
        
        
        
        /********************** INNER LOGIC **********************/
        private IEnumerator CheckIfAlive()
        {
            var delay = new WaitForSeconds(0.5f);
            while (true)
            {
                yield return delay;
                if (_particleSystem.IsAlive(true))
                    continue;

                Return();
                yield break;
            }
        }
        
        
    } // end of class
}