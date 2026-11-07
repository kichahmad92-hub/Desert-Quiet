using UnityEngine;

namespace DesertQuiet.World
{
    /// <summary>Builds a modular, lightweight desert infantry character from Unity primitives.</summary>
    public static class SoldierVisualFactory
    {
        public static void Create(Transform root, Material uniform, Material skin, Material armor,
            Material equipment, Material boots, bool addRifle)
        {
            Part(root, "Uniform Torso", PrimitiveType.Capsule, new Vector3(0f, 0.22f, 0f),
                new Vector3(0.72f, 0.88f, 0.42f), uniform, Vector3.zero);
            Part(root, "Plate Carrier", PrimitiveType.Cube, new Vector3(0f, 0.28f, 0.245f),
                new Vector3(0.66f, 0.62f, 0.16f), armor, Vector3.zero);
            Part(root, "Backpack", PrimitiveType.Cube, new Vector3(0f, 0.34f, -0.29f),
                new Vector3(0.58f, 0.72f, 0.28f), equipment, Vector3.zero);
            Part(root, "Neck", PrimitiveType.Cylinder, new Vector3(0f, 0.78f, 0f),
                new Vector3(0.16f, 0.16f, 0.16f), skin, Vector3.zero);
            Part(root, "Head", PrimitiveType.Sphere, new Vector3(0f, 0.98f, 0.015f),
                new Vector3(0.34f, 0.43f, 0.32f), skin, Vector3.zero);
            Part(root, "Combat Helmet", PrimitiveType.Sphere, new Vector3(0f, 1.15f, 0f),
                new Vector3(0.40f, 0.27f, 0.38f), uniform, Vector3.zero);
            Part(root, "Helmet Front Rim", PrimitiveType.Cube, new Vector3(0f, 1.08f, 0.19f),
                new Vector3(0.38f, 0.045f, 0.13f), equipment, Vector3.zero);
            Part(root, "Face Cover", PrimitiveType.Cube, new Vector3(0f, 0.87f, 0.155f),
                new Vector3(0.25f, 0.13f, 0.045f), uniform, Vector3.zero);

            for (int side = -1; side <= 1; side += 2)
            {
                Part(root, "Upper Arm", PrimitiveType.Capsule, new Vector3(side * 0.43f, 0.28f, 0.015f),
                    new Vector3(0.23f, 0.48f, 0.23f), uniform, new Vector3(0f, 0f, -side * 13f));
                Part(root, "Forearm", PrimitiveType.Capsule, new Vector3(side * 0.49f, -0.13f, 0.10f),
                    new Vector3(0.19f, 0.42f, 0.19f), uniform, new Vector3(0f, 0f, side * 8f));
                Part(root, "Glove", PrimitiveType.Sphere, new Vector3(side * 0.51f, -0.39f, 0.13f),
                    new Vector3(0.20f, 0.17f, 0.19f), boots, Vector3.zero);
                Part(root, "Utility Pouch", PrimitiveType.Cube, new Vector3(side * 0.23f, 0.02f, 0.34f),
                    new Vector3(0.19f, 0.22f, 0.12f), equipment, Vector3.zero);
                Part(root, "Thigh", PrimitiveType.Capsule, new Vector3(side * 0.19f, -0.43f, 0f),
                    new Vector3(0.31f, 0.57f, 0.31f), uniform, Vector3.zero);
                Part(root, "Knee Pad", PrimitiveType.Cube, new Vector3(side * 0.19f, -0.68f, 0.17f),
                    new Vector3(0.25f, 0.20f, 0.10f), armor, Vector3.zero);
                Part(root, "Lower Leg", PrimitiveType.Capsule, new Vector3(side * 0.19f, -0.83f, 0f),
                    new Vector3(0.25f, 0.42f, 0.25f), uniform, Vector3.zero);
                Part(root, "Combat Boot", PrimitiveType.Cube, new Vector3(side * 0.19f, -1.02f, 0.09f),
                    new Vector3(0.29f, 0.18f, 0.40f), boots, Vector3.zero);
            }

            Part(root, "Belt", PrimitiveType.Cube, new Vector3(0f, -0.12f, 0.02f),
                new Vector3(0.68f, 0.12f, 0.48f), equipment, Vector3.zero);
            Part(root, "Chest Magazine Pouch", PrimitiveType.Cube, new Vector3(0f, 0.30f, 0.34f),
                new Vector3(0.34f, 0.20f, 0.10f), equipment, Vector3.zero);
            Part(root, "Shoulder Patch", PrimitiveType.Cube, new Vector3(0.39f, 0.47f, 0.12f),
                new Vector3(0.13f, 0.12f, 0.06f), armor, Vector3.zero);

            if (addRifle)
            {
                Part(root, "Service Rifle Receiver", PrimitiveType.Cube, new Vector3(0.34f, 0.12f, 0.39f),
                    new Vector3(0.13f, 0.14f, 0.72f), equipment, new Vector3(8f, 0f, -8f));
                Part(root, "Rifle Barrel", PrimitiveType.Cylinder, new Vector3(0.34f, 0.14f, 0.79f),
                    new Vector3(0.035f, 0.34f, 0.035f), boots, new Vector3(90f, 0f, 0f));
                Part(root, "Rifle Stock", PrimitiveType.Cube, new Vector3(0.34f, 0.12f, 0.02f),
                    new Vector3(0.12f, 0.13f, 0.25f), uniform, Vector3.zero);
            }
        }

        private static void Part(Transform parent, string partName, PrimitiveType shape, Vector3 position,
            Vector3 scale, Material material, Vector3 rotation)
        {
            GameObject part = GameObject.CreatePrimitive(shape);
            part.name = partName;
            part.transform.SetParent(parent, false);
            part.transform.localPosition = position;
            part.transform.localRotation = Quaternion.Euler(rotation);
            part.transform.localScale = scale;
            Renderer renderer = part.GetComponent<Renderer>();
            renderer.sharedMaterial = material;
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            Collider collider = part.GetComponent<Collider>();
            if (collider != null) Object.DestroyImmediate(collider);
        }
    }
}