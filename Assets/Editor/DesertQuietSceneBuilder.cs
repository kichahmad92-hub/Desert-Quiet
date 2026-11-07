#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using DesertQuiet.Player;
using DesertQuiet.Missions;
using DesertQuiet.World;
using DesertQuiet.Combat;
using DesertQuiet.Audio;

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
            Material soldierUniform = MakeMaterial("DQ Soldier Desert Uniform", new Color(0.42f, 0.39f, 0.27f));
            Material soldierSkin = MakeMaterial("DQ Soldier Skin", new Color(0.55f, 0.37f, 0.25f));
            Material soldierArmor = MakeMaterial("DQ Soldier Armor", new Color(0.20f, 0.23f, 0.17f));
            Material soldierGear = MakeMaterial("DQ Soldier Gear", new Color(0.12f, 0.14f, 0.11f));
            Material soldierBoots = MakeMaterial("DQ Soldier Boots", new Color(0.10f, 0.085f, 0.065f));

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
            player.GetComponent<Renderer>().sharedMaterial = olive;
            player.GetComponent<Renderer>().enabled = false;
            SoldierVisualFactory.Create(player.transform, soldierUniform, soldierSkin, soldierArmor,
                soldierGear, soldierBoots, true);
            player.AddComponent<SoldierProceduralAnimator>();
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
            Camera gameCamera = cameraObject.AddComponent<Camera>();
            gameCamera.fieldOfView = 62f;
            gameCamera.clearFlags = CameraClearFlags.SolidColor;
            gameCamera.backgroundColor = new Color(0.48f, 0.68f, 0.82f);
            cameraObject.AddComponent<AudioListener>();
            AudioSource desertWind = cameraObject.AddComponent<AudioSource>();
            desertWind.clip = DesertAudio.Wind;
            desertWind.loop = true;
            desertWind.volume = 0.18f;
            desertWind.spatialBlend = 0f;
            desertWind.Play();
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
                guard.GetComponent<Renderer>().enabled = false;
                SoldierVisualFactory.Create(guard.transform, soldierUniform, soldierSkin, soldierArmor,
                    soldierGear, soldierBoots, true);
                guard.AddComponent<SoldierProceduralAnimator>();
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

            // Extra desert-base scenery: watchtowers, shelters, rocks and fuel drums.
            Material rock = MakeMaterial("DQ Desert Rock", new Color(0.39f, 0.31f, 0.23f));
            Material canvas = MakeMaterial("DQ Tent Canvas", new Color(0.48f, 0.39f, 0.25f));
            Material metal = MakeMaterial("DQ Dark Metal", new Color(0.16f, 0.18f, 0.17f));

            CreateWatchtower("West Watchtower", new Vector3(-58f, 0f, 18f), concrete, metal);
            CreateWatchtower("East Watchtower", new Vector3(58f, 0f, 18f), concrete, metal);
            CreateWatchtower("North Watchtower", new Vector3(0f, 0f, 88f), concrete, metal);
            CreateTent("Command Tent", new Vector3(-24f, 0f, 24f), canvas, metal);
            CreateTent("Medical Tent", new Vector3(25f, 0f, 48f), canvas, amber);

            for (int i = 0; i < 28; i++)
            {
                float angle = i * 2.399f;
                float radius = 42f + (i % 5) * 11f;
                Vector3 position = new Vector3(Mathf.Cos(angle) * radius, 0.8f, Mathf.Sin(angle) * radius);
                GameObject boulder = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                boulder.name = "Desert Boulder";
                boulder.transform.position = position;
                boulder.transform.localScale = new Vector3(2.2f + (i % 3), 1.4f + (i % 2), 2f + (i % 4));
                boulder.transform.rotation = Quaternion.Euler(i * 13f, i * 29f, i * 7f);
                boulder.GetComponent<Renderer>().sharedMaterial = rock;
            }

            for (int i = 0; i < 10; i++)
            {
                GameObject drum = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                drum.name = "Fuel Drum";
                drum.transform.position = new Vector3(-12f + (i % 5) * 6f, 0.8f, 8f + (i / 5) * 7f);
                drum.transform.localScale = new Vector3(1.1f, 0.8f, 1.1f);
                drum.GetComponent<Renderer>().sharedMaterial = metal;
            }

            // Defensive cover around the base entrance.
            Material sandbag = MakeMaterial("DQ Sandbags", new Color(0.55f, 0.43f, 0.29f));
            CreateBox("Sandbag Cover West", new Vector3(-13f, 0.65f, -2f), new Vector3(7f, 1.3f, 1.2f), sandbag);
            CreateBox("Sandbag Cover East", new Vector3(13f, 0.65f, -2f), new Vector3(7f, 1.3f, 1.2f), sandbag);
            CreateBox("Sandbag Cover North", new Vector3(0f, 0.65f, 12f), new Vector3(9f, 1.3f, 1.2f), sandbag);

            CreateSearchlight("West Searchlight", new Vector3(-58f, 10f, 18f), new Vector3(0.3f, -0.45f, 1f));
            CreateSearchlight("East Searchlight", new Vector3(58f, 10f, 18f), new Vector3(-0.3f, -0.45f, 1f));
            CreateSearchlight("North Searchlight", new Vector3(0f, 10f, 88f), new Vector3(0f, -0.45f, -1f));
            // Parked military vehicles with detailed exterior parts.
            Material vehicleOlive = MakeMaterial("DQ Vehicle Olive", new Color(0.19f, 0.23f, 0.15f));
            Material vehicleRubber = MakeMaterial("DQ Tire Rubber", new Color(0.035f, 0.04f, 0.035f));
            Material vehicleGlass = MakeMaterial("DQ Armored Glass", new Color(0.10f, 0.17f, 0.19f));
            Material vehicleMetal = MakeMaterial("DQ Vehicle Metal", new Color(0.11f, 0.13f, 0.12f));
            Material headlamp = MakeMaterial("DQ Headlamp", new Color(1f, 0.82f, 0.48f));
            Material tailLamp = MakeMaterial("DQ Tail Lamp", new Color(0.65f, 0.045f, 0.025f));
            CreateMilitaryJeep("Armored Patrol Jeep", new Vector3(-34f, 0f, -18f), 24f,
                vehicleOlive, vehicleRubber, vehicleGlass, vehicleMetal, headlamp, tailLamp);
            CreateSupplyTruck("Military Supply Truck", new Vector3(34f, 0f, 32f), -18f,
                vehicleOlive, vehicleRubber, vehicleGlass, vehicleMetal, headlamp, tailLamp);


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

        private static void CreateSearchlight(string name, Vector3 position, Vector3 direction)
        {
            GameObject lightObject = new GameObject(name);
            lightObject.transform.position = position;
            lightObject.transform.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
            Light searchlight = lightObject.AddComponent<Light>();
            searchlight.type = LightType.Spot;
            searchlight.color = new Color(1f, 0.86f, 0.62f);
            searchlight.intensity = 2.2f;
            searchlight.range = 42f;
            searchlight.spotAngle = 58f;
            searchlight.shadows = LightShadows.None;
        }

        private static GameObject CreateVehiclePart(Transform parent, string name, PrimitiveType type,
            Vector3 position, Vector3 scale, Material material, Vector3 rotation)
        {
            GameObject part = GameObject.CreatePrimitive(type);
            part.name = name;
            part.transform.SetParent(parent, false);
            part.transform.localPosition = position;
            part.transform.localRotation = Quaternion.Euler(rotation);
            part.transform.localScale = scale;
            part.GetComponent<Renderer>().sharedMaterial = material;
            return part;
        }

        private static void CreateMilitaryJeep(string name, Vector3 position, float yaw,
            Material body, Material rubber, Material glass, Material metal, Material lamps, Material rearLamps)
        {
            GameObject root = new GameObject(name);
            root.transform.position = position;
            root.transform.rotation = Quaternion.Euler(0f, yaw, 0f);

            CreateVehiclePart(root.transform, "Armored Chassis", PrimitiveType.Cube, new Vector3(0f, 0.9f, 0f), new Vector3(2.25f, 0.65f, 4.1f), body, Vector3.zero);
            CreateVehiclePart(root.transform, "Engine Hood", PrimitiveType.Cube, new Vector3(0f, 1.42f, 1.25f), new Vector3(2.05f, 0.48f, 1.35f), body, Vector3.zero);
            CreateVehiclePart(root.transform, "Cabin Roof", PrimitiveType.Cube, new Vector3(0f, 2.35f, -0.35f), new Vector3(1.95f, 0.18f, 1.85f), body, Vector3.zero);
            CreateVehiclePart(root.transform, "Front Windshield", PrimitiveType.Cube, new Vector3(0f, 1.95f, 0.68f), new Vector3(1.78f, 0.72f, 0.08f), glass, new Vector3(-12f, 0f, 0f));
            CreateVehiclePart(root.transform, "Rear Window", PrimitiveType.Cube, new Vector3(0f, 1.95f, -1.35f), new Vector3(1.65f, 0.62f, 0.07f), glass, new Vector3(12f, 0f, 0f));
            for (int side = -1; side <= 1; side += 2)
            {
                CreateVehiclePart(root.transform, "Side Window", PrimitiveType.Cube, new Vector3(side * 0.99f, 1.98f, -0.32f), new Vector3(0.06f, 0.62f, 1.45f), glass, Vector3.zero);
                CreateVehiclePart(root.transform, "Door Armor", PrimitiveType.Cube, new Vector3(side * 1.04f, 1.22f, -0.38f), new Vector3(0.12f, 0.72f, 1.45f), body, Vector3.zero);
                for (int axle = -1; axle <= 1; axle += 2)
                    CreateVehiclePart(root.transform, "All Terrain Tire", PrimitiveType.Cylinder, new Vector3(side * 1.18f, 0.67f, axle * 1.35f), new Vector3(0.78f, 0.28f, 0.78f), rubber, new Vector3(0f, 0f, 90f));
                CreateVehiclePart(root.transform, "Side Mirror", PrimitiveType.Cube, new Vector3(side * 1.18f, 1.85f, 0.62f), new Vector3(0.22f, 0.12f, 0.28f), metal, Vector3.zero);
            }
            CreateVehiclePart(root.transform, "Front Bumper", PrimitiveType.Cube, new Vector3(0f, 0.68f, 2.12f), new Vector3(2.45f, 0.22f, 0.22f), metal, Vector3.zero);
            CreateVehiclePart(root.transform, "Rear Bumper", PrimitiveType.Cube, new Vector3(0f, 0.68f, -2.12f), new Vector3(2.35f, 0.2f, 0.2f), metal, Vector3.zero);
            CreateVehiclePart(root.transform, "Front Grille", PrimitiveType.Cube, new Vector3(0f, 1.18f, 2.08f), new Vector3(0.72f, 0.42f, 0.06f), metal, Vector3.zero);
            for (int x = -1; x <= 1; x += 2)
            {
                CreateVehiclePart(root.transform, "Headlight", PrimitiveType.Cylinder, new Vector3(x * 0.78f, 1.38f, 2.08f), new Vector3(0.28f, 0.07f, 0.28f), lamps, new Vector3(90f, 0f, 0f));
                CreateVehiclePart(root.transform, "Rear Lamp", PrimitiveType.Cube, new Vector3(x * 0.82f, 1.22f, -2.08f), new Vector3(0.22f, 0.28f, 0.08f), rearLamps, Vector3.zero);
            }
            CreateVehiclePart(root.transform, "Spare Tire", PrimitiveType.Cylinder, new Vector3(0f, 1.45f, -2.22f), new Vector3(0.82f, 0.28f, 0.82f), rubber, new Vector3(90f, 0f, 0f));
            CreateVehiclePart(root.transform, "Radio Antenna", PrimitiveType.Cylinder, new Vector3(0.78f, 2.85f, -0.85f), new Vector3(0.025f, 0.62f, 0.025f), metal, new Vector3(0f, 0f, -8f));
            CreateVehiclePart(root.transform, "Roof Equipment Rack", PrimitiveType.Cube, new Vector3(0f, 2.48f, -0.3f), new Vector3(1.45f, 0.08f, 1.35f), metal, Vector3.zero);
        }

        private static void CreateSupplyTruck(string name, Vector3 position, float yaw,
            Material body, Material rubber, Material glass, Material metal, Material lamps, Material rearLamps)
        {
            GameObject root = new GameObject(name);
            root.transform.position = position;
            root.transform.rotation = Quaternion.Euler(0f, yaw, 0f);

            CreateVehiclePart(root.transform, "Truck Chassis", PrimitiveType.Cube, new Vector3(0f, 0.95f, 0f), new Vector3(2.5f, 0.55f, 5.8f), metal, Vector3.zero);
            CreateVehiclePart(root.transform, "Cargo Bed", PrimitiveType.Cube, new Vector3(0f, 1.8f, -0.95f), new Vector3(2.45f, 1.25f, 3.2f), body, Vector3.zero);
            CreateVehiclePart(root.transform, "Cabin", PrimitiveType.Cube, new Vector3(0f, 1.95f, 1.75f), new Vector3(2.35f, 1.85f, 1.85f), body, Vector3.zero);
            CreateVehiclePart(root.transform, "Truck Windshield", PrimitiveType.Cube, new Vector3(0f, 2.25f, 2.7f), new Vector3(1.95f, 0.85f, 0.08f), glass, Vector3.zero);
            CreateVehiclePart(root.transform, "Truck Roof", PrimitiveType.Cube, new Vector3(0f, 2.92f, 1.75f), new Vector3(2.38f, 0.16f, 1.9f), body, Vector3.zero);
            for (int side = -1; side <= 1; side += 2)
            {
                CreateVehiclePart(root.transform, "Truck Side Window", PrimitiveType.Cube, new Vector3(side * 1.19f, 2.32f, 1.75f), new Vector3(0.06f, 0.72f, 1.25f), glass, Vector3.zero);
                CreateVehiclePart(root.transform, "Cargo Side Rail", PrimitiveType.Cube, new Vector3(side * 1.25f, 2.45f, -0.95f), new Vector3(0.12f, 1.25f, 3.2f), metal, Vector3.zero);
                for (int axle = -1; axle <= 1; axle++)
                    CreateVehiclePart(root.transform, "Truck Tire", PrimitiveType.Cylinder, new Vector3(side * 1.3f, 0.68f, axle * 1.95f), new Vector3(0.9f, 0.3f, 0.9f), rubber, new Vector3(0f, 0f, 90f));
                CreateVehiclePart(root.transform, "Truck Mirror", PrimitiveType.Cube, new Vector3(side * 1.38f, 2.15f, 2.3f), new Vector3(0.22f, 0.12f, 0.3f), metal, Vector3.zero);
            }
            CreateVehiclePart(root.transform, "Truck Front Bumper", PrimitiveType.Cube, new Vector3(0f, 0.78f, 2.78f), new Vector3(2.65f, 0.24f, 0.24f), metal, Vector3.zero);
            CreateVehiclePart(root.transform, "Truck Grille", PrimitiveType.Cube, new Vector3(0f, 1.65f, 2.72f), new Vector3(0.85f, 0.5f, 0.08f), metal, Vector3.zero);
            for (int x = -1; x <= 1; x += 2)
            {
                CreateVehiclePart(root.transform, "Truck Headlight", PrimitiveType.Cylinder, new Vector3(x * 0.88f, 1.72f, 2.72f), new Vector3(0.3f, 0.07f, 0.3f), lamps, new Vector3(90f, 0f, 0f));
                CreateVehiclePart(root.transform, "Truck Rear Lamp", PrimitiveType.Cube, new Vector3(x * 1.05f, 1.25f, -2.92f), new Vector3(0.22f, 0.32f, 0.08f), rearLamps, Vector3.zero);
            }
            CreateVehiclePart(root.transform, "Canvas Cargo Cover", PrimitiveType.Cube, new Vector3(0f, 2.65f, -0.95f), new Vector3(2.28f, 0.12f, 2.95f), body, Vector3.zero);
        }

        private static void CreateWatchtower(string name, Vector3 basePosition, Material concrete, Material metal)
        {
            for (int x = -1; x <= 1; x += 2)
                for (int z = -1; z <= 1; z += 2)
                    CreateBox(name + " Support", basePosition + new Vector3(x * 2f, 3f, z * 2f), new Vector3(0.45f, 6f, 0.45f), metal);
            CreateBox(name + " Platform", basePosition + new Vector3(0f, 6f, 0f), new Vector3(5.2f, 0.5f, 5.2f), concrete);
            CreateBox(name + " Roof", basePosition + new Vector3(0f, 9.2f, 0f), new Vector3(6f, 0.5f, 6f), metal);
            CreateBox(name + " Ladder", basePosition + new Vector3(0f, 3f, -2.5f), new Vector3(0.8f, 5f, 0.35f), concrete);
        }

        private static void CreateTent(string name, Vector3 position, Material canvas, Material trim)
        {
            CreateBox(name + " Base", position + new Vector3(0f, 1f, 0f), new Vector3(8f, 2f, 6f), canvas);
            GameObject roof = GameObject.CreatePrimitive(PrimitiveType.Cube);
            roof.name = name + " Roof";
            roof.transform.position = position + new Vector3(0f, 3f, 0f);
            roof.transform.localScale = new Vector3(8.8f, 0.35f, 6.8f);
            roof.transform.rotation = Quaternion.Euler(0f, 0f, -8f);
            roof.GetComponent<Renderer>().sharedMaterial = trim;
            CreateBox(name + " Entrance", position + new Vector3(0f, 1f, -3.05f), new Vector3(2f, 2f, 0.15f), trim);
        }

        private static void CreateBox(string name, Vector3 position, Vector3 scale, Material material)
        {
            GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
            box.name = name;
            box.transform.position = position;
            box.transform.localScale = scale;
            box.GetComponent<Renderer>().sharedMaterial = material;
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
            // Select a shader that matches the active render pipeline.
            // Using URP/Lit in a Built-in Render Pipeline project renders magenta.
            bool usesSrp = UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline != null;
            Shader shader = usesSrp
                ? Shader.Find("Universal Render Pipeline/Lit")
                : Shader.Find("Standard");
            if (shader == null) shader = Shader.Find("Unlit/Color");
            if (shader == null) shader = Shader.Find("Sprites/Default");
            if (shader == null)
                throw new System.InvalidOperationException("Desert Quiet: No compatible built-in or URP shader was found.");
            Material material = new Material(shader);
            material.name = name; material.color = color;
            return material;
        }
    }
}
#endif