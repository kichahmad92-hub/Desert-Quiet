using UnityEngine;

namespace DesertQuiet.Combat
{
    public sealed class SniperWeapon : MonoBehaviour
    {
        [SerializeField] private Camera aimCamera;
        [SerializeField] private float range = 250f;
        [SerializeField] private int damage = 1;
        [SerializeField] private float fireCooldown = 1.1f;
        [SerializeField] private LayerMask hitMask = ~0;
        private float nextFireTime;
        private string feedback = "جاهز";

        private void Awake()
        {
            if (aimCamera == null) aimCamera = Camera.main;
        }

        public void Fire()
        {
            if (Time.time < nextFireTime) return;
            nextFireTime = Time.time + fireCooldown;
            if (aimCamera == null) { feedback = "الكاميرا غير متصلة"; return; }

            Ray ray = aimCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            if (Physics.Raycast(ray, out RaycastHit hit, range, hitMask, QueryTriggerInteraction.Ignore))
            {
                TargetHealth target = hit.collider.GetComponentInParent<TargetHealth>();
                if (target != null)
                {
                    target.ApplyHit(damage);
                    feedback = "إصابة مؤكدة";
                }
                else feedback = "أصبت الهدف";
            }
            else feedback = "لم تصب الهدف";
        }

        private void OnGUI()
        {
            const float size = 82f;
            Rect fireButton = new Rect(Screen.width - size - 24f, Screen.height - size - 28f, size, size);
            if (GUI.Button(fireButton, "إطلاق")) Fire();
            GUI.Label(new Rect(Screen.width - 190f, Screen.height - 120f, 180f, 28f), feedback);
            GUI.Label(new Rect(Screen.width * 0.5f - 8f, Screen.height * 0.5f - 14f, 16f, 28f), "+");
        }
    }
}