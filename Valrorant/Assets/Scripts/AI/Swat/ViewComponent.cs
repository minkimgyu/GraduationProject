using Demo.Scripts.Runtime;
using Kinemation.FPSFramework.Runtime.Core.Types;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kinemation.FPSFramework.Runtime.Core.Components;
using Kinemation.FPSFramework.Runtime.Core.Playables;
using Kinemation.FPSFramework.Runtime.Layers;

namespace AI.SWAT
{
    public class ViewComponent : MonoBehaviour
    {
        [Header("General")]
        [SerializeField] private Animator animator;

        [Header("Turn In Place")]
        [SerializeField] private float turnInPlaceAngle;
        [SerializeField] private AnimationCurve turnCurve = new AnimationCurve(new Keyframe(0f, 0f));
        [SerializeField] private float turnSpeed = 1f;

        [SerializeField]
        private Vector2 _playerInput;

        private Quaternion moveRotation;
        private float turnProgress = 1f;
        private bool isTurning = false;

        [SerializeField] FPSMovement movement;
        [SerializeField] CoreAnimComponent fpsAnimator;
        [SerializeField] private float sensitivity;

        private static readonly int TurnRight = Animator.StringToHash("TurnRight");
        private static readonly int TurnLeft = Animator.StringToHash("TurnLeft");

        protected CharAnimData charAnimData = new CharAnimData();

        private void TurnInPlace()
        {
            float turnInput = _playerInput.x;
            _playerInput.x = Mathf.Clamp(_playerInput.x, -90f, 90f);
            turnInput -= _playerInput.x;

            float sign = Mathf.Sign(_playerInput.x);
            if (Mathf.Abs(_playerInput.x) > turnInPlaceAngle)
            {
                if (!isTurning)
                {
                    turnProgress = 0f;

                    animator.ResetTrigger(TurnRight);
                    animator.ResetTrigger(TurnLeft);

                    animator.SetTrigger(sign > 0f ? TurnRight : TurnLeft);
                }

                isTurning = true;
            }

            transform.rotation *= Quaternion.Euler(0f, turnInput, 0f);

            float lastProgress = turnCurve.Evaluate(turnProgress);
            turnProgress += Time.deltaTime * turnSpeed;
            turnProgress = Mathf.Min(turnProgress, 1f);

            float deltaProgress = turnCurve.Evaluate(turnProgress) - lastProgress;

            _playerInput.x -= sign * turnInPlaceAngle * deltaProgress;

            transform.rotation *= Quaternion.Slerp(Quaternion.identity,
                Quaternion.Euler(0f, sign * turnInPlaceAngle, 0f), deltaProgress);

            if (Mathf.Approximately(turnProgress, 1f) && isTurning)
            {
                isTurning = false;
            }
        }

        private float _jumpState = 0f;

        // Animation Layers
        [SerializeField] [HideInInspector] private LookLayer lookLayer;
        [SerializeField] [HideInInspector] private AdsLayer adsLayer;
        [SerializeField] [HideInInspector] private SwayLayer swayLayer;
        [SerializeField] [HideInInspector] private LocomotionLayer locoLayer;
        [SerializeField] [HideInInspector] private SlotLayer slotLayer;
        [SerializeField] [HideInInspector] private WeaponCollision collisionLayer;

        protected void InitAnimController()
        {
            fpsAnimator = GetComponentInChildren<CoreAnimComponent>();
            fpsAnimator.animGraph.InitPlayableGraph();
            fpsAnimator.InitializeComponent();

            //recoilComponent = GetComponentInChildren<RecoilAnimation>();

            //fpsCamera = GetComponentInChildren<FPSCamera>();
            //internalLookLayer = GetComponentInChildren<LookLayer>();
            //internalAdsLayer = GetComponentInChildren<AdsLayer>();

            //if (fpsCamera != null)
            //{
            //    fpsCamera.rootBone = fpsAnimator.ikRigData.rootBone;
            //}
        }

        private void Start()
        {
            //InitAnimController();

            //animator = GetComponentInChildren<Animator>();
            //lookLayer = GetComponentInChildren<LookLayer>();
            //adsLayer = GetComponentInChildren<AdsLayer>();
            //locoLayer = GetComponentInChildren<LocomotionLayer>();
            //swayLayer = GetComponentInChildren<SwayLayer>();
            //slotLayer = GetComponentInChildren<SlotLayer>();
            //collisionLayer = GetComponentInChildren<WeaponCollision>();
        }

        protected CoreAnimGraph GetAnimGraph()
        {
            return fpsAnimator.animGraph;
        }

        private void Update()
        {
            UpdateLookInput();
            fpsAnimator.SetCharData(charAnimData);
            fpsAnimator.ikRigData.RetargetWeaponBone();
            GetAnimGraph().UpdateGraph();
            fpsAnimator.ScheduleJobs();
        }


        private void UpdateLookInput()
        {
            //_freeLook = Input.GetKey(KeyCode.X);

            float deltaMouseX = Input.GetAxis("Mouse X") * sensitivity;
            float deltaMouseY = -Input.GetAxis("Mouse Y") * sensitivity;

            //if (_freeLook)
            //{
            //    // No input for both controller and animation component. We only want to rotate the camera

            //    _freeLookInput.x += deltaMouseX;
            //    _freeLookInput.y += deltaMouseY;

            //    _freeLookInput.x = Mathf.Clamp(_freeLookInput.x, -freeLookAngle.x, freeLookAngle.x);
            //    _freeLookInput.y = Mathf.Clamp(_freeLookInput.y, -freeLookAngle.y, freeLookAngle.y);

            //    return;
            //}

            //_freeLookInput = Vector2.Lerp(_freeLookInput, Vector2.zero,
            //    FPSAnimLib.ExpDecayAlpha(15f, Time.deltaTime));

            _playerInput.x += deltaMouseX;
            _playerInput.y += deltaMouseY;

            float proneWeight = animator.GetFloat("ProneWeight");
            Vector2 pitchClamp = Vector2.Lerp(new Vector2(-90f, 90f), new Vector2(-30, 0f), proneWeight);

            _playerInput.y = Mathf.Clamp(_playerInput.y, pitchClamp.x, pitchClamp.y);
            moveRotation *= Quaternion.Euler(0f, deltaMouseX, 0f);
            TurnInPlace();

            //_jumpState = Mathf.Lerp(_jumpState, movementComponent.IsInAir() ? 1f : 0f,
            //    FPSAnimLib.ExpDecayAlpha(10f, Time.deltaTime));

            float moveWeight = Mathf.Clamp01(movement.AnimatorVelocity.magnitude);
            transform.rotation = Quaternion.Slerp(transform.rotation, moveRotation, moveWeight);
            transform.rotation = Quaternion.Slerp(transform.rotation, moveRotation, _jumpState);
            _playerInput.x *= 1f - moveWeight;
            _playerInput.x *= 1f - _jumpState;

            charAnimData.SetAimInput(_playerInput);
            charAnimData.AddDeltaInput(new Vector2(deltaMouseX, charAnimData.deltaAimInput.y));
        }
    }
}