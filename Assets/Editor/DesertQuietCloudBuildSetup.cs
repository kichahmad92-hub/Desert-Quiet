#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace DesertQuiet.Editor
{
    /// <summary>
    /// Configures the generated Mission 01 scene before Unity Build Automation exports the player.
    /// Set the Pre-Export Method in the build target to:
    /// DesertQuiet.Editor.DesertQuietCloudBuildSetup.PreExport
    /// </summary>
    public sealed class DesertQuietCloudBuildSetup : IPreprocessBuildWithReport
    {
        private const string ScenePath = "Assets/Scenes/Mission01_DesertBase.unity";

        public int callbackOrder => -1000;

        /// <summary>
        /// Called by Unity Build Automation's Pre-Export Method hook.
        /// This runs after script compilation and before player export.
        /// </summary>
        public static void PreExport()
        {
            Debug.Log("Desert Quiet: Pre-export scene setup started.");
            EnsureSceneConfiguredNow();
        }

        public void OnPreprocessBuild(BuildReport report)
        {
            EnsureSceneConfiguredNow();
        }

        private static void EnsureSceneConfiguredNow()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
                throw new BuildFailedException("Desert Quiet: Unity is still compiling or updating; cannot safely configure Mission 01 scene.");

            if (!File.Exists(ScenePath))
            {
                Debug.Log("Desert Quiet: Mission scene is missing. Generating it before player export.");
                DesertQuietSceneBuilder.BuildMission01ForAutomation();
                AssetDatabase.Refresh();
            }

            if (!File.Exists(ScenePath))
                throw new BuildFailedException("Desert Quiet: Mission 01 scene was not generated at " + ScenePath);

            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(ScenePath, true)
            };

            AssetDatabase.SaveAssets();
            Debug.Log("Desert Quiet: Mission 01 scene is enabled in Build Settings: " + ScenePath);
        }
    }
}
#endif
