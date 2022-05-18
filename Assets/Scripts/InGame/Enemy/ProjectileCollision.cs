using System;
using System.Collections;
using System.Collections.Generic;
using Enemy;
using UnityEngine;

public class ProjectileCollision : MonoBehaviour
{
    [SerializeField] private float projectileDamage = 5f;

    private void Update()
    {
        StartCoroutine(DeleteBulletEveryTwoSecond());
    }

    private IEnumerator DeleteBulletEveryTwoSecond()
    {
        // delete this gameobject every 5 second
        yield return new WaitForSeconds(5f);
        Destroy(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Health health = other.GetComponent<Health>();

            if (health.enabled == false)
            {
                return;
            }
            health.ModifyHealth(projectileDamage);
            Destroy(gameObject);
        }
    }
}
