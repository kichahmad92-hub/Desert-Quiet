using UnityEngine;

namespace DesertQuiet.World
{
    /// <summary>Lightweight procedural idle and walk motion for the modular soldier visual.</summary>
    public sealed class SoldierProceduralAnimator : MonoBehaviour
    {
        [SerializeField] private float walkFrequency = 7.5f;
        [SerializeField] private float walkAngle = 27f;
        [SerializeField] private float idleAmount = 1.2f;

        private Transform[] arms;
        private Transform[] legs;
        private Transform head;
        private Vector3 previousPosition;
        private float phase;

        private void Awake()
        {
            Transform[] all = GetComponentsInChildren<Transform>(true);
            System.Collections.Generic.List<Transform> armParts = new System.Collections.Generic.List<Transform>();
            System.Collections.Generic.List<Transform> legParts = new System.Collections.Generic.List<Transform>();

            foreach (Transform item in all)
            {
                if (item == transform) continue;
                if (item.name == "Upper Arm" || item.name == "Forearm") armParts.Add(item);
                if (item.name == "Thigh" || item.name == "Lower Leg") legParts.Add(item);
                if (item.name == "Head") head = item;
            }

            arms = armParts.ToArray();
            legs = legParts.ToArray();
            previousPosition = transform.position;
        }

        private void Update()
        {
            Vector3 delta = transform.position - previousPosition;
            previousPosition = transform.position;
            delta.y = 0f;
            float speed = delta.magnitude / Mathf.Max(Time.deltaTime, 0.0001f);
            float movement = Mathf.Clamp01(speed / 2.5f);
            phase += Time.deltaTime * Mathf.Lerp(1.8f, walkFrequency, movement);

            float swing = Mathf.Sin(phase) * walkAngle * movement;
            float idle = Mathf.Sin(Time.time * 1.7f) * idleAmount;

            for (int i = 0; i < arms.Length; i++)
            {
                Transform part = arms[i];
                float side = part.localPosition.x < 0f ? -1f : 1f;
                float forearmBias = part.name == "Forearm" ? -8f : 0f;
                Vector3 baseEuler = part.name == "Upper Arm"
                    ? new Vector3(0f, 0f, -side * 13f)
                    : new Vector3(0f, 0f, side * 8f);
                part.localRotation = Quaternion.Euler(baseEuler.x + swing * side + forearmBias,
                    baseEuler.y, baseEuler.z);
            }

            for (int i = 0; i < legs.Length; i++)
            {
                Transform part = legs[i];
                float side = part.localPosition.x < 0f ? -1f : 1f;
                float offset = part.name == "Lower Leg" ? -10f : 0f;
                part.localRotation = Quaternion.Euler(swing * -side + offset, 0f, 0f);
            }

            if (head != null)
                head.localRotation = Quaternion.Euler(0f, Mathf.Sin(Time.time * 0.65f) * 2f, 0f);

            transform.localPosition = new Vector3(0f,
                Mathf.Sin(phase * 2f) * 0.035f * movement + Mathf.Sin(Time.time * 1.7f) * 0.008f,
                0f);
        }
    }
}