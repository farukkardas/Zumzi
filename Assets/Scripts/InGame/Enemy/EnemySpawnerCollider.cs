using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawnerCollider : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int enemyCount;
    private bool _isSpawnable = true;

   

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_isSpawnable)
                for (int i = 0; i < enemyCount; i++)
                {
                    float randomPosX = Random.Range(-0.3f, 0.6f);
                    float randomPosZ = Random.Range(-0.3f, 0.6f);
                    Vector3 transformPos = transform.position;
                    Vector3 spawnPos = new Vector3(transformPos.x + randomPosX, transformPos.y, transformPos.z + randomPosZ);
                    var enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

                    Vector3 forward = enemy.gameObject.transform.TransformDirection(Vector3.down) * 0.1f;
                    Ray ray = new Ray(enemy.gameObject.transform.position, forward);
                    if (!Physics.Raycast(ray, out RaycastHit hit, 0.1f))
                    {
                        DestroyImmediate(enemy);
                    }
                }

            _isSpawnable = false;
        }
    }
}