using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameBehaviour : MonoBehaviour
{
    [SerializeField]
    private Text _playerText;
    [SerializeField]
    private Text _gamePhaseText;
    [SerializeField]
    private Button _endTurnBtn;
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

    private void Start()
    {
        GameServices.GameBehaviour = this;
        _playerTextFormat = _playerText.text;
        _gamePhaseTextFormat = _gamePhaseText.text;
        _gridCanvasGroup = _grid.GetComponent<CanvasGroup>();
        _disconnectBtn.onClick.AddListener(Disconnect);
        _endTurnBtn.onClick.AddListener(Play);
        GameServices.CurrentGameStatus(GameServices.Status, GameServices.Slots);
        InitializeSlots();

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

    public void Play()
    {
        GameServices.PlayInSlot(_currentSelectedSlot.Index);
        _currentSelectedSlot = null;
        _endTurnBtn.interactable = false;
    }
    private void SlotSelected(SlotBehaviour slot)
    {
        if(slot.WasMarked) return;
        _currentSelectedSlot = slot;
        SelectSlot(slot);
        _endTurnBtn.interactable = true;
    }

    private void InitializeSlots()
    {
        _slotsBehaviour = GetComponentsInChildren<SlotBehaviour>();
        for (int i = 0; i < _slotsBehaviour.Length; i++)
        {
            _slotsBehaviour[i].Row = Math.Floor(i / 3f);
            _slotsBehaviour[i].Column = Math.Floor(i % 3f);
            _slotsBehaviour[i].Index = i;
            _slotsBehaviour[i].Mark(0);
            _slotsBehaviour[i].SetSelected(false);

            _slotsBehaviour[i].OnSelected += SlotSelected;
        }
    }

    private void SelectSlot(SlotBehaviour selectedSlot)
    {
        for (int i = 0; i < _slotsBehaviour.Length; i++)
        {
            var slot = _slotsBehaviour[i];

            if(slot.WasMarked)
            {
                continue;
            }
            
            slot.SetSelected(slot == selectedSlot);
        }
    }

    private void UpdateView()
    {
        int[] slots = GameServices.Slots;

        for (int i = 0; i < slots.Length; i++)
        {
            if(slots[i] == 0) continue;
            
            if(_slotsBehaviour[i].WasMarked) continue;

            _slotsBehaviour[i].Mark(slots[i]);
        }
    }

    internal void YourTurn()
    {
        _gamePhaseText.text = string.Format(_gamePhaseTextFormat, "Your turn");
        _gridCanvasGroup.interactable = true;
        UpdateView();
    }

    internal void OtherPlayerTurn()
    {
        _playerText.text = string.Format(_playerTextFormat, GameServices.PlayerId);
        _gamePhaseText.text = string.Format(_gamePhaseTextFormat, "Other player turn");
        _gridCanvasGroup.interactable = false;
        _endTurnBtn.interactable = false;
        UpdateView();
    }

    internal void Tie()
    {
        _gamePhaseText.text = string.Format(_gamePhaseTextFormat, "Game ended with a tie");
        _gridCanvasGroup.interactable = false;
        _endTurnBtn.interactable = false;
        UpdateView();    
    }

    internal void Victory()
    {
        _gamePhaseText.text = string.Format(_gamePhaseTextFormat, "You won");
        _gridCanvasGroup.interactable = false;
        _endTurnBtn.interactable = false;
        UpdateView();    
    }

    internal void Lose()
    {
        _gamePhaseText.text = string.Format(_gamePhaseTextFormat, "You lose");
        _gridCanvasGroup.interactable = false;
        _endTurnBtn.interactable = false;
        UpdateView();    
    }

    internal void WaitingOtherPlayer()
    {
        _playerText.text = string.Format(_playerTextFormat, GameServices.PlayerId);
        _gamePhaseText.text = string.Format(_gamePhaseTextFormat, "Waiting other player to connect");
        _gridCanvasGroup.interactable = false;
        _endTurnBtn.interactable = false;
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