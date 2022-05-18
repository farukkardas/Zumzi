using System;
using System.Collections;
using Player;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    public class EnemyDie : MonoBehaviour
    {
        private float fadeSpeed = 1f;
        private Collider _collider;
        public static event Action OnDie;

        private void Start()
        {
            _collider = GetComponent<Collider>();
        }
        private void OnEnable()
        {
            GetComponent<Health>().OnHealthChanged += CheckEnemyIsDead;
        }

        private void OnDisable()
        {
            GetComponent<Health>().OnHealthChanged -= CheckEnemyIsDead;
        }



        private void CheckEnemyIsDead(float health)
        {
            if (health <= 0)
            {
                //This will trigger score & playerManager script
                OnDie?.Invoke();
                // Do enemy death animation and other things
                StartCoroutine(EnemyDieCondition());
            }
        }

        
        IEnumerator EnemyDieCondition()
        {
            NavMeshAgent navMeshAgent = GetComponent<NavMeshAgent>();
            navMeshAgent.isStopped = true;
            _collider.enabled = false;
            yield return new WaitForSeconds(2);
           
            Color color = GetComponentInChildren<SkinnedMeshRenderer>().material.color;
            //decrease color.a in fadeSpeed
            while (color.a > 0)
            {
                color.a -= fadeSpeed * Time.deltaTime;
                GetComponentInChildren<SkinnedMeshRenderer>().material.color = color;
                yield return null;
            }

            
            yield return new WaitForSeconds(0.1f);
            Destroy(gameObject);
        }


    
    }
}