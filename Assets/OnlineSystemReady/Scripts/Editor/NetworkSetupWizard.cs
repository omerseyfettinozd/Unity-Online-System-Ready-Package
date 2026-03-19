using UnityEngine;
using UnityEditor;
using FishNet.Managing;
using FishNet.Transporting.Tugboat;
using FishNet.Object;
using FishNet.Component.Spawning;
using FishNet.Component.Transforming;
using OnlineSystemReady.Core;
using OnlineSystemReady.Player;

namespace OnlineSystemReady.Editor
{
    public class NetworkSetupWizard : UnityEditor.EditorWindow
    {
        [MenuItem("Online System/Otomatik Prefabları Oluştur")]
        public static void GeneratePrefabs()
        {
            string savePath = "Assets/OnlineSystemReady/Prefabs";
            if (!AssetDatabase.IsValidFolder("Assets/OnlineSystemReady"))
            {
                AssetDatabase.CreateFolder("Assets", "OnlineSystemReady");
            }
            if (!AssetDatabase.IsValidFolder(savePath))
            {
                AssetDatabase.CreateFolder("Assets/OnlineSystemReady", "Prefabs");
            }

            // 1. Create Player Prefab (Root Object for Logic & Physics)
            GameObject playerObj = new GameObject("Player_Generated");
            
            // Bileşenlerin Eklenmesi
            CharacterController cc = playerObj.AddComponent<CharacterController>();
            cc.height = 2f;
            cc.radius = 0.5f;
            cc.center = Vector3.zero;

            playerObj.AddComponent<NetworkObject>();
            NetworkTransform nt = playerObj.AddComponent<NetworkTransform>();
            playerObj.AddComponent<PlayerController>();

            // Görsel (Visual) Objenin Oluşturulması ve Ayrıştırılması (Jitter Fix)
            GameObject visualObj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            visualObj.name = "Visuals";
            DestroyImmediate(visualObj.GetComponent<Collider>()); // Fizik çarpışmaları ana bedende işlenir
            visualObj.transform.SetParent(playerObj.transform);
            visualObj.transform.localPosition = Vector3.zero;

            // NetworkTransform'a Görsel Objeyi (GraphicalObject) Tanıtma
            // FishNet Sürüm Farklılıklarına Karşı Güvenli SerializedObject Ataması
            SerializedObject so = new SerializedObject(nt);
            SerializedProperty gfxProp = so.FindProperty("_graphicalObject");
            if (gfxProp != null)
            {
                gfxProp.objectReferenceValue = visualObj.transform;
                so.ApplyModifiedProperties();
            }

            string playerPath = AssetDatabase.GenerateUniqueAssetPath(savePath + "/Player.prefab");
            GameObject savedPlayer = PrefabUtility.SaveAsPrefabAsset(playerObj, playerPath);
            DestroyImmediate(playerObj);

            // 2. Create NetworkManager Prefab
            GameObject nmObj = new GameObject("NetworkManager");
            NetworkManager netManager = nmObj.AddComponent<NetworkManager>();
            Tugboat tugboat = nmObj.AddComponent<Tugboat>();
            
            PlayerSpawner spawner = nmObj.AddComponent<PlayerSpawner>();
            if (savedPlayer != null) {
                spawner.SetPlayerPrefab(savedPlayer.GetComponent<NetworkObject>());
            }

            NetworkConnectionManager connManager = nmObj.AddComponent<NetworkConnectionManager>();
            connManager.lanTransport = tugboat;

            SplitScreenManager splitManager = nmObj.AddComponent<SplitScreenManager>();
            connManager.splitScreenManager = splitManager;

            // FishyEOS Eklentisini Otomatik Bulup Ekleme (Varsa)
            System.Type eosType = null;
            foreach (var asm in System.AppDomain.CurrentDomain.GetAssemblies()) 
            {
                eosType = asm.GetType("FishNet.Transporting.FishyEOS.FishyEOS");
                if (eosType != null) break;
                eosType = asm.GetType("Epic.OnlineServices.FishyEOS");
                if (eosType != null) break;
            }

            if (eosType != null)
            {
                var eosComp = nmObj.AddComponent(eosType);
                if (eosComp != null)
                {
                    connManager.eosTransport = eosComp as FishNet.Transporting.Transport;
                }
            }

            string nmPath = AssetDatabase.GenerateUniqueAssetPath(savePath + "/NetworkManager.prefab");
            PrefabUtility.SaveAsPrefabAsset(nmObj, nmPath);
            DestroyImmediate(nmObj);

            Debug.Log("[NetworkSetupWizard] NetworkManager ve Player prefabları başarıyla oluşturuldu: " + savePath);
            AssetDatabase.Refresh();
        }
    }
}
