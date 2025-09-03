using UnityEngine;

namespace AnimationCraft.CameraRig
{
    public class CinematicCamera : MonoBehaviour
    {
        public Transform target;
        public float followSmoothing = 12f;
        public float lookSmoothing = 14f;
        public float swayIntensity = 0.02f;
        public float swaySpeed = 1.4f;
        public float fovBase = 60f;
        public float fovSprint = 68f;

        float yawVelocity;
        float pitchVelocity;
        float swayT;
        Camera cam;

        void Awake()
        {
            cam = GetComponent<Camera>();
            if (cam) cam.fieldOfView = fovBase;
        }

        void LateUpdate()
        {
            if (!target) return;
            float dt = Mathf.Max(0.0001f, Time.deltaTime);
            swayT += dt * swaySpeed;
            Vector3 desiredPos = target.position;
            transform.position = Vector3.Lerp(transform.position, desiredPos, 1f - Mathf.Exp(-followSmoothing * dt));

            var kb = UnityEngine.InputSystem.Keyboard.current;
            bool sprint = kb != null && kb.leftShiftKey.isPressed;
            if (cam)
            {
                float targetFov = sprint ? fovSprint : fovBase;
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFov, 1f - Mathf.Exp(-5f * dt));
            }

            float swayX = Mathf.Sin(swayT) * swayIntensity;
            float swayY = Mathf.Cos(swayT * 1.2f) * swayIntensity * 0.6f;
            transform.localPosition += transform.right * swayX + transform.up * swayY;
        }
    }
}
