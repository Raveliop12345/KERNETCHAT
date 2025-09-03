#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
#if UNITY_RENDER_PIPELINE_UNIVERSAL
using UnityEngine.Rendering.Universal;
#endif

namespace AnimationCraft.EditorTools
{
    [InitializeOnLoad]
    public static class AutoSetup
    {
        static AutoSetup()
        {
            EditorApplication.update += RunOnce;
        }

        static void RunOnce()
        {
            EditorApplication.update -= RunOnce;
            EnsureProjectSettings();
            EnsurePipeline();
            EnsureScenes();
            EnsureMaterials();
        }

        static void EnsureProjectSettings()
        {
            PlayerSettings.colorSpace = ColorSpace.Linear;
            QualitySettings.vSyncCount = 0;
            PlayerSettings.defaultIsFullScreen = true;
            try
            {
                PlayerSettings.activeInputHandler = PlayerSettings.ActiveInputHandling.InputSystemPackage;
            }
            catch { }
        }

        static void EnsurePipeline()
        {
#if UNITY_RENDER_PIPELINE_UNIVERSAL
            if (GraphicsSettings.defaultRenderPipeline == null)
            {
                var asset = ScriptableObject.CreateInstance<UniversalRenderPipelineAsset>();
                AssetDatabase.CreateAsset(asset, "Assets/Settings/URPAsset.asset");
                GraphicsSettings.defaultRenderPipeline = asset;
                QualitySettings.renderPipeline = asset;
            }
#endif
        }

        static void EnsureScenes()
        {
            var scenesPath = "Assets/Scenes";
            if (!AssetDatabase.IsValidFolder(scenesPath)) AssetDatabase.CreateFolder("Assets", "Scenes");

            var menuPath = scenesPath + "/Menu.unity";
            var mainPath = scenesPath + "/Main.unity";

            if (!System.IO.File.Exists(menuPath))
            {
                var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
                var go = new GameObject("Menu");
                go.AddComponent<AnimationCraft.UI.MainMenu>();
                EditorSceneManager.SaveScene(scene, menuPath);
            }

            if (!System.IO.File.Exists(mainPath))
            {
                var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
                var game = new GameObject("Game");
                game.AddComponent<AnimationCraft.Core.Bootstrap>();
                game.AddComponent<AnimationCraft.World.PlayerSpawner>();
                game.AddComponent<AnimationCraft.World.WorldStreamer>();
                game.AddComponent<AnimationCraft.UI.HUDOverlay>();
                EditorSceneManager.SaveScene(scene, mainPath);
            }

            EditorBuildSettings.scenes = new[] {
                new EditorBuildSettingsScene(menuPath, true),
                new EditorBuildSettingsScene(mainPath, true)
            };
        }

        static void EnsureMaterials()
        {
            var matPath = "Assets/Materials/Chunk.mat";
            if (!System.IO.File.Exists(matPath))
            {
                var mat = new Material(Shader.Find("AnimationCraft/UnlitVertexColor"));
                AssetDatabase.CreateAsset(mat, matPath);
            }
        }
    }
}
#endif
