using UnityEngine;

namespace AnimationCraft.FX
{
    public class BlockFXManager : MonoBehaviour
    {
        public static BlockFXManager Instance { get; private set; }
        AnimationCraft.CameraRig.ScreenShake shaker;

        void Awake()
        {
            if (Instance && Instance != this) { Destroy(gameObject); return; }
            Instance = this; DontDestroyOnLoad(gameObject);
        }

        void Update()
        {
            if (!shaker)
            {
                var cam = Camera.main;
                if (cam) shaker = cam.GetComponent<AnimationCraft.CameraRig.ScreenShake>();
            }
        }

        public void SpawnPlace(Vector3 pos, Color color)
        {
            SpawnBurst(pos, color, 28, 2.8f, 0.6f, 1.2f);
            SpawnScaleFlash(pos, 1.12f, 0.06f);
            shaker?.Kick(0.05f);
        }
        public void SpawnBreak(Vector3 pos, Color color)
        {
            SpawnBurst(pos, color, 48, 3.6f, 0.9f, 1.6f);
            SpawnScaleFlash(pos, 0.82f, 0.06f);
            shaker?.Kick(0.12f);
        }

        void SpawnBurst(Vector3 pos, Color color, int count, float speed, float size, float life)
        {
            var go = new GameObject("FX_Burst");
            go.transform.position = pos;
            var ps = go.AddComponent<ParticleSystem>();
            var main = ps.main; main.duration = 0.2f; main.startLifetime = life; main.startSpeed = speed; main.startSize = size; main.maxParticles = 1024; main.loop = false; main.playOnAwake = false;
            var emission = ps.emission; emission.rateOverTime = 0; emission.burstCount = 1; emission.SetBurst(0, new ParticleSystem.Burst(0, (short)count));
            var shape = ps.shape; shape.shapeType = ParticleSystemShapeType.Sphere; shape.radius = 0.35f;
            var col = ps.colorOverLifetime; col.enabled = true; Gradient g = new Gradient(); g.SetKeys(new[]{ new GradientColorKey(color,0), new GradientColorKey(color,1)}, new[]{ new GradientAlphaKey(1,0), new GradientAlphaKey(0,1)}); col.color = new ParticleSystem.MinMaxGradient(g);
            ps.Play(); Destroy(go, life + 0.5f);
        }

        void SpawnScaleFlash(Vector3 pos, float scale, float time)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = "FX_Scale"; go.transform.position = pos; go.transform.localScale = Vector3.one * 1.001f;
            Destroy(go.GetComponent<Collider>());
            StartCoroutine(ScaleRoutine(go.transform, scale, time));
        }

        System.Collections.IEnumerator ScaleRoutine(Transform t, float target, float time)
        {
            Vector3 baseS = t.localScale; Vector3 targetS = baseS * target; float e = 0; while (e < 1)
            { e += Time.deltaTime / time; t.localScale = Vector3.Lerp(baseS, targetS, e); yield return null; }
            e = 0; while (e < 1){ e += Time.deltaTime / time; t.localScale = Vector3.Lerp(targetS, baseS, e); yield return null; }
            Destroy(t.gameObject);
        }
    }
}
