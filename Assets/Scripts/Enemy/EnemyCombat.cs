using UnityEngine;
using HacknSlash.Combat;

namespace HacknSlash.Enemy
{
    public class EnemyCombat : MonoBehaviour
    {
        [SerializeField] private Hitbox hitbox;
        [SerializeField] private DamageData meleeDamage;
        [SerializeField] private float attackCooldown = 1.2f;

        private float cooldown;

        private void Awake()
        {
            if (hitbox != null)
            {
                hitbox.ConfigureOwner(gameObject);
                hitbox.Deactivate();
            }
        }

        private void Update()
        {
            if (cooldown > 0f)
            {
                cooldown -= Time.deltaTime;
            }
        }

        public bool CanAttack()
        {
            return cooldown <= 0f;
        }

        public void PerformAttack()
        {
            if (hitbox == null || meleeDamage == null || !CanAttack())
            {
                return;
            }

            cooldown = attackCooldown;
            hitbox.Activate(meleeDamage);
        }

        public void EndAttack()
        {
            if (hitbox != null)
            {
                hitbox.Deactivate();
            }
        }
    }
}
