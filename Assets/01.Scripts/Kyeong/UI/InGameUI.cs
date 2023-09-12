using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InGameUI : MonoBehaviour
{
    public static InGameUI Instance = null;
    
    private UIDocument _uiDocument;
    private VisualElement _root;

    [Header("Slider")] 
    private VisualElement _hpSliderValue;
    private VisualElement _staminaSliderValue;

    [Header("State")] 
    [SerializeField] private Sprite[] _stateImg;
    private MOVEMENT_MODE _currentState;
    private Dictionary<MOVEMENT_MODE, int> _stateDic = new Dictionary<MOVEMENT_MODE, int>();
    private VisualElement[] _stateVisualElements = new VisualElement[3];

    private AudioSource _audioSource;

    private void Awake()
    {
        if (Instance != null)
            Debug.LogError("Multiple InGameUI is running");
        Instance = this;
        
        _uiDocument = GetComponent<UIDocument>();
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        _root = _uiDocument.rootVisualElement;

        Init();
    }

    private void Init()
    {
        _hpSliderValue = _root.Q<VisualElement>("HPSliderValue");
        _staminaSliderValue = _root.Q<VisualElement>("StaminaSliderValue");

        _currentState = MOVEMENT_MODE.NORMAL;
        for (int i = 0; i < Enum.GetValues(typeof(MOVEMENT_MODE)).Length; ++i)
        {
            _stateDic.Add((MOVEMENT_MODE)i, i);
            _stateVisualElements[i] = _root.Q($"CurrentStateValue0{i+1}"); 
        }
    }

    #region Slider

    public void ChangeHpValue(float value)
    {
        value = Mathf.Clamp(value, 0f, 100f);
        _hpSliderValue.style.flexBasis = Length.Percent(value);
    }
    
    public void ChangeStaminaValue(float value)
    {
        value = Mathf.Clamp(value, 0f, 100f);
        _staminaSliderValue.style.flexBasis = Length.Percent(value);
    }

    #endregion

    public void StateChange(MOVEMENT_MODE state)
    {
        (_stateDic[_currentState], _stateDic[state]) = (_stateDic[state], _stateDic[_currentState]);

        for (int i = 0; i < 3; ++i)
            _stateVisualElements[_stateDic[(MOVEMENT_MODE)i]].style.backgroundImage = new StyleBackground(_stateImg[i]);
        
        _currentState = state;
        if (_audioSource == null) return;
        _audioSource?.Play();
    }
}
