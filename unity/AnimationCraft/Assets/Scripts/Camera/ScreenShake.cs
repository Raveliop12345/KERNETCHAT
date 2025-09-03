using UnityEngine;

namespace AnimationCraft.CameraRig
{
    public class ScreenShake : MonoBehaviour
    {
        public float decay = 1.5f;
        public float posAmplitude = 0.15f;
        public float rotAmplitude = 0.8f;

        float trauma;
        float t;
        Vector3 baseLocalPos;
        Quaternion baseLocalRot;

        void Awake()
        {
            baseLocalPos = transform.localPosition;
            baseLocalRot = transform.localRotation;
        }

        public void Kick(float power)
        {
            trauma = Mathf.Clamp01(trauma + power);
        }

        void LateUpdate()
        {
            float dt = Time.deltaTime;
            if (trauma > 0f)
            {
                t += dt * 60f;
                float tt = trauma * trauma;
                Vector3 offset = new Vector3(
                    (Mathf.PerlinNoise(t, 0f) - 0.5f),
                    (Mathf.PerlinNoise(0f, t) - 0.5f),
                    (Mathf.PerlinNoise(t, t) - 0.5f)) * posAmplitude * tt;
                Vector3 rot = new Vector3(
                    (Mathf.PerlinNoise(0f, t*1.1f) - 0.5f),
                    (Mathf.PerlinNoise(t*1.3f, 0f) - 0.5f),
                    (Mathf.PerlinNoise(t*0.7f, t*0.9f) - 0.5f)) * rotAmplitude * tt * 10f;
                transform.localPosition = baseLocalPos + offset;
                transform.localRotation = baseLocalRot * Quaternion.Euler(rot);
                trauma = Mathf.Max(0f, trauma - decay * dt);
            }
            else
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, baseLocalPos, 1f - Mathf.Exp(-10f * dt));
                transform.localRotation = Quaternion.Slerp(transform.localRotation, baseLocalRot, 1f - Mathf.Exp(-10f * dt));
            }
        }
    }
}
