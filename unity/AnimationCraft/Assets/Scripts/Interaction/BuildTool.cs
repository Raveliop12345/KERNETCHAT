using AnimationCraft.Core;
using AnimationCraft.World;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AnimationCraft.Interaction
{
    public class BuildTool : MonoBehaviour
    {
        public WorldStreamer world;
        public Camera cam;
        public byte currentId = 4; // Grass default

        void Start()
        {
            if (!cam) cam = Camera.main;
            if (!world) world = FindObjectOfType<WorldStreamer>();
        }

        void Update()
        {
            if (!cam || !world) return;
            var hit = BlockRaycaster.Raycast(cam, Constants.MaxRayDistance);
            if (!hit.valid) return;

            var kb = Keyboard.current;
            var mouse = Mouse.current;
            if (mouse == null) return;

            if (mouse.leftButton.wasPressedThisFrame)
            {
                world.SetBlock(hit.block, 0);
            }
            else if (mouse.rightButton.wasPressedThisFrame)
            {
                var place = hit.block + hit.normal;
                world.SetBlock(place, currentId);
            }

            if (mouse.scroll.y.ReadValue() > 0) currentId = (byte)Mathf.Clamp(currentId + 1, 1, 5);
            else if (mouse.scroll.y.ReadValue() < 0) currentId = (byte)Mathf.Clamp(currentId - 1, 1, 5);
        }
    }
}
