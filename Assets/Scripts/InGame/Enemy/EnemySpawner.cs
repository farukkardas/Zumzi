using System.Collections;
using Enemy;
using UnityEngine;
using Random = UnityEngine.Random;


namespace InGame.Enemy
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private Health health;
        [SerializeField] private Animator animator;
        [SerializeField] private int maxEnemySpawn = 5;
        [SerializeField] private int enemySpawnDelay = 10;
        [SerializeField] private float enemyMaxHealth = 100f;
        private bool _isSpawnable = true;
        int _currentEnemy = 0;
        private static readonly int SpawnerHealth = Animator.StringToHash("SpawnerHealth");

        private void Start()
        {
            SetEnemyMaxHealth();
        }

        private void OnEnable()
        {
            health.OnHealthChanged += SpawnerDeath;
        }

        private void OnDisable()
        {
            health.OnHealthChanged -= SpawnerDeath;

        }

        private void SpawnerDeath(float health)
        {
            if (health <= 0)
            {
                Die();
            }
        }

        private void SetEnemyMaxHealth()
        {
            Health enemyHealth = enemyPrefab.GetComponent<Health>();
            enemyHealth.SetMaxHealth(enemyMaxHealth);
        }

        private void FixedUpdate()
        {
            SpawnEnemy();
            RotateMesh();
        }

        void RotateMesh()
        {
            // get mesh in child
            MeshRenderer mesh = GetComponentInChildren<MeshRenderer>();
            // rotate mesh z
            mesh.transform.RotateAround(transform.position, Vector3.up, Time.fixedDeltaTime * 90f);

            animator.SetFloat(SpawnerHealth, health.GetCurrentHealth());
        }

        void SpawnEnemy()
        {
            float randomPosX = Random.Range(-0.3f, 0.6f);
            float randomPosZ = Random.Range(-0.3f, 0.6f);

            if (_currentEnemy == maxEnemySpawn)
            {
                _isSpawnable = false;
            }

            if (_isSpawnable)
            {
                Vector3 transformPos = transform.position;
                Vector3 spawnPos = new Vector3(transformPos.x + randomPosX, transformPos.y, transformPos.z + randomPosZ);

                var enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);


                // check if spawned enemy is not on ground
                Vector3 forward = enemy.gameObject.transform.TransformDirection(Vector3.down) * 0.1f;
                Ray ray = new Ray(enemy.gameObject.transform.position, forward);
                if (!Physics.Raycast(ray, out RaycastHit hit, 0.1f))
                {
                    DestroyImmediate(enemy);
                }

                else
                {
                    StartCoroutine(SpawnDelay());
                    _currentEnemy += 1;
                }


            }
        }

        private IEnumerator SpawnDelay()
        {
            _isSpawnable = false;
            yield return new WaitForSeconds(enemySpawnDelay);
            _isSpawnable = true;
        }

        private void Die()
        {
            Destroy(gameObject, 1f);
        }
    }
}