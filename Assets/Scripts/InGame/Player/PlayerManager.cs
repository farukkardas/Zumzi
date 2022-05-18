using System;
using System.Collections;
using Enemy;
using UI;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

namespace Player
{
    public class PlayerManager : MonoBehaviour
    {
        public static PlayerManager Instance;
        public static Action PlayerDead;
        //Inspector Components
        [SerializeField] private FixedJoystick joystick;
        [SerializeField] private AudioClip breathSound;
        [SerializeField] private AudioSource audioSource;

        //Private fields
        private float walkSpeed = 0.3f;
        private bool _isAlive = true;

        //public fields
        public CharacterController characterController;
        public Animator animator;
        public Health health;
        public float punchDamage = 5f;
        public float kickDamage = 15f;
        public GameObject EnemyBulletPosition;
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
            SetComponents();
        }

        private void OnEnable()
        {
            EnemyDie.OnDie += RaisePlayerHealth;
            health.OnHealthChanged += CheckPlayerDied;
            UIHandler.PlayerRevived += AfterReviveProtection;
        }

        private void OnDisable()
        {
            EnemyDie.OnDie -= RaisePlayerHealth;
            UIHandler.PlayerRevived -= AfterReviveProtection;
            health.OnHealthChanged -= CheckPlayerDied;
        }

        private void CheckPlayerDied(float health)
        {
            if (health <= 30)
            {   
                PlayBreathSound();
            }

            if (!_isAlive)
                return;
            
            if (health <= 0)
            {
                _isAlive = false;
                animator.SetTrigger("Die");
                StartCoroutine(PlayerDie());
                PlayerDead.Invoke();
            }
       
        }

        void RaisePlayerHealth()
        {
            health.AdjustPlayerHealth(10);
        }

        private void SetComponents()
        {
            GameObject uiDisplay = GameObject.Find("UI_Display");
            Transform canvas = uiDisplay.transform.Find("Canvas");
            Transform transformJoystick = canvas.transform.Find("Fixed Joystick");
            characterController.detectCollisions = false;
            FixedJoystick fixedJoystick = transformJoystick.GetComponent<FixedJoystick>();
            joystick = fixedJoystick;
            SetHealthBarImage();
        }

        void Update()
        {
            MoveCharacter();
        }


        IEnumerator PlayerDie()
        {
            _isAlive = false;
            UIHandler.Instance.DisableButtons();
            Time.timeScale = 0.3f;
            yield return new WaitForSeconds(1f);
        }

        private void PlayBreathSound()
        {
            if (health.GetCurrentHealth() < 30)
            {
                if (_isAlive)
                    audioSource.PlayOneShot(breathSound);
            }
        }


        public void SetHealthBarImage()
        {
            HandleHealthBar healthBarImage = GetComponent<HandleHealthBar>();
            GameObject uiDisplay = GameObject.Find("UI_Display");
            Transform canvas = uiDisplay.transform.Find("Canvas");
            Transform healthMainBar = canvas.transform.Find("Health");
            Transform healthBar = healthMainBar.transform.Find("HealthBar");
            Image barImage = healthBar.GetComponent<Image>();

            healthBarImage.healthBar = barImage;
        }

        private void MoveCharacter()
        {
            float moveHorizontal = joystick.Horizontal;
            float moveVertical = -joystick.Vertical;


            Vector3 movement = new Vector3(moveVertical, 0, moveHorizontal);
            movement *= walkSpeed;


            if (movement != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), 0.3f);

                transform.Translate(movement * characterController.velocity.magnitude * Time.deltaTime, Space.World);
            }


            if (characterController.enabled)
            {
                characterController.Move(movement * Time.deltaTime);
            }
        }

        public void AfterReviveProtection()
        {
            StartCoroutine(ReviveCharacter());
        }

        private IEnumerator ReviveCharacter()
        {
            health.SetCurrentHealth(100);

            SkinnedMeshRenderer skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();

            for (int i = 0; i < 10; i++)
            {
                health.enabled = false;
                skinnedMeshRenderer.enabled = false;
                yield return new WaitForSeconds(0.3f);
                skinnedMeshRenderer.enabled = true;
                yield return new WaitForSeconds(0.3f);
            }

            _isAlive = true;
            health.enabled = true;
        }
    }
}