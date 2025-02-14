using Unity.Collections;
using UnityEngine;

namespace Gameplay
{
    [RequireComponent(typeof(LineRenderer))]
    public class TrajectoryDrawer : MonoBehaviour
    {
        [Header("Settings")] public int distance = 10;
        public float tolerance = 0.1f;

        public bool IsDrawing => _isDrawing;

        private float _force;
        private LineRenderer _trajectoryRenderer;
        private Vector2 _aimDirection;
        private NativeArray<Vector3> _pointsArray;
        private bool _isDrawing;



        /********************** MONO BEHAVIOUR **********************/

        private void Awake()
        {
            _trajectoryRenderer = GetComponent<LineRenderer>();
        }

        private void Start()
        {
            _pointsArray = new NativeArray<Vector3>(distance, Allocator.Persistent);
            _trajectoryRenderer.positionCount = distance;
        }

        private void Update()
        {
            if (!_isDrawing)
                return;

            // use ballistic trajectory equation (y = ->v*g*t)
            Vector3 launchVelocity = Quaternion.Euler(-_aimDirection.y, _aimDirection.x, 0) * Vector3.forward * _force;
            Vector3 currentPosition = transform.position;
            Vector3 currentVelocity = launchVelocity;
            for (int i = 0; i < distance; i++)
            {
                _pointsArray[i] = currentPosition;
                currentVelocity.y += Physics.gravity.y * tolerance;
                currentPosition += currentVelocity * tolerance;
            }

            _trajectoryRenderer.SetPositions(_pointsArray);
        }



        /********************** PUBLIC INTERFACE **********************/

        public void StartDrawing(float force)
        {
            _force = force;
            _isDrawing = true;
        }

        public void UpdateDirection(Vector2 direction)
        {
            _aimDirection = direction;
        }

        public void StopDrawing()
        {
            _isDrawing = false;
            for (int i = 0; i < distance; i++)
                _pointsArray[i] = Vector3.zero;
            _trajectoryRenderer.SetPositions(_pointsArray);
        }

        
    } // end of class
}
