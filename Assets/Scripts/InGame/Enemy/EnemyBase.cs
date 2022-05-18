using System;
using System.Collections;
using System.Collections.Generic;
using Enemy;
using GoogleMobileAds.Api;
using UI;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [SerializeField] private GameObject tower1;
    [SerializeField] private GameObject tower2;
    [SerializeField] private Health healthScript;
    [SerializeField] private HandleHealthBar healthBar;
    [SerializeField] private ParticleSystem destroyParticle;
    [SerializeField] private int currentLevel;
    bool _adsOpen = true;

    private void OnEnable()
    {
        healthScript.OnHealthChanged += HandleHealthChanged;
    }

    private void OnDisable()
    {
        healthScript.OnHealthChanged -= HandleHealthChanged;
    }


    private void HandleHealthChanged(float health)
    {
        if (health <= 0)
        {
            Die();
        }
    }


    void Start()
    {
        healthScript.enabled = false;
        healthBar.enabled = false;
    }

    void Update()
    {
        BaseCanAttackable();
    }

    void BaseCanAttackable()
    {
        if (tower1 == null && tower2 == null)
        {
            healthScript.enabled = true;
            healthBar.enabled = true;
        }
    }

    void Die()
    {
        if (healthScript.GetCurrentHealth() < 0.01)
        {
            int maxLevel = PlayerPrefs.GetInt("MaxLevel");

            if (maxLevel <= currentLevel)
            {
                SetInt("MaxLevel", currentLevel + 1);
            }


            if (_adsOpen)
            {
                UIHandler.Instance.OpenWinMenu();
                _adsOpen = false;
            }

            Time.timeScale = 0.3f;
            destroyParticle.Play();
            Destroy(gameObject, 1.2f);
        }
    }

    public void SetInt(string KeyName, int Value)
    {
        PlayerPrefs.SetInt(KeyName, Value);
    }

    public void GetInt(string KeyName, int Value)
    {
        PlayerPrefs.SetInt(KeyName, Value);
    }
}