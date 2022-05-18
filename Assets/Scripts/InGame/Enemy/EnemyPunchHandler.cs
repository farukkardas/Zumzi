using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    public class EnemyPunchHandler : MonoBehaviour
    {
        [SerializeField] private BoxCollider collider;
        [SerializeField] private BoxCollider collider2;


        public void DoAttack(float damage)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 0.04f);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Player"))
                {
                    Health enemyHealth = hitCollider.gameObject.GetComponent<Health>();
                    
                    if (enemyHealth.enabled == false)
                    {
                        return;
                    }

                    enemyHealth.ModifyHealth(damage);
                }


                if (hitCollider.CompareTag("Enemy"))
                {
                    Physics.IgnoreCollision(collider, collider2);
                }
            }
        }
    }
}