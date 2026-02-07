using UnityEngine;
using UnityEngine.UI;
using HacknSlash.Combat;
using HacknSlash.Systems;

namespace HacknSlash.UI
{
    public class CombatHUD : MonoBehaviour
    {
        [SerializeField] private Hurtbox playerHurtbox;
        [SerializeField] private StyleManager styleManager;
        [SerializeField] private Text healthText;
        [SerializeField] private Text styleText;
        [SerializeField] private Text comboText;

        private void Update()
        {
            if (playerHurtbox != null && healthText != null)
            {
                healthText.text = "HP " + playerHurtbox.CurrentHealth;
            }

            if (styleManager != null)
            {
                if (styleText != null)
                {
                    styleText.text = "STYLE " + styleManager.CurrentRank + " (" + styleManager.CurrentStyle + ")";
                }

                if (comboText != null)
                {
                    comboText.text = "COMBO x" + styleManager.CurrentComboCount;
                }
            }
        }
    }
}
