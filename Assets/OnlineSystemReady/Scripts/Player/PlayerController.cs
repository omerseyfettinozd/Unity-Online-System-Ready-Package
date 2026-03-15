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

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
        }

        // --- INPUT READING (For all platforms: PC, Mobile, Console) --- //
        private void Update()
        {
            if (base.IsOwner)
            {
                // In Phase 3 implementation, Unity's new Input System will feed this.
                // Fallback basic input reading for testing purposes:
                float h = Input.GetAxisRaw("Horizontal");
                float v = Input.GetAxisRaw("Vertical");
                _currentMoveInput = new Vector2(h, v).normalized;
            }
        }

        // --- FISHNET TIMING (Tick System) --- //
        public override void OnStartNetwork()
        {
            base.OnStartNetwork();
            base.TimeManager.OnTick += TimeManager_OnTick;
        }

        public override void OnStopNetwork()
        {
            base.OnStopNetwork();
            if (base.TimeManager != null)
            {
                base.TimeManager.OnTick -= TimeManager_OnTick;
            }
        }

        private void TimeManager_OnTick()
        {
            if (base.IsOwner)
            {
                Reconciliation(default, false); // Check for server corrections
                MoveData data = new MoveData { MoveInput = _currentMoveInput };
                Move(data, false); // Send input to server & predict locally
            }

            if (base.IsServer)
            {
                Move(default, true); // Server processes incoming inputs
            }
        }

        public override void CreateReconcile()
        {
            // Server calls this automatically to generate and send the synchronization data to clients.
            ReconcileData data = new ReconcileData
            {
                Position = transform.position,
                Rotation = transform.rotation
            };
            Reconciliation(data, true, Channel.Unreliable);
        }

        // --- REPLICATE (Movement Logic) --- //
        [Replicate]
        private void Move(MoveData data, bool asServer, Channel channel = Channel.Unreliable, bool replaying = false)
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
        private void Reconciliation(ReconcileData data, bool asServer, Channel channel = Channel.Unreliable)
        {
            // If the server tells us we are at a different place, we snap to it.
            transform.position = data.Position;
            transform.rotation = data.Rotation;
        }
    }
}
