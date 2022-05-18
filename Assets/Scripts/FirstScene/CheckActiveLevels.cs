using System;
using System.Collections;
using System.Collections.Generic;
using FirstScene;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CheckActiveLevels : MonoBehaviour
{
    [SerializeField] private Button[] levelButtons;
    [SerializeField] private TMP_Text currentLevelText;

    private void Awake()
    {
        if (!PlayerPrefs.HasKey("MaxLevel"))
        {
            PlayerPrefs.SetInt("MaxLevel", 1);
        }
    }


    private void LateUpdate()
    {
        CheckLevelButtons();
    }

    private void CheckLevelButtons()
    {
        foreach (var button in levelButtons)
        {
            int maxLevel = PlayerPrefs.GetInt("MaxLevel");

            currentLevelText.text = maxLevel.ToString();

            LevelButton currentButton = button.GetComponent<LevelButton>();

            if (currentButton.levelIndex <= maxLevel)
            {
                Transform lockImageParent = currentButton.transform.Find("LockImage");
                Image lockImage = lockImageParent.GetComponent<Image>();
                lockImage.enabled = false;
                button.interactable = true;
            }

            else
            {
                button.interactable = false;
            }
        }
    }
}