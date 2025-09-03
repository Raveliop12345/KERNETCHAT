using UnityEngine;

namespace AnimationCraft.Chunks
{
    public class ChunkFader : MonoBehaviour
    {
        public float fadeInDuration = 0.2f;
        float t;
        Renderer rend;
        MaterialPropertyBlock mpb;

        void OnEnable()
        {
            if (!rend) rend = GetComponent<Renderer>();
            if (mpb == null) mpb = new MaterialPropertyBlock();
            t = 0f;
            SetFade(0f);
        }

        void Update()
        {
            if (!rend) return;
            t += Time.deltaTime / Mathf.Max(0.01f, fadeInDuration);
            float f = Mathf.Clamp01(t);
            SetFade(f);
        }

        void SetFade(float f)
        {
            rend.GetPropertyBlock(mpb);
            mpb.SetFloat("_Fade", f);
            rend.SetPropertyBlock(mpb);
        }
    }
}
