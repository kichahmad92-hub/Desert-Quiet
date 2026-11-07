using UnityEngine;
using DesertQuiet.Core;

namespace DesertQuiet.Missions
{
    public sealed class Mission01Controller : MonoBehaviour
    {
        public enum Stage { ReachBase, RescueHostage, ReachExtraction, Completed }
        [SerializeField] private Stage stage = Stage.ReachBase;
        private string status = "تسلل إلى القاعدة وابحث عن الرهينة.";
        public Stage CurrentStage => stage;

        public void ReachBase()
        {
            if (stage != Stage.ReachBase) return;
            stage = Stage.RescueHostage;
            status = "وصلت إلى القاعدة. اقترب من الرهينة واضغط إنقاذ.";
        }
        public void RescueHostage()
        {
            if (stage != Stage.RescueHostage) return;
            stage = Stage.ReachExtraction;
            status = "تم إنقاذ الرهينة. توجّه إلى نقطة الاستخراج.";
        }
        public void CompleteMission()
        {
            if (stage != Stage.ReachExtraction) return;
            stage = Stage.Completed;
            ProgressStore.CompleteMission01(1200);
            status = "اكتملت المهمة بنجاح! الرصيد: " + ProgressStore.Credits + " نقطة.";
        }
        private void OnGUI()
        {
            GUI.color = new Color(0.06f, 0.08f, 0.07f, 0.88f);
            GUI.Box(new Rect(16, 16, 380, 112), GUIContent.none);
            GUI.color = Color.white;
            GUI.Label(new Rect(30, 25, 350, 25), "إنقاذ في الصحراء");
            GUI.Label(new Rect(30, 53, 350, 25), "الهدف: " + StageText());
            GUI.Label(new Rect(30, 82, 350, 35), status);
        }
        private string StageText()
        {
            switch (stage)
            {
                case Stage.ReachBase: return "الوصول إلى القاعدة";
                case Stage.RescueHostage: return "إنقاذ الرهينة";
                case Stage.ReachExtraction: return "الوصول إلى الاستخراج";
                default: return "مكتملة";
            }
        }
    }
}