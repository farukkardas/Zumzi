using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    [SerializeField] private Animator circleAnimator;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip blackHoleSound;
    
    private void Start()
    {
        circleAnimator.Play("HoleOpenAnimation");
    }
}
