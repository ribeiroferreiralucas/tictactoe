using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ViewerBehaviour : MonoBehaviour
{
    [SerializeField]
    private Text _gamePhaseText;
    [SerializeField]
    private Button _disconnectBtn;
    [SerializeField]
    private Transform _grid;
   [Header("-> Scenes")]
    [SerializeField]
    private GameObject MainSceneGameObject;
    private SlotBehaviour[] _slotsBehaviour;
    private SlotBehaviour _currentSelectedSlot;
    private string _playerTextFormat;
    private string _gamePhaseTextFormat;
    private CanvasGroup _gridCanvasGroup;

    private void Awake()
    {
        GameServices.ViewerBehaviour = this;
        _gamePhaseTextFormat = _gamePhaseText.text;
        _gridCanvasGroup = _grid.GetComponent<CanvasGroup>();
        _disconnectBtn.onClick.AddListener(Disconnect);
        _gridCanvasGroup.interactable = false;
        _slotsBehaviour = GetComponentsInChildren<SlotBehaviour>();

    }
    private void OnEnable()
    {
        InitializeSlots();
        GameServices.RequestStatus();
    }

    private void Disconnect()
    {
        InitializeSlots();
        GameServices.Disconnect();
    }

    internal void Disconnected()
    {
        _gamePhaseText.text = string.Format(_gamePhaseTextFormat, "You are Disconnected");

        WaitAndRun(0.5f, () =>
        {
            MainSceneGameObject.SetActive(true);
            gameObject.SetActive(false);
        });
    }

    private void InitializeSlots()
    {
        for (int i = 0; i < _slotsBehaviour.Length; i++)
        {
            _slotsBehaviour[i].Row = Math.Floor(i / 3f);
            _slotsBehaviour[i].Column = Math.Floor(i % 3f);
            _slotsBehaviour[i].Index = i;
            _slotsBehaviour[i].Mark(0);
        }
    }


    private void UpdateView()
    {
        int[] slots = GameServices.Slots;

        for (int i = 0; i < slots.Length; i++)
        {
            _slotsBehaviour[i].Mark(slots[i]);
        }
    }

    internal void UpdateTurn()
    {
        string turn =  GameServices.Status == GameServices.CurrentStatus.Player1Turn ? "Player 1 Turn" : "Player 2 Turn";
        _gamePhaseText.text = string.Format(_gamePhaseTextFormat, turn);
        UpdateView();
    }

    internal void Tie()
    {
        _gamePhaseText.text = string.Format(_gamePhaseTextFormat, "Game ended with a tie");
        _gridCanvasGroup.interactable = false;
        UpdateView();    
    }

    internal void VictoryPlayer1()
    {
        _gamePhaseText.text = string.Format(_gamePhaseTextFormat, "Player 1 won");
        _gridCanvasGroup.interactable = false;
        UpdateView();    
    }

    internal void VictoryPlayer2()
    {
        _gamePhaseText.text = string.Format(_gamePhaseTextFormat, "Player 2 won");
        _gridCanvasGroup.interactable = false;
        UpdateView();    
    }

    internal void WaitingOtherPlayer()
    {
        _gamePhaseText.text = string.Format(_gamePhaseTextFormat, "Waiting players to connect");
        _gridCanvasGroup.interactable = false;
        UpdateView();
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