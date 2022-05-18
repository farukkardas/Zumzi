using System;
using System.Collections;
using Enemy;
using InGame.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    public class FightButtons : MonoBehaviour
    {
        [SerializeField] private AudioClip kickSound;
        [SerializeField] private AudioClip powerUpSound;
        [SerializeField] private Button kickButton;
        [SerializeField] private Button powerUpButton;
        [SerializeField] private Button punchButton;


        public static FightButtons Instance;
        [HideInInspector] public bool isPunchable;
        [HideInInspector] public bool isKickable;

        private Animator _animator;
        private AudioSource _audioSource;
        private CharacterController _characterController;
        private int _attackTrigger;
        private int _kickTrigger;
        private int _powerUpTrigger;
        private float poweredPunch = 15f;
        private float poweredKick = 30f;


        #region BuiltIn Methods

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
            _audioSource = GetComponent<AudioSource>();
            isPunchable = true;
            isKickable = true;
            _attackTrigger = Animator.StringToHash("AttackTrigger");
            _kickTrigger = Animator.StringToHash("KickTrigger");
            _powerUpTrigger = Animator.StringToHash("PowerupTrigger");
            _animator = PlayerManager.Instance.animator;
            _characterController = PlayerManager.Instance.GetComponent<CharacterController>();
        }

        #endregion

        #region Timer Methods

        IEnumerator IsPunchableCoroutine(float delay)
        {
            isPunchable = false;
            yield return new WaitForSeconds(delay);
            isPunchable = true;
        }


        IEnumerator IsKickable(float delay)
        {
            isKickable = false;
            yield return new WaitForSeconds(delay);
            isKickable = true;
        }

        IEnumerator DelayedSoundClip(AudioClip _clip, float _delay)
        {
            yield return new WaitForSeconds(_delay);
            _audioSource.PlayOneShot(_clip);
        }

        IEnumerator ButtonDelay(Button button, float delay)
        {
            button.interactable = false;

            yield return new WaitForSeconds(delay);

            button.interactable = true;
        }

        IEnumerator DelayIcon(Button button, float start, float end, float delay)
        {
            var buttonImage = button.transform.Find("Delay");
            Image buttonIcon = buttonImage.GetComponent<Image>();
            buttonIcon.fillAmount = start;

            float elapsed = 0.0f;
            while (elapsed < delay)
            {
                buttonIcon.fillAmount = Mathf.Lerp(start, end, elapsed / delay);
                elapsed += Time.deltaTime;
                yield return null;
            }

            buttonIcon.fillAmount = 0;
        }

        IEnumerator ConstraintPosition()
        {
            _characterController.enabled = false;
            yield return new WaitForSeconds(1.5f);
            _characterController.enabled = true;
        }

        IEnumerator GetPowerUp()
        {
            PlayerManager.Instance.punchDamage = poweredPunch;
            PlayerManager.Instance.kickDamage = poweredKick;
            punchButton.interactable = false;
            kickButton.interactable = false;
            yield return new WaitForSeconds(1.65f);
            punchButton.interactable = true;
            kickButton.interactable = true;
            yield return new WaitForSeconds(10f);
            PlayerManager.Instance.punchDamage = 5f;
            PlayerManager.Instance.kickDamage = 15f;
        }

        #endregion


        public void PunchButton()
        {
            if (isPunchable)
            {
                _animator.SetTrigger(_attackTrigger);
                PlayerPunchHandler.Instance.DoAttack(PlayerManager.Instance.punchDamage);
                StartCoroutine(IsPunchableCoroutine(0.3f));
            }
        }


        public void KickButton()
        {
            if (isKickable)
            {
                _animator.SetTrigger(_kickTrigger);
                StartCoroutine(DelayedSoundClip(kickSound, 0.2f));
                StartCoroutine(ButtonDelay(kickButton, 2f));
                StartCoroutine(DelayIcon(kickButton, 1f, 0f, 2f));
                PlayerKickHandler.Instance.DoAttack(PlayerManager.Instance.kickDamage);
                StartCoroutine(IsKickable(2f));
            }
        }

        public void PowerUpButton()
        {
            _animator.SetTrigger(_powerUpTrigger);
            StartCoroutine(ConstraintPosition());
            StartCoroutine(GetPowerUp());
            StartCoroutine(DelayedSoundClip(powerUpSound, 0.2f));
            StartCoroutine(ButtonDelay(powerUpButton, 15f));
            StartCoroutine(DelayIcon(powerUpButton, 1f, 0f, 15f));
        }
    }
}