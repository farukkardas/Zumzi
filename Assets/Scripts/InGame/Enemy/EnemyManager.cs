using System;
using System.Collections;
using Enemy;
using Player;
using UnityEngine;
using UnityEngine.AI;

namespace InGame.Enemy
{
    public class EnemyManager : MonoBehaviour
    {
        [SerializeField] private float enemyDamage = 1f;
        [SerializeField] private NavMeshAgent navMeshAgent;
        private Animator _animator;
        private Vector3 _playerTransform;
        private SkinnedMeshRenderer _renderer;
        private Health _healthInstance;
        private EnemyPunchHandler _punchHandler;
        private float _velocity;
        private float _health;
        private int _velocityHash;
        private int _healthHash;
        private int _punchHash;
        private bool canAttack = true;
        public bool canFollow = true;

        private void Start()
        {
            ComponentSetter();
            AnimatorHashSetter();
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
                canFollow = false;
            }
        }



        private void ComponentSetter()
        {
            _animator = GetComponent<Animator>();
            _renderer = GetComponentInChildren<SkinnedMeshRenderer>();
            _healthInstance = GetComponent<Health>();
            _punchHandler = GetComponentInChildren<EnemyPunchHandler>();
        }

        private void AnimatorHashSetter()
        {
            _velocityHash = Animator.StringToHash("Velocity");
            _healthHash = Animator.StringToHash("Health");
            _punchHash = Animator.StringToHash("EnemyPunch");
        }

        void FixedUpdate()
        {
            GetCurrentHealth();
            AnimatorSetters();
            EnemyFollowCondition();

        }

        private void GetCurrentHealth()
        {
            _health = GetComponent<Health>().GetCurrentHealth();
        }

        private void AnimatorSetters()
        {
            _velocity = navMeshAgent.velocity.magnitude;
            _animator.SetFloat(_velocityHash, _velocity);
            _animator.SetFloat(_healthHash, _health);
        }

        private void FollowPlayer()
        {
            if (canFollow)
            {
                if (_health <= 0)
                {
                    canFollow = false;
                }

                _playerTransform = PlayerManager.Instance.transform.position;
                navMeshAgent.destination = _playerTransform;

                var q = Quaternion.LookRotation(_playerTransform - transform.position);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, q, 360 * Time.deltaTime);

                if (navMeshAgent.isStopped)
                {
                    navMeshAgent.ResetPath();
                }

            }
        }

        private void EnemyFollowCondition()
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 0.5f);

            foreach (var collider in hitColliders)
            {
                if (collider.CompareTag("Player"))
                {
                    EnemyAttack();
                    FollowPlayer();
                }
            }
        }

        private void EnemyAttack()
        {
            float dist = Vector3.Distance(PlayerManager.Instance.transform.position, transform.position);


            if (dist < 0.06)
            {
                if (canAttack)
                {
                    _animator.SetTrigger(_punchHash);
                    _punchHandler.DoAttack(enemyDamage);
                    StartCoroutine(AttackDelay());
                }
            }
        }

        IEnumerator AttackDelay()
        {
            canAttack = false;
            yield return new WaitForSeconds(1f);
            canAttack = true;
        }
    }
}