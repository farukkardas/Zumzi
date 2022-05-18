using System;
using System.Collections;
using System.Globalization;
using System.Runtime.CompilerServices;
using Enemy;
using GoogleMobileAds.Api;
using InGame.World;
using Player;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    public class UIHandler : MonoBehaviour
    {
        public static UIHandler Instance;
        public static Action PlayerRevived;
        [SerializeField] private GameObject pauseMenu;
        [SerializeField] private GameObject dieMenu;
        [SerializeField] private GameObject winMenu;
        [SerializeField] private GameObject[] uiButtons;
        [SerializeField] private GameObject joystickController;
        [SerializeField] private AudioClip openMenuSound;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private TMP_Text buildingCount;
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private Image healthBar;
        [SerializeField] private GameObject noConnectionPanel;
        private bool _toggle = true;
        private int _playerScore = 0;

        private RewardedAd _rewardedAd;
        private InterstitialAd _interstitialAd;

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
            Time.timeScale = 1;
            scoreText.text = "Score: 0";
            //AdMob implementations

            MobileAds.Initialize(status => { });

            RewardsAdSubscriptions();
            RequestInterstitial();
        }


        private void OnEnable()
        {
            EnemyDie.OnDie += AddScore;
            CheckInternetConnection.OnConnectionChanged += ControlNoConnectionPanel;
            PlayerManager.PlayerDead += CheckPlayerStatue;
        }

        private void OnDisable()
        {
            EnemyDie.OnDie -= AddScore;
            CheckInternetConnection.OnConnectionChanged -= ControlNoConnectionPanel;
            PlayerManager.PlayerDead -= CheckPlayerStatue;

        }

        internal void CloseDieMenu()
        {
            dieMenu.SetActive(false);
        }

        private void ControlNoConnectionPanel(bool condition)
        {
            if (condition)
            {
                noConnectionPanel.SetActive(false);
                joystickController.SetActive(true);
                foreach (var eButton in uiButtons)
                {
                    eButton.SetActive(true);
                }
            }
            else
            {
                joystickController.SetActive(false);
                noConnectionPanel.SetActive(true);
                foreach (var eButton in uiButtons)
                {
                    eButton.SetActive(false);
                }
            }

        }

        private void Update()
        {
            SetBuildingCount();
        }

        void AddScore()
        {
            _playerScore += 5;
            scoreText.text = $"Score: {_playerScore}";
        }

        #region RewardAds

        private void RewardsAdSubscriptions()
        {
            string adUnitId;
#if UNITY_ANDROID
            adUnitId = "ca-app-pub-3940256099942544/5224354917";
#elif UNITY_IPHONE
            adUnitId = "ca-app-pub-3940256099942544/1712485313";
#else
            adUnitId = "unexpected_platform";
#endif

            _rewardedAd = new RewardedAd(adUnitId);


            _rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
            _rewardedAd.OnAdClosed += HandleRewardedAdClosed;

            AdRequest request = new AdRequest.Builder().Build();
            this._rewardedAd.LoadAd(request);
        }

        private void HandleRewardedAdClosed(object sender, EventArgs args)
        {
            // AdClosed event received.
        }

        private void HandleUserEarnedReward(object sender, Reward args)
        {
            Time.timeScale = 1f;

            joystickController.SetActive(true);
            foreach (var button in uiButtons)
            {
                button.SetActive(true);
            }
            healthBar.fillAmount = 1f;
            PlayerRevived.Invoke();
            dieMenu.SetActive(false);
        }

        public void OpenRewardsAds()
        {
            if (_rewardedAd.IsLoaded())
            {
                _rewardedAd.Show();
            }
        }

        #endregion

        #region InterstitialAds

        private void RequestInterstitial()
        {
            string adUnitId = "";
#if UNITY_ANDROID
             adUnitId = "ca-app-pub-3940256099942544/1033173712";
#endif

            this._interstitialAd = new InterstitialAd(adUnitId);

            // Called when an ad request failed to load.
            this._interstitialAd.OnAdFailedToLoad += HandleOnAdFailedToLoad;
            // Called when an ad is shown.
            this._interstitialAd.OnAdOpening += HandleOnAdOpened;
            // Called when the ad is closed.
            this._interstitialAd.OnAdClosed += HandleOnAdClosed;



            // Create an empty ad request.
            AdRequest request = new AdRequest.Builder().Build();
            // Load the interstitial with the request.
            this._interstitialAd.LoadAd(request);
        }



        public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            MonoBehaviour.print("HandleFailedToReceiveAd event received with message: "
                                + args);
        }

        public void HandleOnAdOpened(object sender, EventArgs args)
        {
            RequestInterstitial();
            MonoBehaviour.print("HandleAdOpened event received");
        }

        public void HandleOnAdClosed(object sender, EventArgs args)
        {
            RequestInterstitial();
            winMenu.SetActive(true);
        }

        private void NextGameAds()
        {
            if (_interstitialAd.IsLoaded())
            {
                this._interstitialAd.Show();
            }
        }

        #endregion

        #region UI Methods

        private void SetBuildingCount()
        {
            buildingCount.text = "Building Count: " + LevelManager.Instance.GetBuildingCount().ToString();
        }

        public void OpenPauseMenu()
        {
            joystickController.SetActive(!joystickController.activeSelf);
            pauseMenu.SetActive(!pauseMenu.activeSelf);
            Time.timeScale = Mathf.Approximately(Time.timeScale, 0.0f) ? 1.0f : 0.0f;
        }

        public void TurnBackHome()
        {
            SceneManager.LoadScene("Menu", LoadSceneMode.Single);
        }

        public void RestartGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            pauseMenu.SetActive(false);
            dieMenu.SetActive(false);
            joystickController.SetActive(true);
            Time.timeScale = 1;
        }

        public void ResumeGame()
        {
            pauseMenu.SetActive(false);
            joystickController.SetActive(true);
            Time.timeScale = 1;
        }

        public void CloseSounds()
        {
            _toggle = !_toggle;

            if (_toggle)
                AudioListener.volume = 1f;

            else
                AudioListener.volume = 0f;
        }

        public void DisableButtons()
        {
            joystickController.SetActive(false);
            foreach (var eButton in uiButtons)
            {
                eButton.SetActive(false);
            }
        }

        private void CheckPlayerStatue()
        {
            StartCoroutine(EnableDieMenu());
        }

        public void PlayButtonClick()
        {
            audioSource.PlayOneShot(openMenuSound);
        }

        private IEnumerator EnableDieMenu()
        {
            yield return new WaitForSeconds(1.5f);
            dieMenu.SetActive(true);
        }

        public void LoadNextLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

        public void OpenWinMenu()
        {

            NextGameAds();
        }

        #endregion
    }
}