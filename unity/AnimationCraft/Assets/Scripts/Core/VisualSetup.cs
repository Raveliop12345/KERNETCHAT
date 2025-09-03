using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace AnimationCraft.Core
{
    public class VisualSetup : MonoBehaviour
    {
        void Start()
        {
            EnsureGlobalVolume();
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
                bloom.intensity.value = 0.2f;
                bloom.scatter.value = 0.6f;
                bloom.threshold.value = 0.9f;
            }

            profile.TryGet<ColorAdjustments>(out var color);
            if (!color)
            {
                color = profile.Add<ColorAdjustments>(true);
                color.postExposure.value = 0.0f;
                color.contrast.value = 10f;
                color.saturation.value = 10f;
            }

            profile.TryGet<Vignette>(out var vignette);
            if (!vignette)
            {
                vignette = profile.Add<Vignette>(true);
                vignette.intensity.value = 0.18f;
            }

            profile.TryGet<FilmGrain>(out var grain);
            if (!grain)
            {
                grain = profile.Add<FilmGrain>(true);
                grain.intensity.value = 0.1f;
                grain.type.value = FilmGrainLookup.Thin1;
            }

            profile.TryGet<DepthOfField>(out var dof);
            if (!dof)
            {
                dof = profile.Add<DepthOfField>(true);
                dof.mode.value = DepthOfFieldMode.Bokeh;
                dof.gaussianStart.value = 4f;
                dof.gaussianEnd.value = 20f;
                dof.focusDistance.value = 10f;
                dof.focalLength.value = 50f;
                dof.aperture.value = 16f;
            }
        }
    }
}
