using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Enemy;
using Player;
using UnityEngine;

namespace InGame.World
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance;
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private List<GameObject> buildings;
        [SerializeField] private List<GameObject> destroyableBuildings;
        [SerializeField] private GameObject blackHoleSpawnPoint;
        [SerializeField] private GameObject leftTower;
        [SerializeField] private GameObject rightTower;
        [SerializeField] private GameObject enemyBase;

        private int _destroyableCount;
        private int _buildingCount;
        private int _idleStart;
        private bool _isDone;


        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
        }


        private void Start()
        {
            StartCoroutine(StartAnimation());
        }

        private void FixedUpdate()
        {
            SetFirstPlayerPosition();
            GetDestroyableBuildCount();
            GetBuildingCount();
            EnableBaseHealth();
            EnableTowerHealth();
        }

        public int GetBuildingCount()
        {
            for (int i = 0; i < buildings.Count; i++)
            {
                if (buildings[i] == null)
                {
                    buildings.Remove(buildings[i]);
                }

            }

            _buildingCount = buildings.Count;
            return _buildingCount;
        }

        private void GetDestroyableBuildCount()
        {
            for (int i = 0; i < destroyableBuildings.Count; i++)
            {
                if(destroyableBuildings[i] == null)
                {
                    destroyableBuildings.Remove(destroyableBuildings[i]);
                }
            }

            _destroyableCount = destroyableBuildings.Count;
        }

        private void EnableTowerHealth()
        {
            if (_destroyableCount != 0)
            {
                return;
            }

            if (leftTower == null || rightTower == null)
            {
                return;
            }

            leftTower.GetComponent<Health>().enabled = true;
            rightTower.GetComponent<Health>().enabled = true;
        }
        
  
        private void SetFirstPlayerPosition()
        {
            if (_isDone)
            {
                var playerTransform = PlayerManager.Instance.transform;
                var playerPosition = playerTransform.position;
                playerPosition = new Vector3(playerPosition.x, 0.0f,
                    playerPosition.z);
                playerTransform.position = playerPosition;
                _isDone = false;
            }
        }


        private void EnableBaseHealth()
        {
            if (leftTower == null && rightTower == null)
            {
                if (enemyBase == null)
                {
                    return;
                }

                Health baseHealth = enemyBase.GetComponent<Health>();
                baseHealth.enabled = true;
            }
        }

        private IEnumerator StartAnimation()
        {
            _idleStart = Animator.StringToHash("Idle");

            Vector3 spawnPosition = blackHoleSpawnPoint.transform.position;

            GameObject playerInstantiate =
                Instantiate(playerPrefab, new Vector3(spawnPosition.x, spawnPosition.y - 0.03f, spawnPosition.z),
                    Quaternion.identity);
            playerInstantiate.transform.Rotate(new Vector3(0, -90, 0));
            PlayerManager.Instance.characterController.enabled = false;
            PlayerManager.Instance.animator.Play("BlackHoleStart");
            yield return new WaitForSeconds(1.5f);
            PlayerManager.Instance.animator.SetTrigger(_idleStart);
            yield return new WaitForSeconds(0.5f);
            PlayerManager.Instance.characterController.enabled = true;
            _isDone = true; 
        }
    }
}