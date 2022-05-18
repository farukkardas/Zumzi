using System.Collections;
using System.Collections.Generic;
using Enemy;
using Player;
using UnityEngine;
using Random = UnityEngine.Random;

namespace InGame.Player
{
    public class PlayerPunchHandler : MonoBehaviour
    {
        public static PlayerPunchHandler Instance;

        [SerializeField] private AudioSource audioSource;
        [SerializeField] private List<AudioClip> punchSounds;
        private int _takeDamageHash;
        private static readonly int TakeDamage = Animator.StringToHash("TakeDamage");
        private static readonly int TowerDamage = Animator.StringToHash("TowerDamage");

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


        IEnumerator EnemyDamageChangeMesh(SkinnedMeshRenderer skinnedMeshRenderer, Animator animator)
        {
            Material skinnedMeshMaterial = skinnedMeshRenderer.material;
            _takeDamageHash = Animator.StringToHash("TakeDamage");
            animator.SetTrigger(TakeDamage);
            skinnedMeshMaterial.color = Color.red;
            yield return new WaitForSeconds(0.2f);
            skinnedMeshMaterial.color = Color.black;
        }

        IEnumerator SpawnerDamageChangeMesh(MeshRenderer meshRenderer, Animator animator)
        {
            Material meshMaterial = meshRenderer.material;
            Color defaultColor = meshMaterial.color;
            meshMaterial.color = Color.blue;
            yield return new WaitForSeconds(0.2f);
            meshMaterial.color = defaultColor;
        }

        public void DoAttack(float damage)
        {
            // int maxColliders = 10;
            // Collider[] hitColliders = new Collider[maxColliders];
            // var size = Physics.OverlapSphereNonAlloc(transform.position, 0.03f, hitColliders);
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 0.03f);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Player"))
                {
                    return;
                }


                if (hitCollider.CompareTag("Enemy"))
                {
                    if (FightButtons.Instance.isPunchable)
                    {
                        Health enemyHealth = hitCollider.gameObject.GetComponent<Health>();
                        SkinnedMeshRenderer skinnedMeshRenderer =
                            hitCollider.GetComponentInChildren<SkinnedMeshRenderer>();
                        Animator animator = hitCollider.GetComponent<Animator>();


                        enemyHealth.ModifyHealth(damage);
                        StartCoroutine(EnemyDamageChangeMesh(skinnedMeshRenderer, animator));


                        for (int i = 0; i < punchSounds.Count; i++)
                        {
                            AudioClip tempAudioClip = punchSounds[i];
                            int randomIndex = Random.Range(i, punchSounds.Count);
                            punchSounds[i] = punchSounds[randomIndex];
                            punchSounds[randomIndex] = tempAudioClip;
                            audioSource.PlayOneShot(tempAudioClip);
                        }
                    }
                }

                if (hitCollider.CompareTag("EnemySpawner"))
                {
                    if (FightButtons.Instance.isPunchable)
                    {
                        Health enemyHealth = hitCollider.gameObject.GetComponent<Health>();
                        MeshRenderer meshRenderer = hitCollider.GetComponentInChildren<MeshRenderer>();
                        Animator animator = hitCollider.GetComponent<Animator>();

                        //Sound effect
                        for (int i = 0; i < punchSounds.Count; i++)
                        {
                            AudioClip tempAudioClip = punchSounds[i];
                            int randomIndex = Random.Range(i, punchSounds.Count);
                            punchSounds[i] = punchSounds[randomIndex];
                            punchSounds[randomIndex] = tempAudioClip;
                            audioSource.PlayOneShot(tempAudioClip);
                        }

                        // Modify health and change the mesh
                        if (enemyHealth.enabled == false)
                        {
                            return;
                        }

                        enemyHealth.ModifyHealth(damage);
                        StartCoroutine(SpawnerDamageChangeMesh(meshRenderer, animator));


                        //Play animations
                        if (animator == null)
                        {
                            return;
                        }

                        animator.Play("SpawnerTakeDamage");
                        animator.SetTrigger(TowerDamage);
                    }
                }
            }
        }
    }
}