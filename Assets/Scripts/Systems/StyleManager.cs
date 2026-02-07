using System.Collections.Generic;
using UnityEngine;

namespace HacknSlash.Systems
{
    public class StyleManager : MonoBehaviour
    {
        private static readonly string[] RankLabels = { "D", "C", "B", "A", "S", "SS", "SSS" };

        [SerializeField] private int[] rankThresholds = { 0, 100, 220, 380, 600, 850, 1200 };
        [SerializeField] private float passiveDecayPerSecond = 30f;
        [SerializeField] private int repeatPenalty = 15;
        [SerializeField] private int damageTakenPenalty = 80;

        private Dictionary<string, int> attackRepeatCounter = new Dictionary<string, int>();

        public int CurrentStyle { get; private set; }
        public int CurrentComboCount { get; private set; }

        public string CurrentRank
        {
            get
            {
                int i;
                for (i = rankThresholds.Length - 1; i >= 0; i--)
                {
                    if (CurrentStyle >= rankThresholds[i])
                    {
                        return RankLabels[i];
                    }
                }

                return RankLabels[0];
            }
        }

        private void Update()
        {
            CurrentStyle -= Mathf.RoundToInt(passiveDecayPerSecond * Time.deltaTime);
            if (CurrentStyle < 0)
            {
                CurrentStyle = 0;
            }
        }

        public void RegisterAttack(string attackId, int baseStyle, bool airAttack, int comboCount)
        {
            if (string.IsNullOrEmpty(attackId))
            {
                attackId = "unknown";
            }

            CurrentComboCount = comboCount;

            int gain = baseStyle;
            if (airAttack)
            {
                gain += 12;
            }
            gain += Mathf.Min(comboCount * 2, 40);

            int repeatedCount = 0;
            if (!attackRepeatCounter.TryGetValue(attackId, out repeatedCount))
            {
                attackRepeatCounter[attackId] = 0;
            }
            attackRepeatCounter[attackId] = attackRepeatCounter[attackId] + 1;

            if (attackRepeatCounter[attackId] > 2)
            {
                gain -= repeatPenalty * (attackRepeatCounter[attackId] - 2);
            }

            CurrentStyle += Mathf.Max(0, gain);
        }

        public void RegisterDamageTaken()
        {
            CurrentStyle -= damageTakenPenalty;
            if (CurrentStyle < 0)
            {
                CurrentStyle = 0;
            }

            CurrentComboCount = 0;
            attackRepeatCounter.Clear();
        }

        public float GetScoreMultiplier()
        {
            if (CurrentStyle < rankThresholds[1]) return 1.0f;
            if (CurrentStyle < rankThresholds[2]) return 1.2f;
            if (CurrentStyle < rankThresholds[3]) return 1.5f;
            if (CurrentStyle < rankThresholds[4]) return 2.0f;
            if (CurrentStyle < rankThresholds[5]) return 2.6f;
            if (CurrentStyle < rankThresholds[6]) return 3.3f;
            return 4.0f;
        }
    }
}
