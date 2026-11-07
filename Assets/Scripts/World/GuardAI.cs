using UnityEngine;
using DesertQuiet.Combat;

namespace DesertQuiet.World
{
    public sealed class GuardAI : MonoBehaviour
    {
        private enum State { Patrol, Investigate, Chase }
        [SerializeField] private float sightRange = 24f;
        [SerializeField] private float sightAngle = 105f;
        [SerializeField] private float hearingRange = 65f;
        [SerializeField] private float patrolSpeed = 1.7f;
        [SerializeField] private float chaseSpeed = 3.6f;
        [SerializeField] private float attackRange = 2.2f;
        [SerializeField] private float attackCooldown = 1.4f;
        private Transform player;
        private Vector3 patrolOrigin;
        private Vector3 investigatePoint;
        private State state;
        private float nextAttack;
        private float stateTimer;

        private void Start()
        {
            patrolOrigin = transform.position;
            GameObject found = GameObject.FindGameObjectWithTag("Player");
            if (found != null) player = found.transform;
        }

        private void Update()
        {
            if (player == null || GetComponent<TargetHealth>()?.IsDefeated == true) return;

            float distance = Vector3.Distance(transform.position, player.position);
            if (distance <= sightRange && CanSeePlayer())
            {
                state = State.Chase;
                stateTimer = 4f;
            }
            else if (state == State.Chase)
            {
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0f) state = State.Investigate;
            }

            if (state == State.Chase)
            {
                MoveTowards(player.position, chaseSpeed);
                if (distance <= attackRange && Time.time >= nextAttack)
                {
                    nextAttack = Time.time + attackCooldown;
                    PlayerHealth health = player.GetComponent<PlayerHealth>();
                    if (health != null) health.ApplyDamage(10);
                }
            }
            else if (state == State.Investigate)
            {
                MoveTowards(investigatePoint, chaseSpeed);
                if (Vector3.Distance(transform.position, investigatePoint) < 1.5f)
                    state = State.Patrol;
            }
            else
            {
                float offset = Mathf.Sin(Time.time * 0.55f + patrolOrigin.x) * 5f;
                Vector3 patrolPoint = patrolOrigin + Vector3.right * offset;
                MoveTowards(patrolPoint, patrolSpeed);
            }
        }

        private bool CanSeePlayer()
        {
            Vector3 toPlayer = player.position + Vector3.up - (transform.position + Vector3.up);
            if (Vector3.Angle(transform.forward, toPlayer) > sightAngle * 0.5f) return false;
            if (Physics.Raycast(transform.position + Vector3.up, toPlayer.normalized,
                out RaycastHit hit, sightRange, ~0, QueryTriggerInteraction.Ignore))
                return hit.transform == player || hit.transform.IsChildOf(player);
            return false;
        }

        private void MoveTowards(Vector3 point, float speed)
        {
            Vector3 direction = point - transform.position;
            direction.y = 0f;
            if (direction.sqrMagnitude < 0.05f) return;
            Quaternion rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 7f * Time.deltaTime);
            transform.position += transform.forward * speed * Time.deltaTime;
        }

        public static void ReportGunshot(Vector3 position)
        {
            GuardAI[] guards = FindObjectsOfType<GuardAI>();
            foreach (GuardAI guard in guards)
            {
                if (guard == null || guard.player == null) continue;
                if (Vector3.Distance(guard.transform.position, position) <= guard.hearingRange)
                {
                    guard.investigatePoint = position;
                    guard.state = State.Investigate;
                    guard.stateTimer = 0f;
                }
            }
        }
    }
}