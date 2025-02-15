using UnityEngine;
using UnityEngine.Serialization;

namespace Gameplay
{
    [RequireComponent(typeof(Camera))]
    public class LazyFollowCamera : MonoBehaviour
    {
        public float rotationSpeed = 2.0f;
        public float rotationIntensity = 0.5f; 
    
        private Camera _camera;
        private Vector3 _direction;
        private Quaternion _initialRotation;
        
        
        /********************** MONO BEHAVIOUR **********************/
    
        private void Awake()
        {
            _camera = GetComponent<Camera>();
            _initialRotation = _camera.transform.localRotation;
        }

        private void Update()
        {
            Quaternion targetLocalRotation = Quaternion.Euler(-_direction.y, 0, 0) * _initialRotation;
            Quaternion targetWorldRotation = Quaternion.Euler(0, _direction.x, 0);
        
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetLocalRotation, Time.deltaTime * rotationSpeed);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetWorldRotation * transform.rotation, Time.deltaTime * rotationSpeed);
        }
        
        
        /******************** PUBLIC  INTERFACE ********************/
    
        public void FollowDirection(Vector2 direction)
        {
            _direction = direction * rotationIntensity;
        }
        
        
    } // 
}