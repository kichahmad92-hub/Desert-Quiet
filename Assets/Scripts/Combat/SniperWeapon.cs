using UnityEngine;

namespace DesertQuiet.Combat
{
    public sealed class SniperWeapon : MonoBehaviour
    {
        [SerializeField] private Camera aimCamera;
        [SerializeField] private float range = 250f;
        [SerializeField] private int damage = 1;
        [SerializeField] private float fireCooldown = 1.1f;
        [SerializeField] private float scopeFieldOfView = 22f;
        [SerializeField] private LayerMask hitMask = ~0;

        private float nextFireTime;
        private float normalFieldOfView = 60f;
        private bool isScoped;
        private string feedback = "جاهز";

        private void Awake()
        {
            if (aimCamera == null) aimCamera = Camera.main;
            if (aimCamera != null) normalFieldOfView = aimCamera.fieldOfView;
        }

        public void ToggleScope()
        {
            if (aimCamera == null)
            {
                feedback = "الكاميرا غير متصلة";
                return;
            }

            isScoped = !isScoped;
            aimCamera.fieldOfView = isScoped ? scopeFieldOfView : normalFieldOfView;
            feedback = isScoped ? "منظار القنص مفعّل" : "تم إلغاء المنظار";
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
            Rect scopeButton = new Rect(Screen.width - (size * 2f) - 38f, Screen.height - size - 28f, size, size);

            if (GUI.Button(fireButton, "إطلاق")) Fire();
            if (GUI.Button(scopeButton, isScoped ? "إلغاء المنظار" : "منظار")) ToggleScope();

            GUI.Label(new Rect(Screen.width - 210f, Screen.height - 125f, 200f, 28f), feedback);
            float centerX = Screen.width * 0.5f;
            float centerY = Screen.height * 0.5f;
            GUI.Label(new Rect(centerX - 8f, centerY - 14f, 16f, 28f), "+");

            if (isScoped)
            {
                GUI.Box(new Rect(centerX - 2f, 0f, 4f, Screen.height), GUIContent.none);
                GUI.Box(new Rect(0f, centerY - 2f, Screen.width, 4f), GUIContent.none);
                GUI.Label(new Rect(centerX - 70f, 18f, 140f, 30f), "وضع القنص");
            }
        }

        private void OnDisable()
        {
            if (aimCamera != null) aimCamera.fieldOfView = normalFieldOfView;
            isScoped = false;
        }
    }
}