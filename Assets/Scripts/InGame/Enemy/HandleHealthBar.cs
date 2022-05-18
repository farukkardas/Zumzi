using System;
using System.Collections;
using Player;
using UnityEngine;
using UnityEngine.UI;

namespace Enemy
{
    public class HandleHealthBar : MonoBehaviour
    {
        public Image healthBar;
        [SerializeField] private float updateInSeconds = 0.5f;

        private void Awake()
        {
            GetComponentInParent<Health>().OnHealthChanged += HandleHealthChanged;
        }

        private void HandleHealthChanged(float pct)
        {
            StartCoroutine(ChangeToPct(pct));
        }
        

        private IEnumerator ChangeToPct(float pct)
        {
            float preChangePct = healthBar.fillAmount;
            float elapsed = 0f;

            while (elapsed < updateInSeconds)
            {
                elapsed += Time.deltaTime;
                healthBar.fillAmount = Mathf.Lerp(preChangePct, pct, elapsed / updateInSeconds);
                yield return null;
            }

            healthBar.fillAmount = pct;
        }
    }
}