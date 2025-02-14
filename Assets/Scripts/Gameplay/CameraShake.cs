using UnityEngine;

using System.Collections;
using UnityEngine;

namespace Gameplay
{
    public class CameraShake : MonoBehaviour
    {
        public float duration = 1f;
        public float magnitude = 0.05f;
        
        private Vector3 _originalPosition;
        private Coroutine _shakeCoroutine;

        
        
        /********************** MONO BEHAVIOUR **********************/
        private void Start()
        {
            _originalPosition = transform.localPosition;
        }
        
        
        /********************** PUBLIC INTERFACE **********************/
        
        public void Shake()
        {
            if (_shakeCoroutine != null)
            {
                StopCoroutine(_shakeCoroutine);
                transform.localPosition = _originalPosition;
            }

            _shakeCoroutine = StartCoroutine(ShakeRoutine());
        }
        
        
        /********************** INNER LOGIC **********************/
        
        private IEnumerator ShakeRoutine()
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                float progress = elapsed / duration;
                float damper = 1f - Mathf.SmoothStep(0f, 1f, progress);

                float offsetX = Random.Range(-1f, 1f) * magnitude * damper;
                float offsetY = Random.Range(-1f, 1f) * magnitude * damper;

                transform.localPosition = _originalPosition + new Vector3(offsetX, offsetY, 0);

                elapsed += Time.deltaTime;
                yield return null;
            }

            float smoothTime = 0.2f;
            float returnElapsed = 0f;
            while (returnElapsed < smoothTime)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, _originalPosition, returnElapsed / smoothTime);
                returnElapsed += Time.deltaTime;
                yield return null;
            }

            transform.localPosition = _originalPosition;
        }
        
        
    } // end of class
}
