using UnityEngine;

namespace AnimationCraft.FX
{
    public class BlockFXManager : MonoBehaviour
    {
        public static BlockFXManager Instance { get; private set; }

        void Awake()
        {
            if (Instance && Instance != this) { Destroy(gameObject); return; }
            Instance = this; DontDestroyOnLoad(gameObject);
        }

        public void SpawnPlace(Vector3 pos, Color color)
        {
            SpawnBurst(pos, color, 16, 2f, 0.5f, 1.2f);
            SpawnScaleFlash(pos, 1.1f, 0.08f);
        }
        public void SpawnBreak(Vector3 pos, Color color)
        {
            SpawnBurst(pos, color, 24, 3f, 0.8f, 1.5f);
            SpawnScaleFlash(pos, 0.8f, 0.08f);
        }

        void SpawnBurst(Vector3 pos, Color color, int count, float speed, float size, float life)
        {
            var go = new GameObject("FX_Burst");
            go.transform.position = pos;
            var ps = go.AddComponent<ParticleSystem>();
            var main = ps.main; main.duration = 0.2f; main.startLifetime = life; main.startSpeed = speed; main.startSize = size; main.maxParticles = 512; main.loop = false; main.playOnAwake = false;
            var emission = ps.emission; emission.rateOverTime = 0; emission.burstCount = 1; emission.SetBurst(0, new ParticleSystem.Burst(0, (short)count));
            var shape = ps.shape; shape.shapeType = ParticleSystemShapeType.Sphere; shape.radius = 0.3f;
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
