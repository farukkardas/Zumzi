using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FirstScene
{
    public class LoadLevel : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        private static readonly int TriggerLevel = Animator.StringToHash("EndTrigger");


        private void Start()
        {
            StartCoroutine(ChangeLevel());
        }

        private IEnumerator ChangeLevel()
        {
            animator.SetTrigger(TriggerLevel);
            yield return new WaitForSeconds(2f);
            
        }
    }
}