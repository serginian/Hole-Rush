using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gameplay
{
    public class PlayerController : MonoBehaviour, IInitializable
    {
        [Header("Settings")] 
        public float maxJaw = 16;
        public float minJaw = -16;
        public float maxPitch = 16;
        public float minPitch = -6;
        public float launchForce = 10f;

        [Header("References")] 
        [SerializeField] private TrajectoryDrawer trajectoryVisualisation;
        [SerializeField] private BallController ballController;
        [SerializeField] private Camera mainCamera;

        [Header("Input")] 
        [SerializeField] InputActionReference fireAction;
        [SerializeField] InputActionReference rotateAction;

        private bool IsAiming => trajectoryVisualisation.IsDrawing; // decouple if more performance is needed
        
        private LazyFollowCamera _lazyFollowCamera;
        private Vector2 _aimDirection = Vector2.zero;
        private bool _ballIsReady = true;
        private bool _missionStarted;



        /********************** INITIALIZATION **********************/
        public int InitializationOrder => 0;

        public void Initialize(MissionManager manager)
        {
            // Observer pattern
            manager.OnMissionStarted += OnMissionStarted;
            manager.OnMissionEnded += OnMissionEnded;
        }



        /********************** GAMEPLAY EVENTS **********************/

        private void Awake()
        {
            _lazyFollowCamera = mainCamera.GetComponent<LazyFollowCamera>();
        }

        private void Start()
        {
            ballController.OnBallReturned += OnBallReturned;
        }

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
            trajectoryVisualisation.StopDrawing();
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
            if (IsAiming || !_ballIsReady)
                return;

            trajectoryVisualisation.StartDrawing(launchForce);
        }

        private void OnFireReleased(InputAction.CallbackContext context)
        {
            if (!IsAiming)
                return;

            ThrowBall();
            
            _aimDirection = Vector2.zero;
            _lazyFollowCamera.FollowDirection(_aimDirection);
            trajectoryVisualisation.UpdateDirection(_aimDirection);
            trajectoryVisualisation.StopDrawing();
        }

        private void OnRotate(InputAction.CallbackContext context)
        {
            if (!IsAiming)
                return;

            Vector2 input = context.ReadValue<Vector2>();
            _aimDirection = new Vector2()
            {
                x = Mathf.Clamp(_aimDirection.x + input.x, -maxJaw, maxJaw),
                y = Mathf.Clamp(_aimDirection.y + input.y, -maxPitch, maxPitch)
            };
            _lazyFollowCamera.FollowDirection(_aimDirection);
            trajectoryVisualisation.UpdateDirection(_aimDirection);
        }



        /********************** INNER LOGIC **********************/
        
        private void ThrowBall()
        {
            Vector3 launchDirection = Quaternion.Euler(-_aimDirection.y, _aimDirection.x, 0) * Vector3.forward;
            ballController.Throw(launchDirection, launchForce);
            _ballIsReady = false;
        }

        private void OnBallReturned()
        {
            ballController.transform.localPosition = Vector3.zero;
            _ballIsReady = true;
        }

        
    } // end of class
}