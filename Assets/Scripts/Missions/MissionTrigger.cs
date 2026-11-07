using UnityEngine;
namespace DesertQuiet.Missions
{
    public sealed class MissionTrigger : MonoBehaviour
    {
        public enum ActionType { EnterBase, RescueHostage, Extract }
        [SerializeField] private ActionType action;
        [SerializeField] private Mission01Controller mission;
        [SerializeField] private bool requireInteract;
        private bool inside;
        private bool activated;
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            inside = true;
            if (!requireInteract) Activate();
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player")) inside = false;
        }
        private void Update()
        {
            if (inside && requireInteract && !activated && Input.GetKeyDown(KeyCode.E)) Activate();
        }
        private void Activate()
        {
            if (activated || mission == null) return;
            activated = true;
            if (action == ActionType.EnterBase) mission.ReachBase();
            else if (action == ActionType.RescueHostage) mission.RescueHostage();
            else mission.CompleteMission();
        }
        public void Configure(Mission01Controller target, ActionType type, bool interact)
        {
            mission = target; action = type; requireInteract = interact;
        }
    }
}