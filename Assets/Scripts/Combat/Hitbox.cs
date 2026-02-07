using System.Collections.Generic;
using UnityEngine;

namespace HacknSlash.Combat
{
    [RequireComponent(typeof(Collider))]
    public class Hitbox : MonoBehaviour
    {
        [SerializeField] private DamageData baseDamage;
        [SerializeField] private bool activeOnStart;

        private Collider cachedCollider;
        private GameObject owner;
        private HashSet<int> hitIds = new HashSet<int>();

        private void Awake()
        {
            cachedCollider = GetComponent<Collider>();
            if (cachedCollider != null)
            {
                cachedCollider.isTrigger = true;
                cachedCollider.enabled = activeOnStart;
            }
        }

        public void ConfigureOwner(GameObject source)
        {
            owner = source;
        }

        public void Activate(DamageData overrideData)
        {
            hitIds.Clear();
            if (overrideData != null)
            {
                baseDamage = overrideData.Clone();
            }

            if (cachedCollider != null)
            {
                cachedCollider.enabled = true;
            }
        }

        public void Deactivate()
        {
            hitIds.Clear();
            if (cachedCollider != null)
            {
                cachedCollider.enabled = false;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            Hurtbox hurtbox = other.GetComponent<Hurtbox>();
            if (hurtbox == null || baseDamage == null)
            {
                return;
            }

            int id = other.gameObject.GetInstanceID();
            if (hitIds.Contains(id))
            {
                return;
            }

            hitIds.Add(id);

            DamageData payload = baseDamage.Clone();
            payload.Direction = transform.forward;
            hurtbox.ReceiveDamage(payload, owner != null ? owner : gameObject);
        }
    }
}
