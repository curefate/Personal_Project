using UnityEngine;
using NativeWebSocket;
using UnityEngine.Events;
using System;




#if UNITY_EDITOR
using UnityEditor;
#endif

public class WebSocketButton : MonoBehaviour
{
    private WebSocket websocket;
    public string serverIP = "XXX.XXX.XXX.XXX"; // Replace with your server's IP address
    public int serverPort = 8081; // Replace with your server's port number (8081 is the default)
    public AudioSource bgmAudioSource;
    public AudioClip cheaterClip;
    public AudioClip buyClip;
    public AudioClip jet2Clip;

    private bool _cheated;

    async void Start()
    {
        websocket = new WebSocket("ws://" + serverIP + ":" + serverPort);
        Debug.Log("Attempting to connect to WebSocket server at ws://" + serverIP + ":" + serverPort);

        //Runs when connected to the server
        websocket.OnOpen += async () =>
        {
            Debug.Log("Connected to WebSocket server");
            string UUID = SystemInfo.deviceUniqueIdentifier; // Certain devices block MAC address access for privacy reasons so we send a UUID instead

            await websocket.SendText("Device (Unity):" + SystemInfo.deviceName + " ... Device's Unique Identifier: " + UUID);
        };

        //Runs when a message is received from the server
        websocket.OnMessage += (bytes) =>
        {
            string message = System.Text.Encoding.UTF8.GetString(bytes);
            Debug.Log("Received: " + message);

            IncomingMessageParser(message);

        };

        //Runs when disconnected from the server
        websocket.OnClose += (code) =>
        {
            Debug.Log("WebSocket closed");
        };

        await websocket.Connect();
    }

    void Update()
    {
        //Although not necessary for our lab, I have left this here as a reference
        //Websockets will not work on WebGL builds so with this preprocessor directive we include all builds except WebGL as well as including the editor for testing purposes
#if !UNITY_WEBGL || UNITY_EDITOR

        websocket.DispatchMessageQueue();
#endif
    }

    async void OnDestroy()
    {
        if (websocket != null)
            await websocket.Close();
    }

    public async void SendLedValue(int value)
    {
        if (websocket != null && websocket.State == WebSocketState.Open)
        {
            await websocket.SendText("LED_INTENSITY:" + value);
            Debug.Log("Sent: LED_INTENSITY:" + value);
        }
        else
        {
            Debug.LogWarning("WebSocket not connected");
        }
    }

    public void IncomingMessageParser(String msg)
    {
        string valueParsed = msg.Substring(msg.IndexOf(":") + 1);

        if (msg.Contains("button"))
        {
            if (valueParsed == "1")
            {
                //do something if button pressed
            }
            if (valueParsed == "0")
            {
                //do something if button released
                if (_cheated) return;
                _cheated = true;
                GoldManager goldManager = FindFirstObjectByType<GoldManager>();
                goldManager.Gold += 9999;
                bgmAudioSource.clip = jet2Clip;
                bgmAudioSource.Play();
                bgmAudioSource.PlayOneShot(buyClip);
                var currentAudioSource = GetComponent<AudioSource>();
                currentAudioSource.clip = cheaterClip;
                currentAudioSource.Play();
            }
        }
    }
}