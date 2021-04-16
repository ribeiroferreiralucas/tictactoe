using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

internal class SlotBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler 
{
    [SerializeField]
    private Color _markedColor;
    [SerializeField]
    private Color _hoverColor;
    [SerializeField]
    private Color _selectedColor;
    private Button _btn;
    private Text[] _marks;

    public double Row { get; internal set; }
    public double Column { get; internal set; }
    public Action<SlotBehaviour> OnSelected { get; internal set; }
    public bool WasMarked { get; internal set; }

    private int _playerIdMark;

    public int Index { get; internal set; }
    public bool IsSelected { get; private set; }
    public Text[] Marks 
    { 
        get 
        {
            if(_marks == null)
                _marks = GetComponentsInChildren<Text>();
            return _marks;
        } 
    }

    public Button Btn 
    { 
        get
        {
            if(!_btn)
                _btn = GetComponent<Button>();
            return _btn;
        }
    }

    private void Awake()
    {
        for (int i = 0; i < Marks.Length; i++)
        {
            Marks[i].enabled = false;
        }

        Btn.onClick.AddListener(OnClick);
    }

    public void Mark(int playerId)
    {
        
        WasMarked = !(playerId == 0);
        _playerIdMark = playerId;
        if(playerId == 0)
        {
            for (int i = 0; i < Marks.Length; i++)
            {
                Marks[i].enabled = false;
            }        
        }
        else{
            Marks[playerId-1].enabled = true;
            Marks[playerId-1].color = _selectedColor;
        }
        Btn.interactable = !WasMarked;
    }

    private void OnClick()
    {
        if(WasMarked) return;
        OnSelected.Invoke(this);
    }

    internal void SetSelected(bool selected)
    {
        IsSelected = selected;
        Marks[GameServices.PlayerId -1].enabled = IsSelected;
        Marks[GameServices.PlayerId-1].color = _selectedColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(WasMarked || IsSelected || GameServices.PlayerId == 0 ) return;
        
        Marks[GameServices.PlayerId-1].enabled = true;
        Marks[GameServices.PlayerId-1].color = _hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(WasMarked || IsSelected || GameServices.PlayerId == 0) return;
        Marks[GameServices.PlayerId-1].enabled = false;
    }
}