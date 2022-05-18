using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Enemy
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private float maxHealth;
        [SerializeField] private float currentHealth;
        public event Action<float> OnHealthChanged = delegate { };

        private void Awake()
        {
            currentHealth = maxHealth;
        }

        public void ModifyHealth(float amount)
        {
            currentHealth -= amount;

            float currentHealthPct = currentHealth / maxHealth;
            OnHealthChanged(currentHealthPct);
            
        }

        public void AdjustPlayerHealth(float amount)
        {
            currentHealth += amount;
            if (currentHealth >= 100)
            {
                currentHealth = 100;
            }
            float currentHealthPct = currentHealth / maxHealth;
            OnHealthChanged(currentHealthPct);
        }

        private void Update()
        {
            GetCurrentHealth();
        }

        public float GetCurrentHealth()
        {
            return currentHealth;
        }
        
        public void SetCurrentHealth(float health)
        {
            currentHealth = health; 
        }
        public void SetMaxHealth(float enemyMaxHealth)
        {
            maxHealth = enemyMaxHealth;
        }
    }
}