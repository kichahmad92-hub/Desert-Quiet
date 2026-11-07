using UnityEngine;
namespace DesertQuiet.World
{
    public sealed class SimpleCameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform target;
        public void SetTarget(Transform value) { target = value; }
        private void LateUpdate()
        {
            if (target == null) return;
            Vector3 desired = target.position - target.forward * 6f + Vector3.up * 3f;
            transform.position = Vector3.Lerp(transform.position, desired, Time.deltaTime * 5f);
            transform.LookAt(target.position + Vector3.up * 1.2f);
        }
    }
    public sealed class SimpleGuardPatrol : MonoBehaviour
    {
        private Vector3 origin;
        private void Start() { origin = transform.position; }
        private void Update()
        {
            float offset = Mathf.Sin(Time.time * 0.6f + origin.x) * 5f;
            transform.position = new Vector3(origin.x + offset, origin.y, origin.z);
        }
    }
}