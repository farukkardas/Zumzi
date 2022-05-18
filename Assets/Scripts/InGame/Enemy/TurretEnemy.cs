using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography;
using Enemy;
using Player;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class TurretEnemy : MonoBehaviour
{
    [SerializeField] private GameObject projectile;
    [SerializeField] private GameObject spawnPoint;
    [SerializeField] private float shootDelay = 50f;
    [SerializeField] private float shootBetweenSeconds = 3f;
    [SerializeField] private float turretRange = 1f;
    private GameObject _tempBullet;
    private bool _canShoot = true;

    private void OnEnable()
    {
        //getcomponent health and subscribe
        GetComponent<Health>().OnHealthChanged += OnHealthChanged;
    }

    private void OnDisable()
    {
        GetComponent<Health>().OnHealthChanged -= OnHealthChanged;

    }

    private void OnHealthChanged(float health)
    {
        if (health <= 0)
        {
            Destroy(gameObject, 0.1f);
        }
    }


    private void Update()
    {
        CalculateEnemyLocationAndShoot();
    }

    private void CalculateEnemyLocationAndShoot()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, turretRange);

        foreach (var collider in hitColliders)
        {
            if (collider.CompareTag("Player"))
            {
                if (_canShoot)
                {
                    GameObject instantiatedProjectile = Instantiate(projectile, spawnPoint.transform.position,
                        spawnPoint.transform.rotation);

                    //Player rotation
                    Transform playerTransform = PlayerManager.Instance.transform;
                    //Player position
                    Vector3 playerPosition = new Vector3(playerTransform.position.x, playerTransform.transform.position.y + 0.02f, playerTransform.transform.position.z);

                    //Shoot Projectile to player
                    StartCoroutine(ShootProjectile(instantiatedProjectile, instantiatedProjectile.transform.position,
                        playerPosition, shootDelay));

                    //Delay after seconds
                    StartCoroutine(ProjectileDelay());
                }
            }
        }
    }


    private IEnumerator ShootProjectile(GameObject instantiatedProjectile, Vector3 pos1, Vector3 pos2, float duration)
    {
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            if (instantiatedProjectile != null)
            {
                instantiatedProjectile.transform.position = Vector3.Lerp(instantiatedProjectile.transform.position,
                    pos2, t / duration);
                yield return 0;
            }
        }
    }

    IEnumerator ProjectileDelay()
    {
        _canShoot = false;
        yield return new WaitForSeconds(shootBetweenSeconds);
        _canShoot = true;
    }

}