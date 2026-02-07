using UnityEngine;

namespace HacknSlash.Combat
{
    public enum DamageReaction
    {
        Light,
        Heavy,
        Launch,
        Slam,
        Stun
    }

    [System.Serializable]
    public class DamageData
    {
        public int Amount;
        public float HitStun;
        public float Knockback;
        public Vector3 Direction;
        public DamageReaction Reaction;
        public string AttackId;
        public bool IsAirAttack;
        public int StyleValue;

        public DamageData Clone()
        {
            return (DamageData)MemberwiseClone();
        }
    }
}
