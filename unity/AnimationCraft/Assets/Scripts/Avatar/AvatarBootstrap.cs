using UnityEngine;

namespace AnimationCraft.Avatar
{
    public class AvatarBootstrap : MonoBehaviour
    {
        static AvatarBootstrap instance;
        Transform player;
        bool spawned;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Init()
        {
            var go = new GameObject("AvatarManager");
            DontDestroyOnLoad(go);
            instance = go.AddComponent<AvatarBootstrap>();
        }

        void Update()
        {
            if (spawned) return;
            var pgo = GameObject.FindWithTag("Player");
            if (!pgo) return;
            player = pgo.transform;
            SpawnAvatar();
            spawned = true;
        }

        void SpawnAvatar()
        {
            var avatar = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            avatar.name = "Avatar";
            avatar.transform.SetParent(player);
            avatar.transform.localPosition = new Vector3(0, 0.9f, 0);
            avatar.transform.localRotation = Quaternion.identity;
            avatar.transform.localScale = Vector3.one;
            var col = avatar.GetComponent<Collider>();
            if (col) Destroy(col);

            var anim = player.gameObject.AddComponent<AvatarAnimator>();
            anim.avatar = avatar.transform;
            var cam = player.GetComponentInChildren<Camera>();
            anim.cameraTransform = cam ? cam.transform : null;
        }
    }
}
