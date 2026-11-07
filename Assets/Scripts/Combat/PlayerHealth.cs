using UnityEngine;
using UnityEngine.SceneManagement;

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
            float scale = Mathf.Clamp(Screen.width / 720f, 0.75f, 1.35f);
            Matrix4x4 oldMatrix = GUI.matrix;
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(scale, scale, 1f));

            float barWidth = 190f;
            float barX = 18f;
            float barY = 18f;
            GUI.color = new Color(0f, 0f, 0f, 0.72f);
            GUI.Box(new Rect(barX, barY, barWidth + 20f, 58f), GUIContent.none);
            GUI.color = Color.white;
            GUI.Label(new Rect(barX + 10f, barY + 5f, barWidth, 22f), status);
            GUI.color = new Color(0.18f, 0.18f, 0.18f, 1f);
            GUI.Box(new Rect(barX + 10f, barY + 31f, barWidth, 13f), GUIContent.none);
            GUI.color = currentHealth > 30 ? new Color(0.2f, 0.75f, 0.25f) : new Color(0.9f, 0.18f, 0.12f);
            GUI.Box(new Rect(barX + 10f, barY + 31f, barWidth * currentHealth / Mathf.Max(1f, maxHealth), 13f), GUIContent.none);
            GUI.color = Color.white;

            if (currentHealth <= 0)
            {
                float panelWidth = 280f;
                float panelHeight = 110f;
                float x = (Screen.width / scale - panelWidth) * 0.5f;
                float y = (Screen.height / scale - panelHeight) * 0.5f;
                GUI.Box(new Rect(x, y, panelWidth, panelHeight), "انتهت المهمة");
                if (GUI.Button(new Rect(x + 35f, y + 55f, panelWidth - 70f, 38f), "إعادة المحاولة"))
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }

            GUI.matrix = oldMatrix;
        }
    }
}