using UnityEngine;

namespace AnimationCraft.Avatar
{
    public class AvatarAnimator : MonoBehaviour
    {
        public Transform avatar;
        public Transform cameraTransform;

        public float idleBobAmplitude = 0.05f;
        public float idleBobFrequency = 1.2f;
        public float leanAngleMax = 20f;
        public float leanSmoothing = 6f;
        public float followSmoothing = 10f;

        Vector3 lastPlayerPos;
        Vector3 velWorld;
        Vector3 velLocal;
        float bobT;
        Vector3 baseLocalPos;
        Quaternion baseLocalRot;
        float targetPitch;
        float targetRoll;

        void Start()
        {
            lastPlayerPos = transform.position;
            if (avatar)
            {
                baseLocalPos = avatar.localPosition;
                baseLocalRot = avatar.localRotation;
            }
        }

        void LateUpdate()
        {
            if (!avatar) return;
            float dt = Mathf.Max(0.0001f, Time.deltaTime);
            Vector3 p = transform.position;
            velWorld = (p - lastPlayerPos) / dt;
            lastPlayerPos = p;

            velLocal = transform.InverseTransformDirection(velWorld);
            float speed = velLocal.magnitude;

            float strafe = Mathf.Clamp(velLocal.x / 10f, -1f, 1f);
            float forward = Mathf.Clamp(velLocal.z / 10f, -1f, 1f);

            float targetRollDeg = -strafe * leanAngleMax;  // roll on X movement
            float targetPitchDeg = -forward * (leanAngleMax * 0.75f); // pitch on Z movement

            targetRoll = Mathf.Lerp(targetRoll, targetRollDeg, 1f - Mathf.Exp(-leanSmoothing * dt));
            targetPitch = Mathf.Lerp(targetPitch, targetPitchDeg, 1f - Mathf.Exp(-leanSmoothing * dt));

            bobT += dt * (idleBobFrequency * Mathf.Lerp(1f, 2.0f, Mathf.Clamp01(speed / 6f)));
            float bob = Mathf.Sin(bobT) * idleBobAmplitude;

            var targetPos = baseLocalPos + new Vector3(0, bob, 0);
            avatar.localPosition = Vector3.Lerp(avatar.localPosition, targetPos, 1f - Mathf.Exp(-followSmoothing * dt));

            var targetRot = Quaternion.Euler(targetPitch, 0f, targetRoll) * baseLocalRot;
            avatar.localRotation = Quaternion.Slerp(avatar.localRotation, targetRot, 1f - Mathf.Exp(-followSmoothing * dt));
        }
    }
}
