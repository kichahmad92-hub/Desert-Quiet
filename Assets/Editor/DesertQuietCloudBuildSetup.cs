#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace DesertQuiet.Editor
{
    /// <summary>
    /// Ensures the generated Mission 01 scene exists and is enabled for cloud builds.
    /// This runs when the Unity Editor loads the project, before Build Automation starts
    /// collecting the enabled Build Settings scenes.
    /// </summary>
    [InitializeOnLoad]
    public sealed class DesertQuietCloudBuildSetup : IPreprocessBuildWithReport
    {
        private const string ScenePath = "Assets/Scenes/Mission01_DesertBase.unity";

        static DesertQuietCloudBuildSetup()
        {
            EditorApplication.delayCall += EnsureSceneConfigured;
        }

        public int callbackOrder => -1000;

        public void OnPreprocessBuild(BuildReport report)
        {
            EnsureSceneConfigured();
        }

        private static void EnsureSceneConfigured()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                EditorApplication.delayCall += EnsureSceneConfigured;
                return;
            }

            if (!File.Exists(ScenePath))
            {
                Debug.Log("Desert Quiet: Mission scene is missing. Generating it for the cloud build.");
                DesertQuietSceneBuilder.BuildMission01ForAutomation();
                AssetDatabase.Refresh();
            }

            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(ScenePath, true)
            };

            Debug.Log("Desert Quiet: Mission 01 scene is enabled in Build Settings.");
        }
    }
}
#endif
