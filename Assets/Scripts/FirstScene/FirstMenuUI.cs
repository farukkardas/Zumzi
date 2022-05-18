using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace FirstScene
{
    public class FirstMenuUI : MonoBehaviour
    {
        [SerializeField] private Button[] buttons;
        [SerializeField] private GameObject levelsPanel;
        [SerializeField] private GameObject firstLoginPanel;
        [SerializeField] private Animator buttonAnimator;
        [SerializeField] private Animator optionsAnimator;
        [SerializeField] private Animator contactAnimator;
        [SerializeField] private Animator titleAnimator;

        private static readonly int ContactButtonPosition = Animator.StringToHash("ContactButtonPosition");
        private static readonly int OptionsButtonPosition = Animator.StringToHash("OptionsButtonPosition");
        private static readonly int ButtonPositionChange = Animator.StringToHash("ButtonPositionChange");
        private static readonly int TitleAnimation = Animator.StringToHash("TitleAnimation");
        private static readonly int IdleState = Animator.StringToHash("IdleState");

        private void Start()
        {
            Time.timeScale = 1;
            StartCoroutine(ButtonsStartPositions());
          
        }
        

        public void OpenLevelPanel()
        {
            LevelPanelDelay(levelsPanel, firstLoginPanel);
        }

        public void OpenFirstPanel()
        {
            LevelPanelDelay(firstLoginPanel, levelsPanel);
        }


        private void LevelPanelDelay(GameObject activePanel, GameObject closePanel1)
        {
            activePanel.SetActive(true);
            closePanel1.SetActive(false);
        }

        private IEnumerator ButtonsStartPositions()
        {
            foreach (var vButton in buttons)
            {
                vButton.interactable = false;
            }

            buttonAnimator.SetTrigger(ButtonPositionChange);
               yield return new WaitForSeconds(0.5f);
            optionsAnimator.SetTrigger(OptionsButtonPosition);
                yield return new WaitForSeconds(0.5f);
            contactAnimator.SetTrigger(ContactButtonPosition);
               yield return new WaitForSeconds(0.2f);
            titleAnimator.SetTrigger(TitleAnimation);
               yield return new WaitForSeconds(2f);
            foreach (var vButton in buttons)
            {
                vButton.interactable = true;
            }
        }

        public void ContactButton()
        {
            Application.OpenURL("https://www.linkedin.com/in/faruk-kardas/");
        }
    }
}