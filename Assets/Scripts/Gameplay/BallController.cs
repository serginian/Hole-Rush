using System;
using Audio;
using Pooling;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay
{
    [RequireComponent(typeof(Rigidbody))]
    public class BallController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CameraShake cameraShake;
        [SerializeField] private ParticleSystem sustainedParticles;
        [Header("Resources")]
        public GameObject hitParticles;
        public GameObject brakeParticles;
        
        public event UnityAction OnBallReturned;

        private const string HIT = "Hit";
        private const string MISS = "Miss";

        private Rigidbody _rigidbody;
        // Object Pooling pattern
        private ObjectPool<ShurikenPoolableObject> _hitPool;
        private ObjectPool<ShurikenPoolableObject> _brakePool;
        


        /********************** MONO BEHAVIOUR **********************/
        
        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _hitPool = new ObjectPool<ShurikenPoolableObject>(hitParticles, 3, 10);
            _brakePool = new ObjectPool<ShurikenPoolableObject>(brakeParticles, 3, 10);
        }
        
        private void OnCollisionEnter(Collision other)
        {
            if (other.collider.CompareTag("Hole"))
                Hit(other);
            else
                Brake(other);
            
            cameraShake.Shake();
        }


        
        /******************** PUBLIC  INTERFACE ********************/
        
        public void Throw(Vector3 launchDirection, float launchForce)
        {
            sustainedParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            _rigidbody.isKinematic = false;
            _rigidbody.linearVelocity = launchDirection * launchForce;
        }


        
        /********************** INNER LOGIC **********************/
        private void Hit(Collision collision)
        {
            AudioPlayer.PlaySound(HIT, AudioGroup.Gameplay);
            
            var obj = _hitPool.Get();
            obj.transform.position = collision.contacts[0].point;
            obj.transform.rotation = Quaternion.LookRotation(collision.contacts[0].normal);
            
            collision.gameObject.GetComponent<Hole>().Hit();
            Return();
        }

        private void Brake(Collision collision)
        {
            AudioPlayer.PlaySound(MISS, AudioGroup.Gameplay);
            
            var obj = _brakePool.Get();
            obj.transform.position = collision.contacts[0].point;
            obj.transform.rotation = Quaternion.LookRotation(collision.contacts[0].normal);
            
            Return();
        }

        private void Return()
        {
            sustainedParticles.Play();
            
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            _rigidbody.isKinematic = true;
            
            transform.rotation = Quaternion.identity;
            OnBallReturned?.Invoke();
        }
        
        
    } // end of class
}
