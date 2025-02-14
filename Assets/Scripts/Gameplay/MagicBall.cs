using Audio;
using UnityEngine;

namespace Gameplay
{
    public class MagicBall : MonoBehaviour
    {
        public GameObject hitParticles;
        public GameObject brakeParticles;

        private const string HIT = "Hit";
        private const string MISS = "Miss";
        private CameraShake _cameraShake;

        private void Awake()
        {
            Debug.Assert(Camera.main != null, "There are no active Camera on the scene!");
            _cameraShake = Camera.main.GetComponent<CameraShake>();
        }
        
        
        /********************** MONO BEHAVIOUR **********************/
        private void OnCollisionEnter(Collision other)
        {
            if (other.collider.CompareTag("Hole"))
                Hit(other);
            else
                Brake(other);
            
            _cameraShake.Shake();
        }

        
        /********************** INNER LOGIC **********************/
        private void Hit(Collision collision)
        {
            AudioPlayer.PlaySound(HIT, AudioGroup.Gameplay);
            Instantiate(hitParticles, collision.contacts[0].point, Quaternion.identity);
            Destroy(gameObject);
            collision.gameObject.GetComponent<Hole>().Hit();
        }

        private void Brake(Collision collision)
        {
            AudioPlayer.PlaySound(MISS, AudioGroup.Gameplay);
            Instantiate(brakeParticles, collision.contacts[0].point, Quaternion.identity);
            Destroy(gameObject);
        }
        
        
    } // end of class
}
