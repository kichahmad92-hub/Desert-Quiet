using UnityEngine;

namespace DesertQuiet.Combat
{
    public sealed class PlayerHealth : MonoBehaviour
    {
        [SerializeField] private int maxHealth = 100;
        private int currentHealth;
        private string status = "الصحة: 100";

        private void Awake() { currentHealth = maxHealth; }

        public void ApplyDamage(int amount)
        {
            if (amount <= 0 || currentHealth <= 0) return;
            currentHealth = Mathf.Max(0, currentHealth - amount);
            status = "الصحة: " + currentHealth;
            if (currentHealth == 0) status = "تم إسقاطك - أعد تشغيل المهمة";
        }

        private void OnGUI()
        {
            GUI.Label(new Rect(18f, 18f, 230f, 34f), status);
        }
    }
}