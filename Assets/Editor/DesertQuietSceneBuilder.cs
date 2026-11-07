#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using DesertQuiet.Player;
using DesertQuiet.Missions;
using DesertQuiet.World;
using DesertQuiet.Combat;

namespace DesertQuiet.Editor
{
    public static class DesertQuietSceneBuilder
    {
        [MenuItem("Desert Quiet/Build Mission 01 Scene")]
        public static void BuildMission01()
        {
            BuildMission01Internal(true);
        }

        public static string BuildMission01ForAutomation()
        {
            return BuildMission01Internal(false);
        }

        private static string BuildMission01Internal(bool showDialog)
        {
            if (showDialog && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return null;
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            RenderSettings.ambientLight = new Color(0.72f, 0.64f, 0.48f);
            RenderSettings.fog = true;
            RenderSettings.fogColor = new Color(0.78f, 0.65f, 0.45f);
            RenderSettings.fogDensity = 0.0025f;

            Material sand = MakeMaterial("DQ Sand", new Color(0.68f, 0.52f, 0.31f));
            Material concrete = MakeMaterial("DQ Concrete", new Color(0.43f, 0.42f, 0.36f));
            Material olive = MakeMaterial("DQ Olive", new Color(0.20f, 0.25f, 0.16f));
            Material amber = MakeMaterial("DQ Objective", new Color(1f, 0.58f, 0.08f));

            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Desert Ground";
            ground.transform.localScale = new Vector3(30f, 1f, 30f);
            ground.GetComponent<Renderer>().sharedMaterial = sand;

            GameObject sunObject = new GameObject("Desert Sun");
            Light sun = sunObject.AddComponent<Light>();
            sun.type = LightType.Directional;
            sun.intensity = 1.25f;
            sunObject.transform.rotation = Quaternion.Euler(48f, -32f, 0f);

            Wall("North Perimeter", new Vector3(0, 2, 110), new Vector3(220, 4, 3), concrete);
            Wall("South Perimeter", new Vector3(0, 2, -110), new Vector3(220, 4, 3), concrete);
            Wall("East Perimeter", new Vector3(110, 2, 0), new Vector3(3, 4, 220), concrete);
            Wall("West Perimeter", new Vector3(-110, 2, 0), new Vector3(3, 4, 220), concrete);

            GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            player.name = "Player";
            player.tag = "Player";
            player.transform.position = new Vector3(0, 1.1f, -65f);
            Object.DestroyImmediate(player.GetComponent<CapsuleCollider>());
            CharacterController cc = player.AddComponent<CharacterController>();
            cc.height = 2f;
            cc.radius = 0.45f;
            player.AddComponent<ThirdPersonMobileController>();
            player.AddComponent<TouchMovePad>();
            player.AddComponent<SniperWeapon>();
            player.AddComponent<PlayerHealth>();

            GameObject cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            cameraObject.AddComponent<Camera>().fieldOfView = 62f;
            cameraObject.AddComponent<AudioListener>();
            cameraObject.transform.position = new Vector3(0, 5, -73);
            cameraObject.AddComponent<SimpleCameraFollow>().SetTarget(player.transform);

            Mission01Controller mission = new GameObject("Mission 01 Controller").AddComponent<Mission01Controller>();
            Trigger("Base Entry Zone", new Vector3(0, 2, -5), new Vector3(24, 5, 12), amber, mission, MissionTrigger.ActionType.EnterBase, false);

            GameObject hostage = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            hostage.name = "Hostage - Rescue";
            hostage.transform.position = new Vector3(0, 1, 35);
            hostage.GetComponent<Renderer>().sharedMaterial = amber;
            Trigger("Hostage Rescue Zone", new Vector3(0, 1, 35), new Vector3(6, 3, 6), amber, mission, MissionTrigger.ActionType.RescueHostage, true);
            Trigger("Extraction Zone", new Vector3(0, 1, 82), new Vector3(12, 3, 12), amber, mission, MissionTrigger.ActionType.Extract, false);

            for (int i = 0; i < 6; i++)
            {
                GameObject guard = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                guard.name = "Guard Patrol " + (i + 1);
                guard.transform.position = new Vector3((i % 2 == 0 ? -1 : 1) * (18 + i * 4), 1, 5 + i * 18);
                guard.GetComponent<Renderer>().sharedMaterial = olive;
                guard.AddComponent<GuardAI>();
                guard.AddComponent<TargetHealth>();
            }
            for (int i = 0; i < 14; i++)
            {
                GameObject crate = GameObject.CreatePrimitive(PrimitiveType.Cube);
                crate.name = "Supply Crate";
                crate.transform.position = new Vector3(Mathf.Sin(i * 2.1f) * 75f, 1f, Mathf.Cos(i * 1.7f) * 75f);
                crate.transform.localScale = new Vector3(2, 2, 2);
                crate.GetComponent<Renderer>().sharedMaterial = olive;
            }

            const string scenePath = "Assets/Scenes/Mission01_DesertBase.unity";
            EditorSceneManager.SaveScene(scene, scenePath);
            AssetDatabase.SaveAssets();
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(scenePath, true)
            };
            if (showDialog)
                EditorUtility.DisplayDialog("Desert Quiet", "Mission 01 scene created and added to Build Settings.", "OK");
            return scenePath;
        }

        private static void Wall(string name, Vector3 position, Vector3 scale, Material material)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name; go.transform.position = position; go.transform.localScale = scale;
            go.GetComponent<Renderer>().sharedMaterial = material;
        }

        private static void Trigger(string name, Vector3 position, Vector3 size, Material material, Mission01Controller mission, MissionTrigger.ActionType action, bool interact)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name; go.transform.position = position; go.transform.localScale = size;
            go.GetComponent<Renderer>().sharedMaterial = material;
            go.GetComponent<Renderer>().enabled = false;
            go.GetComponent<BoxCollider>().isTrigger = true;
            go.AddComponent<MissionTrigger>().Configure(mission, action, interact);
        }

        private static Material MakeMaterial(string name, Color color)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            Material material = new Material(shader);
            material.name = name; material.color = color;
            return material;
        }
    }
}
#endif