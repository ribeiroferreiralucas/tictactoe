using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ConnectPanelBehaviour : MonoBehaviour
{
    [Header("Text Fields")]
    [SerializeField]
    private Text _statusText;
    [SerializeField]
    private Text _errorText;

    [Header("Buttons")]
    [SerializeField]
    private Button _connectAsPlayerBtn;
    [SerializeField]
    private Button _connectAsViewerBtn;

    [Header("-> Scenes")]
    [SerializeField]
    private Canvas _gameScene;
    [SerializeField]
    private Canvas _viewerScene;
    private Canvas _canvas;

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
        _connectAsPlayerBtn.onClick.AddListener(ConnectAsPlayer);
        _connectAsViewerBtn.onClick.AddListener(ConnectAsViewer);
    }

    private void OnEnable()
    {
        SetButtonsInteractable(true);
    }

    private void SetButtonsInteractable(bool interactable)
    {
        _connectAsPlayerBtn.interactable = interactable;
        _connectAsViewerBtn.interactable = interactable;
    }

    private void ShowError(string message)
    {
        _statusText.enabled = false;
        _errorText.enabled = true;
        _errorText.text = message;
    }

    private void ShowStatus(string message)
    {
        _statusText.enabled = true;
        _errorText.enabled = false;

        _statusText.text = message;
    }
    private void ToGame()
    {
        _canvas.gameObject.SetActive(false);
        _gameScene.gameObject.SetActive(true);

    }
    private void ToViewer()
    {
        _canvas.gameObject.SetActive(false);
        _viewerScene.gameObject.SetActive(true);

    }
    
    private void ConnectAsPlayer()
    {
        SetButtonsInteractable(false);

        ShowStatus($"Tring to Connect to the Server");
        GameServices.ConnectToServer(SuccessToConnectedToServer, ErrorToConnectToServer);
    }
    private void SuccessToConnectedToServer()
    {
        ShowStatus($"Successfully connected to the Server. Trying to register as Player");
        GameServices.RegisterAsPlayer(SuccessToRegisterAsPlayer, ErrorToRegisterAsPlayer);
     }

    private void SuccessToRegisterAsPlayer()
    {
        ShowStatus($"Successfully registered as Player. Your player name is Player {GameServices.PlayerId}.\nMoving to GameScene");
        WaitAndRun(0.5f, ToGame);
    }

    private void ErrorToRegisterAsPlayer(string error)
    {
        ShowError($"Error to register as player: {error}");
        SetButtonsInteractable(true);
    }

    private void ErrorToConnectToServer(string error)
    {
        ShowError($"Error connecting to the server: {error}");
        SetButtonsInteractable(true);
    }

 

    private void ConnectAsViewer()
    {
        SetButtonsInteractable(false);
        ShowStatus($"Tring to Connect to the Server as Viewer");
        GameServices.ConnectToServer(SuccessToConnectedViewerToServer, ErrorToConnectToServer);
    }

    private void SuccessToConnectedViewerToServer()
    {
        ShowStatus($"Successfully registered as Player. Your player name is Player {GameServices.PlayerId}.\nMoving to GameScene");
        WaitAndRun(0.5f, ToViewer);
    }


    private void WaitAndRun(float secondsToWait, Action run)
    {
        StartCoroutine(WaitAndRunCoroutine(secondsToWait, run));
    }
    IEnumerator WaitAndRunCoroutine(float secondsToWait, Action run)
    {        
        yield return new WaitForSeconds(secondsToWait);
        run?.Invoke();
    }
}
