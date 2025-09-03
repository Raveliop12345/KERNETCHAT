using System;
using System.IO;
using AnimationCraft.Core;
using AnimationCraft.Save;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AnimationCraft.UI
{
    public class MainMenu : MonoBehaviour
    {
        string worldName = "AnimationCraft-World";
        string seedText = "";
        Vector2 scroll;
        string[] worlds = Array.Empty<string>();

        void OnEnable()
        {
            worlds = WorldIndex.ListWorlds();
        }

        void OnGUI()
        {
            GUIStyle title = new GUIStyle(GUI.skin.label) { fontSize = 24 };
            GUILayout.BeginArea(new Rect(20, 20, 600, 600));
            GUILayout.Label("AnimationCraft", title);

            GUILayout.Space(10);
            GUILayout.Label("New World:");
            GUILayout.BeginHorizontal();
            GUILayout.Label("Name:", GUILayout.Width(60));
            worldName = GUILayout.TextField(worldName, GUILayout.Width(200));
            GUILayout.Label("Seed:", GUILayout.Width(40));
            seedText = GUILayout.TextField(seedText, GUILayout.Width(120));
            if (GUILayout.Button("Random", GUILayout.Width(80))) seedText = UnityEngine.Random.Range(int.MinValue, int.MaxValue).ToString();
            if (GUILayout.Button("Create", GUILayout.Width(80)))
            {
                int seed = 0; int.TryParse(seedText, out seed);
                StartWorld(worldName, seed);
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(20);
            GUILayout.Label("Existing Worlds:");
            scroll = GUILayout.BeginScrollView(scroll, GUILayout.Height(200));
            foreach (var w in WorldIndex.ListWorlds())
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(w, GUILayout.Width(200));
                if (GUILayout.Button("Play", GUILayout.Width(80)))
                {
                    var infoPath = WorldIndex.WorldJson(w);
                    int seed = 0;
                    if (File.Exists(infoPath))
                    {
                        var json = File.ReadAllText(infoPath);
                        var info = JsonUtility.FromJson<WorldInfo>(json);
                        seed = info.seed;
                    }
                    StartWorld(w, seed);
                }
                if (GUILayout.Button("Delete", GUILayout.Width(80)))
                {
                    WorldIndex.DeleteWorld(w);
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();

            GUILayout.EndArea();
        }

        void StartWorld(string id, int seed)
        {
            WorldSession.WorldId = id;
            WorldSession.Seed = seed == 0 ? UnityEngine.Random.Range(int.MinValue, int.MaxValue) : seed;
            WorldSession.ViewRadius = Constants.DefaultViewRadius;
            SceneManager.LoadScene("Assets/Scenes/Main.unity");
        }
    }
}
