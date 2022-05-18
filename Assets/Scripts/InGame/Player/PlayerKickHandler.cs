using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Enemy;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Player
{
    public class PlayerKickHandler : MonoBehaviour
    {
        public static PlayerKickHandler Instance;

        [SerializeField] private AudioSource audioSource;
        [SerializeField] private List<AudioClip> kickSounds;
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


        IEnumerator ChangeMaterialColorOnDamaged(SkinnedMeshRenderer _renderer, Animator _animator)
        {
            _takeDamageHash = Animator.StringToHash("TakeDamage");
            _animator.SetTrigger(TakeDamage);
            _renderer.material.color = Color.red;
            yield return new WaitForSeconds(0.2f);
            _renderer.material.color = Color.black;
        }

        IEnumerator SpawnerDamageChangeMesh(MeshRenderer _renderer, Animator _animator)
        {
            if (_renderer != null)
            {
                var material = _renderer.material;
                Color defaultColor = material.color;
                material.color = Color.blue;
                yield return new WaitForSeconds(0.2f);
                material.color = defaultColor;
            }
        }

        public void DoAttack(float damage)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 0.07f);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Player"))
                {
                    return;
                }


                if (hitCollider.CompareTag("Enemy"))
                {
                    if (FightButtons.Instance.isKickable)
                    {
                        Health enemyHealth = hitCollider.gameObject.GetComponent<Health>();
                        SkinnedMeshRenderer meshRenderer = hitCollider.GetComponentInChildren<SkinnedMeshRenderer>();
                        Animator animator = hitCollider.GetComponent<Animator>();
                        Transform playerDirection = GetComponentInParent<Transform>();
                        Vector3 enemyPosition = hitCollider.gameObject.transform.position;

                        StartCoroutine(ChangeMaterialColorOnDamaged(meshRenderer, animator));

                        enemyHealth.ModifyHealth(damage);


                        Vector3 pushPosition = new Vector3(enemyPosition.x, 0,
                            enemyPosition.z - playerDirection.transform.position.z * 0.03f);


                        hitCollider.gameObject.transform.position =
                            Vector3.Lerp(hitCollider.transform.position, pushPosition, 1f);


                        for (int i = 0; i < kickSounds.Count; i++)
                        {
                            audioSource.PlayOneShot(kickSounds[i]);
                        }
                    }
                }

                if (hitCollider.CompareTag("EnemySpawner"))
                {
                    if (FightButtons.Instance.isKickable)
                    {
                        Health enemyHealth = hitCollider.gameObject.GetComponent<Health>();
                        MeshRenderer meshRenderer = hitCollider.GetComponentInChildren<MeshRenderer>();
                        Animator animator = hitCollider.GetComponent<Animator>();


                        //Do damage and change mesh
                        if (enemyHealth.enabled == false)
                        {
                            return;
                        }
                        enemyHealth.ModifyHealth(damage);
                        StartCoroutine(SpawnerDamageChangeMesh(meshRenderer, animator));
                        
                        
                        //Sounds
                        foreach (var t in kickSounds)
                        {
                            audioSource.PlayOneShot(t);
                        }
                        
                        //Animations

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