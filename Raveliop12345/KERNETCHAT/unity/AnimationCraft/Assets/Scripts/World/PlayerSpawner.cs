using AnimationCraft.Inputs;
using AnimationCraft.CameraRig;
using UnityEngine;

namespace AnimationCraft.World
{
    public class PlayerSpawner : MonoBehaviour
    {
        public GameObject playerPrefab;

        void Start()
        {
            if (playerPrefab == null)
            {
                playerPrefab = CreateDefaultPlayer();
            }
            var player = GameObject.Instantiate(playerPrefab);
            player.name = "Player";
            player.tag = "Player";
            player.transform.position = new Vector3(0, 64, 0);
        }

        GameObject CreateDefaultPlayer()
        {
            var go = new GameObject("Player");

            var anchor = new GameObject("CameraAnchor");
            anchor.transform.SetParent(go.transform);
            anchor.transform.localPosition = new Vector3(0, 1.6f, -4f);
            anchor.transform.localRotation = Quaternion.identity;

            var camGo = new GameObject("Camera");
            var cam = camGo.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.Skybox;
            var cine = camGo.AddComponent<CinematicCamera>();
            cine.target = anchor.transform;

            go.AddComponent<PlayerControllerTPS>();
            go.AddComponent<AnimationCraft.Core.VisualSetup>();
            var fx = new GameObject("FX"); fx.AddComponent<AnimationCraft.FX.BlockFXManager>();
            return go;
        }
    }
}
