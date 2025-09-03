using AnimationCraft.Core;
using AnimationCraft.World;
using AnimationCraft.Voxel;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AnimationCraft.Interaction
{
    public class BuildTool : MonoBehaviour
    {
        public WorldStreamer world;
        public Camera cam;
        public byte currentId = 4;

        void Start()
        {
            if (!cam) cam = Camera.main;
            if (!world) world = FindObjectOfType<WorldStreamer>();
            EnsureFX();
        }

        void EnsureFX()
        {
            if (!AnimationCraft.FX.BlockFXManager.Instance)
            {
                var fx = new GameObject("FX"); fx.AddComponent<AnimationCraft.FX.BlockFXManager>();
            }
        }

        void Update()
        {
            if (!cam || !world) return;
            var hit = BlockRaycaster.Raycast(cam, Constants.MaxRayDistance);
            if (!hit.valid) return;

            var mouse = Mouse.current;
            if (mouse == null) return;

            if (mouse.leftButton.wasPressedThisFrame)
            {
                world.SetBlock(hit.block, 0);
                var c = AnimationCraft.Voxel.BlockRegistry.Get(BlockId.Grass).color;
                AnimationCraft.FX.BlockFXManager.Instance?.SpawnBreak(hit.block + Vector3.one * 0.5f, c);
            }
            else if (mouse.rightButton.wasPressedThisFrame)
            {
                var place = hit.block + hit.normal;
                world.SetBlock(place, currentId);
                var c = AnimationCraft.Voxel.BlockRegistry.Get((BlockId)currentId).color;
                AnimationCraft.FX.BlockFXManager.Instance?.SpawnPlace(place + Vector3.one * 0.5f, c);
            }

            float scroll = mouse.scroll.y.ReadValue();
            if (scroll > 0) currentId = (byte)Mathf.Clamp(currentId + 1, 1, 5);
            else if (scroll < 0) currentId = (byte)Mathf.Clamp(currentId - 1, 1, 5);
        }
    }
}
