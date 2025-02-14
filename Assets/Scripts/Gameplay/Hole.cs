using UnityEngine;
using DG.Tweening;
using System.Collections;
using UnityEngine.Events;
using Debug = System.Diagnostics.Debug;

namespace Gameplay
{
    public enum HoleState
    {
        Good,
        Bad
    }

    public class Hole : MonoBehaviour
    {
        [Header("Settings")] public Color goodColor = Color.blue;
        public Color badColor = Color.red;
        public float showHideTime = 0.5f;
        public float stateChangeIntervalMin = 5f;
        public float stateChangeIntervalMax = 12f;
        public float warningTime = 3f;
        public float flashingSpeed = 0.2f;

        public HoleState State => _state;

        private HoleState _state;
        private Vector3 _initialSize;
        private Renderer _renderer;
        private Coroutine _changeStateCoroutine;
        private UnityAction<Hole> _onHoleHit;
        



        /********************** MONO BEHAVIOUR **********************/
        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
            _initialSize = transform.localScale;
            transform.localScale = Vector3.zero;
        }

        
        /********************** PUBLIC INTERFACE **********************/

        public void Initialize(MissionManager manager, HoleState initialState, UnityAction<Hole> onHit)
        {
            _onHoleHit = onHit;
            _state = initialState;
            UpdateColor(initialState);

            manager.OnMissionEnded += DestroyHole;
            
            _changeStateCoroutine = StartCoroutine(StateChangeRoutine());
            Appear();
        }

        public void Hit()
        {
            _onHoleHit?.Invoke(this);
        }

        public void Appear()
        {
            _renderer.enabled = true;
            transform.DOKill();
            transform.DOScale(_initialSize, showHideTime);
        }

        public void Disappear(UnityAction onComplete = null)
        {
            _renderer.enabled = true;
            transform.DOKill();
            transform.DOScale(Vector3.zero, showHideTime).OnComplete(() => onComplete?.Invoke());
        }



        /********************** PRIVATE LOGIC **********************/
        private void DestroyHole()
        {
            Destroy(gameObject);
        }
        
        private void UpdateColor(HoleState state)
        {
            var color = state == HoleState.Good ? goodColor : badColor;
            _renderer.material.color = color;
            _renderer.material.SetColor("_EmissionColor", color);
        }

        private IEnumerator StateChangeRoutine()
        {
            var flashTime = new WaitForSeconds(flashingSpeed);
            while (true)
            {
                // wait random time
                yield return new WaitForSeconds(Random.Range(stateChangeIntervalMin, stateChangeIntervalMax));

                // start flashing (additional time)
                for (float t = 0; t < warningTime; t += flashingSpeed)
                {
                    _renderer.enabled = !_renderer.enabled;
                    yield return flashTime;
                }

                // change state
                _renderer.enabled = true;
                _state = _state == HoleState.Good ? HoleState.Bad : HoleState.Good;
                UpdateColor(_state);

            } // repeat
        }
        
    } // end of class
}