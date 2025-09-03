using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace AnimationCraft.Core
{
    public class VisualSetup : MonoBehaviour
    {
        void Start()
        {
            EnsureSkybox();
            EnsureGlobalVolume();
        }

        void EnsureSkybox()
        {
            if (RenderSettings.skybox != null) return;
            var tex = Resources.Load<Texture2D>("HDRIs/kiara_1_dawn_4k");
            if (!tex) return;
            var mat = new Material(Shader.Find("Skybox/Panoramic"));
            if (mat.HasProperty("_Tex")) mat.SetTexture("_Tex", tex);
            else mat.SetTexture("_MainTex", tex);
            mat.SetFloat("_Exposure", 1.1f);
            mat.SetFloat("_Rotation", 0f);
            RenderSettings.skybox = mat;
            DynamicGI.UpdateEnvironment();
            RenderSettings.ambientMode = AmbientMode.Skybox;
        }

        void EnsureGlobalVolume()
        {
            var existing = GameObject.Find("Global Volume");
            if (existing) return;
            var go = new GameObject("Global Volume");
            go.layer = 0;
            var vol = go.AddComponent<Volume>();
            vol.isGlobal = true;
            vol.priority = 0;
            var profile = ScriptableObject.CreateInstance<VolumeProfile>();
            vol.sharedProfile = profile;

            profile.TryGet<Bloom>(out var bloom);
            if (!bloom)
            {
                bloom = profile.Add<Bloom>(true);
                bloom.intensity.value = 0.35f;
                bloom.scatter.value = 0.7f;
                bloom.threshold.value = 0.9f;
            }

            profile.TryGet<ColorAdjustments>(out var color);
            if (!color)
            {
                color = profile.Add<ColorAdjustments>(true);
                color.postExposure.value = 0.15f;
                color.contrast.value = 12f;
                color.saturation.value = 12f;
            }

            profile.TryGet<Vignette>(out var vignette);
            if (!vignette)
            {
                vignette = profile.Add<Vignette>(true);
                vignette.intensity.value = 0.2f;
            }

            profile.TryGet<FilmGrain>(out var grain);
            if (!grain)
            {
                grain = profile.Add<FilmGrain>(true);
                grain.intensity.value = 0.12f;
                grain.type.value = FilmGrainLookup.Medium1;
            }

            profile.TryGet<DepthOfField>(out var dof);
            if (!dof)
            {
                dof = profile.Add<DepthOfField>(true);
                dof.mode.value = DepthOfFieldMode.Bokeh;
                dof.focusDistance.value = 12f;
                dof.focalLength.value = 55f;
                dof.aperture.value = 12f;
            }
        }
    }
}
