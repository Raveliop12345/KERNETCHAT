using UnityEngine;
using UnityEngine.InputSystem;

namespace AnimationCraft.Inputs
{
    public class PlayerControllerTPS : MonoBehaviour
    {
        public float moveSpeed = 10f;
        public float sprintMultiplier = 2f;
        public float lookSensitivity = 1.5f;
        public Transform cameraTransform;

        float yaw, pitch;

        void Awake()
        {
            if (!cameraTransform)
            {
                var cam = GetComponentInChildren<Camera>();
                if (cam) cameraTransform = cam.transform;
                else cameraTransform = Camera.main ? Camera.main.transform : null;
            }
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        void Update()
        {
            var kb = Keyboard.current;
            var mouse = Mouse.current;
            if (kb == null || mouse == null) return;

            Vector2 look = mouse.delta.ReadValue() * 0.1f * lookSensitivity;
            yaw += look.x;
            pitch -= look.y;
            pitch = Mathf.Clamp(pitch, -89f, 89f);

            transform.rotation = Quaternion.Euler(0, yaw, 0);
            if (cameraTransform)
                cameraTransform.localRotation = Quaternion.Euler(pitch, 0, 0);

            Vector3 dir = Vector3.zero;
            if (kb.wKey.isPressed) dir += Vector3.forward;
            if (kb.sKey.isPressed) dir += Vector3.back;
            if (kb.aKey.isPressed) dir += Vector3.left;
            if (kb.dKey.isPressed) dir += Vector3.right;
            if (kb.spaceKey.isPressed) dir += Vector3.up;
            if (kb.leftCtrlKey.isPressed) dir += Vector3.down;
            float speed = moveSpeed * (kb.leftShiftKey.isPressed ? sprintMultiplier : 1f);
            transform.position += transform.TransformDirection(dir.normalized) * speed * Time.deltaTime;
        }
    }
}
