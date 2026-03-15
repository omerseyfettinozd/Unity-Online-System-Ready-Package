using UnityEngine;

namespace OnlineSystemReady.Core
{
    /// <summary>
    /// Handles splitting cameras and spawning a secondary local generic player for couch co-op.
    /// </summary>
    public class SplitScreenManager : MonoBehaviour
    {
        [Header("Split Screen Settings")]
        [Tooltip("The generic Player 2 prefab (does NOT need a NetworkObject since it's fully local)")]
        public GameObject localPlayer2Prefab;
        public Transform player2SpawnPoint;

        public void EnableSplitScreen()
        {
            if (localPlayer2Prefab == null)
            {
                Debug.LogWarning("[SplitScreen] Local Player 2 Prefab is not assigned.");
                return;
            }

            // Spawn the secondary player locally (No network synchronization needed for Player 2 on the same machine)
            Vector3 spawnPos = player2SpawnPoint != null ? player2SpawnPoint.position : Vector3.zero;
            GameObject player2 = Instantiate(localPlayer2Prefab, spawnPos, Quaternion.identity);
            player2.name = "Player2_Local";

            AdjustCamerasForSplitScreen();
        }

        private void AdjustCamerasForSplitScreen()
        {
            // Player 1 Camera (Top Half)
            Camera p1Camera = Camera.main;
            if (p1Camera != null)
            {
                p1Camera.rect = new Rect(0f, 0.5f, 1f, 0.5f);
            }

            // Player 2 Camera (Bottom Half)
            // It searches for the camera inside the newly spawned Player2 object.
            GameObject localPlayer2 = GameObject.Find("Player2_Local");
            if (localPlayer2 != null)
            {
                Camera p2Camera = localPlayer2.GetComponentInChildren<Camera>();
                if (p2Camera != null)
                {
                    p2Camera.rect = new Rect(0f, 0f, 1f, 0.5f);
                }
            }
        }
    }
}
