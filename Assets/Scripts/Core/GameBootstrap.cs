using UnityEngine;

namespace DesertQuiet.Core
{
    /// <summary>
    /// Lightweight scene bootstrap. Add this component to a single
    /// GameObject in the startup scene when that scene is created.
    /// </summary>
    public sealed class GameBootstrap : MonoBehaviour
    {
        private void Awake()
        {
            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }
    }
}
