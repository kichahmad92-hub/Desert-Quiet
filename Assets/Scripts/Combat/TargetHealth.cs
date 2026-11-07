using UnityEngine;

namespace DesertQuiet.Combat
{
    public sealed class TargetHealth : MonoBehaviour
    {
        [SerializeField] private int health = 1;
        [SerializeField] private bool disableOnDefeat = true;
        public bool IsDefeated { get; private set; }

        public void ApplyHit(int damage)
        {
            if (IsDefeated || damage <= 0) return;
            health -= damage;
            if (health > 0) return;
            IsDefeated = true;
            if (disableOnDefeat) gameObject.SetActive(false);
        }
    }
}