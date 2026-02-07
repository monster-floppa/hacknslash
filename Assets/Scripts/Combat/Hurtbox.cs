using UnityEngine;

namespace HacknSlash.Combat
{
    public class Hurtbox : MonoBehaviour
    {
        public delegate void DamageReceivedHandler(DamageData data, GameObject source);
        public event DamageReceivedHandler OnDamageReceived;

        [SerializeField] private int maxHealth = 100;
        [SerializeField] private bool invulnerable;

        public int CurrentHealth { get; private set; }
        public bool IsAlive { get { return CurrentHealth > 0; } }

        private void Awake()
        {
            CurrentHealth = maxHealth;
        }

        public void ReceiveDamage(DamageData data, GameObject source)
        {
            if (invulnerable || !IsAlive || data == null)
            {
                return;
            }

            CurrentHealth -= Mathf.Max(0, data.Amount);
            if (CurrentHealth < 0)
            {
                CurrentHealth = 0;
            }

            if (OnDamageReceived != null)
            {
                OnDamageReceived(data, source);
            }
        }

        public void HealToFull()
        {
            CurrentHealth = maxHealth;
        }

        public void SetInvulnerable(bool value)
        {
            invulnerable = value;
        }
    }
}
