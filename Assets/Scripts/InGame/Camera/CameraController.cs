using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform camTransform;


    private Vector3 _offset;
    private float SmoothTime = 0.1f;
    private Vector3 _velocity = Vector3.zero;
    private bool canFollow = false;

    private void Awake()
    {
    }

    private void Start()
    {
        StartCoroutine(SetPlayerTransform());
    }

    IEnumerator SetPlayerTransform()
    {
        yield return new WaitForSeconds(2f);
        GameObject player = GameObject.Find("Player(Clone)");
        Transform playerT = player.transform;
        playerTransform = playerT;
        _offset = camTransform.position - playerTransform.position;
        canFollow = true;
    }

    void Update()
    {
        if (canFollow)
        {
            CameraFollow();
        }
    }

    void CameraFollow()
    {
        Vector3 targetPosition = playerTransform.position + _offset;

        transform.position = Vector3.SmoothDamp(transform.position,
            new Vector3(targetPosition.x, targetPosition.y + 0.05f, targetPosition.z), ref _velocity, SmoothTime);

    
    }
}