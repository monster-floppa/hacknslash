using UnityEngine;
using HacknSlash.Combat;

namespace HacknSlash.Enemy
{
    [RequireComponent(typeof(Hurtbox))]
    public class EnemyAI : MonoBehaviour
    {
        [SerializeField] private EnemyCombat combat;
        [SerializeField] private float chaseDistance = 10f;
        [SerializeField] private float attackDistance = 2.5f;
        [SerializeField] private float moveSpeed = 4f;

        private EnemyStateMachine stateMachine = new EnemyStateMachine();
        private Hurtbox hurtbox;
        private Transform player;
        private float stunTimer;

        private void Awake()
        {
            hurtbox = GetComponent<Hurtbox>();
            hurtbox.OnDamageReceived += OnDamageReceived;
            stateMachine.Initialize(EnemyState.Idle);
        }

        private void Start()
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }

        private void OnDestroy()
        {
            if (hurtbox != null)
            {
                hurtbox.OnDamageReceived -= OnDamageReceived;
            }
        }

        private void Update()
        {
            if (!hurtbox.IsAlive)
            {
                stateMachine.ChangeState(EnemyState.Dead);
                return;
            }

            if (player == null)
            {
                stateMachine.ChangeState(EnemyState.Idle);
                return;
            }

            if (stunTimer > 0f)
            {
                stunTimer -= Time.deltaTime;
                stateMachine.ChangeState(EnemyState.Stunned);
                return;
            }

            float distance = Vector3.Distance(transform.position, player.position);

            if (distance > chaseDistance)
            {
                stateMachine.ChangeState(EnemyState.Idle);
                return;
            }

            if (distance > attackDistance)
            {
                stateMachine.ChangeState(EnemyState.Chase);
                ChasePlayer();
                return;
            }

            stateMachine.ChangeState(EnemyState.Attack);
            TryAttack();
        }

        private void ChasePlayer()
        {
            Vector3 dir = (player.position - transform.position);
            dir.y = 0f;
            if (dir.sqrMagnitude < 0.01f)
            {
                return;
            }

            dir.Normalize();
            transform.position += dir * (moveSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 8f);
        }

        private void TryAttack()
        {
            transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
            if (combat != null && combat.CanAttack())
            {
                combat.PerformAttack();
            }
        }

        private void OnDamageReceived(DamageData data, GameObject source)
        {
            if (data == null)
            {
                return;
            }

            stunTimer = data.HitStun;
            if (data.Reaction == DamageReaction.Launch)
            {
                stateMachine.ChangeState(EnemyState.Airborne);
            }
        }
    }
}
