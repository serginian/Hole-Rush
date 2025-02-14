using UnityEngine;
using UnityEngine.InputSystem;

namespace Gameplay
{
    public class PlayerController : MonoBehaviour, IInitializable
    {
        [Header("Settings")] public float maxJaw = 16;
        public float minJaw = -16;
        public float maxPitch = 16;
        public float minPitch = -6;
        public float launchForce = 10f;

        [Header("References")] 
        [SerializeField]
        private TrajectoryDrawer trajectory;

        [Header("Resources")] 
        [SerializeField] private GameObject ballPrefab;

        [Header("Input")] 
        [SerializeField] InputActionReference fireAction;
        [SerializeField] InputActionReference rotateAction;

        private Vector2 _aimDirection = Vector2.zero;
        private bool _missionStarted;
        private bool _isAiming;



        /********************** INITIALIZATION **********************/
        public int InitializationOrder => 0;

        public void Initialize(MissionManager manager)
        {
            // Observer pattern
            manager.OnMissionStarted += OnMissionStarted;
            manager.OnMissionEnded += OnMissionEnded;
        }



        /********************** GAMEPLAY EVENTS **********************/
        private void OnMissionStarted()
        {
            _missionStarted = true;
            fireAction.action.started += OnFireStarted;
            fireAction.action.canceled += OnFireReleased;
            rotateAction.action.performed += OnRotate;
        }

        private void OnMissionEnded()
        {
            _missionStarted = false;
            trajectory.StopDrawing();
            fireAction.action.started -= OnFireStarted;
            rotateAction.action.performed -= OnRotate;
        }

        private void OnEnable()
        {
            if (_missionStarted)
                OnMissionStarted();
        }

        private void OnDisable()
        {
            if (_missionStarted)
                OnMissionEnded();
        }


        /********************** INPUT EVENTS **********************/

        private void OnFireStarted(InputAction.CallbackContext context)
        {
            if (_isAiming)
                return;

            _isAiming = true;
            trajectory.StartDrawing(launchForce);
        }

        private void OnFireReleased(InputAction.CallbackContext context)
        {
            if (!_isAiming)
                return;

            _isAiming = false;
            trajectory.StopDrawing();
            ThrowBall();
        }

        private void OnRotate(InputAction.CallbackContext context)
        {
            if (!_isAiming)
                return;

            Vector2 input = context.ReadValue<Vector2>();
            _aimDirection = new Vector2()
            {
                x = Mathf.Clamp(_aimDirection.x + input.x, -maxJaw, maxJaw),
                y = Mathf.Clamp(_aimDirection.y + input.y, -maxPitch, maxPitch)
            };

            trajectory.UpdateDirection(_aimDirection);
        }



        /********************** INNER LOGIC **********************/
        private void ThrowBall()
        {
            StopAllCoroutines();
            if (ballPrefab == null) return;
            GameObject ball = Instantiate(ballPrefab, transform.position, Quaternion.identity);
            Rigidbody rb = ball.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 launchDirection = Quaternion.Euler(-_aimDirection.y, _aimDirection.x, 0) * Vector3.forward;
                rb.linearVelocity = launchDirection * launchForce;
            }
        }

    } // end of class
}