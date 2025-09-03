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

        void Start()
        {
            world = FindObjectOfType<WorldStreamer>();
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
            GUILayout.BeginArea(new Rect(10, 10, 400, 200));
            GUILayout.Label($"Block: scroll to cycle | Seed: {WorldSession.Seed} | View: {WorldSession.ViewRadius}", s);
            if (showDebug && world)
            {
                GUILayout.Label($"FPS: {fps:F1}", s);
                GUILayout.Label($"Chunks: {world.ActiveChunkCount} | Jobs: {world.PendingJobs}", s);
            }
            GUILayout.EndArea();
        }
    }
}
