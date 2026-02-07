using UnityEngine;
using HacknSlash.Combat;
using HacknSlash.Systems;

namespace HacknSlash.Player
{
    public class PlayerCombat : MonoBehaviour
    {
        [SerializeField] private PlayerController controller;
        [SerializeField] private Hitbox weaponHitbox;
        [SerializeField] private Transform lockOnAnchor;
        [SerializeField] private LayerMask enemyMask;
        [SerializeField] private float lockOnRadius = 16f;
        [SerializeField] private float comboResetTime = 0.75f;

        [Header("Attacks")]
        [SerializeField] private DamageData lightAttack;
        [SerializeField] private DamageData heavyAttack;
        [SerializeField] private DamageData launcherAttack;
        [SerializeField] private DamageData slamAttack;

        private int comboStep;
        private float comboTimer;
        private bool lockOnEnabled;
        private Transform currentTarget;
        private StyleManager styleManager;

        private void Awake()
        {
            styleManager = FindObjectOfType<StyleManager>();
            if (weaponHitbox != null)
            {
                weaponHitbox.ConfigureOwner(gameObject);
            }
        }

        private void Update()
        {
            HandleLockOn();
            HandleAttackInput();
            UpdateComboTimer();
            UpdateFacing();
        }

        private void HandleAttackInput()
        {
            bool isAir = controller.StateMachine.CurrentState == PlayerState.Jumping ||
                         controller.StateMachine.CurrentState == PlayerState.Falling ||
                         controller.StateMachine.CurrentState == PlayerState.AirAttacking;

            if (Input.GetButtonDown("Fire1"))
            {
                TriggerAttack(lightAttack, isAir, "light");
            }

            if (Input.GetButtonDown("Fire2"))
            {
                DamageData selected = isAir ? slamAttack : heavyAttack;
                TriggerAttack(selected, isAir, "heavy");
            }

            if (!isAir && Input.GetButton("Vertical") && Input.GetAxis("Vertical") > 0.5f && Input.GetButtonDown("Fire2"))
            {
                TriggerAttack(launcherAttack, false, "launcher");
            }
        }

        private void TriggerAttack(DamageData data, bool isAir, string attackTag)
        {
            if (data == null || weaponHitbox == null)
            {
                return;
            }

            comboStep++;
            comboTimer = comboResetTime;

            if (controller.StateMachine.CurrentState != PlayerState.Dashing)
            {
                controller.StateMachine.ChangeState(isAir ? PlayerState.AirAttacking : PlayerState.Attacking);
            }

            weaponHitbox.Activate(data);
            styleManager.RegisterAttack(attackTag, data.StyleValue, data.IsAirAttack || isAir, comboStep);

            // freeze frame corto de impacto (simulado por timescale)
            if (comboStep % 3 == 0)
            {
                Time.timeScale = 0.9f;
                Time.timeScale = 1f;
            }
        }

        public void EndAttackWindow()
        {
            if (weaponHitbox != null)
            {
                weaponHitbox.Deactivate();
            }

            if (controller.StateMachine.CurrentState == PlayerState.Attacking)
            {
                controller.StateMachine.ChangeState(PlayerState.Grounded);
            }
            if (controller.StateMachine.CurrentState == PlayerState.AirAttacking)
            {
                controller.StateMachine.ChangeState(PlayerState.Falling);
            }
        }

        private void UpdateComboTimer()
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0f)
            {
                comboStep = 0;
            }
        }

        private void HandleLockOn()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                lockOnEnabled = !lockOnEnabled;
                if (lockOnEnabled)
                {
                    AcquireTarget();
                }
            }

            if (lockOnEnabled && (currentTarget == null || !currentTarget.gameObject.activeInHierarchy))
            {
                AcquireTarget();
            }
        }

        private void AcquireTarget()
        {
            Collider[] found = Physics.OverlapSphere(transform.position, lockOnRadius, enemyMask);
            float bestDist = float.MaxValue;
            Transform best = null;

            int i;
            for (i = 0; i < found.Length; i++)
            {
                float d = Vector3.Distance(transform.position, found[i].transform.position);
                if (d < bestDist)
                {
                    best = found[i].transform;
                    bestDist = d;
                }
            }

            currentTarget = best;
        }

        private void UpdateFacing()
        {
            if (!lockOnEnabled || currentTarget == null)
            {
                return;
            }

            Vector3 dir = currentTarget.position - transform.position;
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.01f)
            {
                Quaternion look = Quaternion.LookRotation(dir.normalized);
                transform.rotation = Quaternion.Slerp(transform.rotation, look, Time.deltaTime * 18f);
            }
        }

        public Transform CurrentTarget
        {
            get { return currentTarget; }
        }

        public bool LockOnEnabled
        {
            get { return lockOnEnabled; }
        }
    }
}
