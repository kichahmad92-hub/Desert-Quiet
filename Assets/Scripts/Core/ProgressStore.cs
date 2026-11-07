using UnityEngine;

namespace DesertQuiet.Core
{
    public static class ProgressStore
    {
        private const string Mission01CompleteKey = "DQ_Mission01_Complete";
        private const string CreditsKey = "DQ_Credits";

        public static bool Mission01Complete => PlayerPrefs.GetInt(Mission01CompleteKey, 0) == 1;
        public static int Credits => PlayerPrefs.GetInt(CreditsKey, 0);

        public static void CompleteMission01(int reward)
        {
            if (Mission01Complete) return;
            PlayerPrefs.SetInt(Mission01CompleteKey, 1);
            PlayerPrefs.SetInt(CreditsKey, Credits + Mathf.Max(0, reward));
            PlayerPrefs.Save();
        }

        public static void ResetProgress()
        {
            PlayerPrefs.DeleteKey(Mission01CompleteKey);
            PlayerPrefs.DeleteKey(CreditsKey);
            PlayerPrefs.Save();
        }
    }
}