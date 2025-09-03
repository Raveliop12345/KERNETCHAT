using AnimationCraft.Inputs;
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
            var camGo = new GameObject("Camera");
            camGo.transform.SetParent(go.transform);
            camGo.transform.localPosition = new Vector3(0, 1.6f, -4f);
            camGo.transform.localRotation = Quaternion.identity;
            var cam = camGo.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.Skybox;

            go.AddComponent<PlayerControllerTPS>();
            return go;
        }
    }
}
