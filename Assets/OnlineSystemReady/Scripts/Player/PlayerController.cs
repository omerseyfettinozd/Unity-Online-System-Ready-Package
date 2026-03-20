using UnityEngine;
using FishNet.Object;
using FishNet.Object.Prediction;
using FishNet.Transporting;

namespace OnlineSystemReady.Player
{
    // A strut to hold the input variables for replicate
    public struct MoveData : IReplicateData
    {
        public Vector2 MoveInput;

        private uint _tick;
        public void Dispose() { }
        public uint GetTick() => _tick;
        public void SetTick(uint value) => _tick = value;
    }

    // A struct to hold the state variables for reconcile
    public struct ReconcileData : IReconcileData
    {
        public Vector3 Position;
        public Quaternion Rotation;

        private uint _tick;
        public void Dispose() { }
        public uint GetTick() => _tick;
        public void SetTick(uint value) => _tick = value;
    }

    /// <summary>
    /// Handles Cross-Platform inputs and FishNet Client-Side Prediction (CSP).
    /// </summary>
    public class PlayerController : NetworkBehaviour
    {
        [Header("Movement Settings")]
        public float moveSpeed = 5f;
        public float rotationSpeed = 10f;

        private CharacterController _characterController;
        private Vector2 _currentMoveInput;

        [Header("Smoothing (Host Jitter Fix)")]
        [Tooltip("Sadece Host ekranındaki titremeyi gidermek için Visuals objesini buraya atayın.")]
        public Transform visuals;
        private bool _isHostSmoothed = false;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
        }

        private void OnDestroy()
        {
            if (visuals != null && _isHostSmoothed)
            {
                Destroy(visuals.gameObject);
            }
        }

        // --- INPUT READING & HOST SMOOTHING --- //
        private void Update()
        {
            // Host ekranında Client'in süzülerek (pürüzsüz) kaymasını sağlayan Slerp Süzgeci
            if (_isHostSmoothed && visuals != null)
            {
                visuals.position = Vector3.Lerp(visuals.position, transform.position, Time.deltaTime * 25f);
                visuals.rotation = Quaternion.Slerp(visuals.rotation, transform.rotation, Time.deltaTime * 25f);
            }


            if (base.IsOwner)
            {
                // Reading from Unity's new Input System
                // Assumes there's an active Input Action Map that logs Vector2 to a public/static method or component
                // For now, testing with direct active InputSystem device reading or simplified wrapper
                
                // Note: We need a reliable way to get Vector2 from the Input System without hardcoding references.
                // Creating a fallback/temporary wrapper for Cross-platform reading.
#if ENABLE_INPUT_SYSTEM
                if (UnityEngine.InputSystem.Gamepad.current != null)
                {
                    _currentMoveInput = UnityEngine.InputSystem.Gamepad.current.leftStick.ReadValue();
                }
                else if (UnityEngine.InputSystem.Keyboard.current != null)
                {
                    float h = 0f; float v = 0f;
                    if (UnityEngine.InputSystem.Keyboard.current.wKey.isPressed || UnityEngine.InputSystem.Keyboard.current.upArrowKey.isPressed) v += 1f;
                    if (UnityEngine.InputSystem.Keyboard.current.sKey.isPressed || UnityEngine.InputSystem.Keyboard.current.downArrowKey.isPressed) v -= 1f;
                    if (UnityEngine.InputSystem.Keyboard.current.dKey.isPressed || UnityEngine.InputSystem.Keyboard.current.rightArrowKey.isPressed) h += 1f;
                    if (UnityEngine.InputSystem.Keyboard.current.aKey.isPressed || UnityEngine.InputSystem.Keyboard.current.leftArrowKey.isPressed) h -= 1f;
                    _currentMoveInput = new Vector2(h, v).normalized;
                }
#else
                float h = Input.GetAxisRaw("Horizontal");
                float v = Input.GetAxisRaw("Vertical");
                _currentMoveInput = new Vector2(h, v).normalized;
#endif
            }
        }

        // --- FISHNET TIMING (Tick System) --- //
        public override void OnStartNetwork()
        {
            base.OnStartNetwork();
            base.TimeManager.OnTick += TimeManager_OnTick;
            base.TimeManager.OnPostTick += TimeManager_OnPostTick;

            // Host Jitter Fix: Eğer Sunucu isek ve obje bizim değilse görseli bedenden ayır
            if (base.IsServerStarted && !base.Owner.IsLocalClient && visuals != null)
            {
                visuals.SetParent(null);
                _isHostSmoothed = true;
            }
        }

        public override void OnStopNetwork()
        {
            base.OnStopNetwork();
            if (base.TimeManager != null)
            {
                base.TimeManager.OnTick -= TimeManager_OnTick;
                base.TimeManager.OnPostTick -= TimeManager_OnPostTick;
            }
        }

        private void TimeManager_OnTick()
        {
            if (base.IsOwner)
            {
                Reconciliation(default, Channel.Unreliable); // Check for server corrections
                MoveData data = new MoveData { MoveInput = _currentMoveInput };
                Move(data); // Send input to server & predict locally
            }
            if (base.IsServerStarted)
            {
                Move(default); // Server unspools and executes inputs
            }
        }

        private void TimeManager_OnPostTick()
        {
            if (base.IsServerStarted)
            {
                // Server sends the true state back to clients to correct them
                ReconcileData data = new ReconcileData
                {
                    Position = transform.position,
                    Rotation = transform.rotation
                };
                Reconciliation(data, Channel.Unreliable);
            }
        }

        public override void CreateReconcile()
        {
            ReconcileData data = new ReconcileData
            {
                Position = transform.position,
                Rotation = transform.rotation
            };
            Reconciliation(data, Channel.Unreliable);
        }

        // --- REPLICATE (Movement Logic) --- //
        [Replicate]
        private void Move(MoveData data, ReplicateState state = ReplicateState.Invalid, Channel channel = Channel.Unreliable)
        {
            if (_characterController == null) return;

            Vector3 moveDirection = new Vector3(data.MoveInput.x, 0f, data.MoveInput.y);
            
            // Move character
            _characterController.Move(moveDirection * moveSpeed * (float)base.TimeManager.TickDelta);

            // Rotate character to face movement direction
            if (moveDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * (float)base.TimeManager.TickDelta);
            }
        }

        // --- RECONCILE (Correction Logic) --- //
        [Reconcile]
        private void Reconciliation(ReconcileData data, Channel channel = Channel.Unreliable)
        {
            // If the server tells us we are at a different place, we snap to it.
            transform.position = data.Position;
            transform.rotation = data.Rotation;
        }
    }
}
