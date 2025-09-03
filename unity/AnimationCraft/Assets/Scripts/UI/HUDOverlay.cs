using AnimationCraft.Core;
using AnimationCraft.World;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AnimationCraft.UI
{
    public class HUDOverlay : MonoBehaviour
    {
        bool showDebug = false;
        float fps;
        float accum;
        int frames;
        float timer;
        WorldStreamer world;
        int viewSlider;

        void Start()
        {
            world = FindObjectOfType<WorldStreamer>();
            viewSlider = WorldSession.ViewRadius;
        }

        void Update()
        {
            var kb = Keyboard.current;
            if (kb != null && kb.f3Key.wasPressedThisFrame) showDebug = !showDebug;

            frames++;
            accum += Time.unscaledDeltaTime;
            timer += Time.unscaledDeltaTime;
            if (timer >= 0.5f)
            {
                fps = frames / accum;
                frames = 0; accum = 0; timer = 0;
            }
        }

        void OnGUI()
        {
            GUIStyle s = new GUIStyle(GUI.skin.label) { fontSize = 14, normal = { textColor = Color.white } };
            GUILayout.BeginArea(new Rect(10, 10, 420, 240));
            GUILayout.Label($"Block: scroll to cycle | Seed: {WorldSession.Seed}", s);
            GUILayout.BeginHorizontal();
            GUILayout.Label("View radius:", GUILayout.Width(90));
            string val = GUILayout.TextField(viewSlider.ToString(), GUILayout.Width(40));
            if (int.TryParse(val, out int parsed)) viewSlider = Mathf.Clamp(parsed, 1, 12);
            if (GUILayout.Button("Apply", GUILayout.Width(60)) && world)
            {
                world.SetViewRadius(viewSlider);
                WorldSession.ViewRadius = viewSlider;
            }
            GUILayout.EndHorizontal();

            if (showDebug && world)
            {
                GUILayout.Label($"FPS: {fps:F1}", s);
                GUILayout.Label($"Chunks: {world.ActiveChunkCount} | Jobs: {world.PendingJobs}", s);
            }
            GUILayout.EndArea();
        }
    }
}
