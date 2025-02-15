using System;
using Audio;
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
        


        /********************** MONO BEHAVIOUR **********************/
        
        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
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
            //todo: to object pooling
            Instantiate(hitParticles, collision.contacts[0].point, Quaternion.identity);
            collision.gameObject.GetComponent<Hole>().Hit();
            Return();
        }

        private void Brake(Collision collision)
        {
            AudioPlayer.PlaySound(MISS, AudioGroup.Gameplay);
            //todo: to object pooling
            Instantiate(brakeParticles, collision.contacts[0].point, Quaternion.identity);
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
