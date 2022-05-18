using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UI;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class CheckInternetConnection : MonoBehaviour
{
    public static CheckInternetConnection Instance;
    public static Action<bool> OnConnectionChanged;
    private bool delay = true;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }
    private void Start()
    {
       
    }
    void FixedUpdate()
    {
        // This one use UnityEngine default internet internetReachability
         TestInternetConnection();

        // This one makes a HttpRequest every 5 seconds
        // MakeHttpRequest();
    }
    private void TestInternetConnection()
    {
        var activeScene = SceneManager.GetActiveScene();
        if (activeScene.name == "Menu")
            return;
        
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            OnConnectionChanged(false);
        }
        else
        {
            OnConnectionChanged(true);
        }
    }

    private async void MakeHttpRequest()
    {
        var activeScene = SceneManager.GetActiveScene();
        if (activeScene.name == "Menu")
            return;
        // This method maybe not healty because if you send too much requests, you will be blacklisted, if its your server you can use this method
        if (delay)
        {
            StartCoroutine(DelayToTestConnection());
            using var request = new UnityWebRequest("https://www.google.com/");
            var operation = request.SendWebRequest();

            while (!operation.isDone)
                await Task.Yield();

            if (request.result == UnityWebRequest.Result.Success)
            {
                OnConnectionChanged(true);
            }
            else
            {
                OnConnectionChanged(false);
            }
        }

    }

    private IEnumerator DelayToTestConnection()
    {
        delay = false;
        yield return new WaitForSeconds(5f);
        delay = true;
    }

}
