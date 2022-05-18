using System;
using System.Collections;
using System.Collections.Generic;
using Enemy;
using Player;
using UnityEngine;

public class ShooterEnemy : MonoBehaviour
{
    [SerializeField] private GameObject gunPrefab;
    [SerializeField] private Transform rightHand;
    [SerializeField] private GameObject projectile;

    private Animator _animator;
    private Health _healthInstance;
    private SkinnedMeshRenderer _renderer;
    private int _healthHash;
    private float _health;
    private Vector3 _playerTransform;
    private bool _canShoot = true;

    private static readonly int Shoot = Animator.StringToHash("Shoot");

    private void Start()
    {
        ComponentSetter();
        AnimatorHashSetter();
    }

    private void ComponentSetter()
    {
        _animator = GetComponent<Animator>();
        _renderer = GetComponentInChildren<SkinnedMeshRenderer>();
        _healthInstance = GetComponent<Health>();
    }

    private void AnimatorHashSetter()
    {
        _healthHash = Animator.StringToHash("Health");
    }

    private void FixedUpdate()
    {
        GetCurrentHealth(); 
        AnimatorSetters();
        RotateToPlayer();
        StartCoroutine(EnemyShoot());
    }

    private void RotateToPlayer()
    {
        if (_healthInstance.GetCurrentHealth() <= 0)
            return;
        _playerTransform = PlayerManager.Instance.transform.position;
        var q = Quaternion.LookRotation(_playerTransform - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, q, 360 * Time.deltaTime);
    }

    private void GetCurrentHealth()
    {
        _health = GetComponent<Health>().GetCurrentHealth();
    }

    private void AnimatorSetters()
    {
        _animator.SetFloat(_healthHash, _health);

        Transform playerTransform = PlayerManager.Instance.transform;
        //Player position
        Vector3 playerPosition = new Vector3(playerTransform.position.x, playerTransform.transform.position.y + 0.02f, playerTransform.transform.position.z);

    }

    // Need a nice gun asset 
    //private void AttachGunHand()
    //{
    //    //lookrotation
    //    var q = Quaternion.LookRotation(_playerTransform - transform.position);
    //    //rotatetowards
    //    _instantiatedGun.transform.rotation = Quaternion.RotateTowards(_instantiatedGun.transform.rotation, q, 360 * Time.deltaTime);
    //    _instantiatedGun.transform.position = rightHand.position;
    //}


    private IEnumerator EnemyShoot()
    {
        if (_healthInstance.GetCurrentHealth() <= 0)
            _canShoot = false;

        float dist = Vector3.Distance(PlayerManager.Instance.transform.position, transform.position);

        if (dist < 0.7)
        {
            if (_canShoot)
            {
                _canShoot = false;
                _animator.SetTrigger(Shoot);
                yield return new WaitForSeconds(0.5f);
                var miniProjectile = Instantiate(projectile, rightHand.transform.position, rightHand.transform.rotation);
                miniProjectile.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

                var miniProjectileRb = miniProjectile.GetComponent<Rigidbody>();
                miniProjectileRb.AddForce(this.gameObject.transform.forward * 50);

                yield return new WaitForSeconds(1f);
                _canShoot = true;

            }
        }
    }
}