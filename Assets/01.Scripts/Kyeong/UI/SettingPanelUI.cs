using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class SettingPanelUI : MonoBehaviour
{
    private UIDocument _uiDocument;
    private VisualElement _root;

    private Slider[] _sliders = new Slider[3];
    private Slider _brightSlider;

    [SerializeField] private bool _isButton = false;
    [SerializeField] private AudioMixer _audioMixers;
    private AudioSource _audioSource;

    private UIDocument _brightUIDocument;
    private VisualElement _brightContainer;
    private Button _exitButton;
    private Button _menuButton;
    
    private bool _isSettingPanelShow = false;

    private void Awake()
    {
        _uiDocument = GetComponent<UIDocument>();
        _brightUIDocument = transform.root.Find("BrightControlUI").GetComponent<UIDocument>();
        _brightContainer = _brightUIDocument.rootVisualElement.Q<VisualElement>("Container");
        _audioSource = GetComponent<AudioSource>();
        _uiDocument.enabled = false;
    }

    private void Start()
    {
        for (int i = 0; i < 3; ++i)
        {
            if (!PlayerPrefs.HasKey($"Sound0{i+1}"))
                PlayerPrefs.SetFloat($"Sound0{i+1}", 0);
            
            float value = PlayerPrefs.GetFloat($"Sound0{i + 1}");
            _audioMixers.SetFloat($"Sound0{i + 1}", value);
        }
        
        if (!PlayerPrefs.HasKey("Bright"))
            PlayerPrefs.SetFloat("Bright", 0);
        
        float index = PlayerPrefs.GetFloat("Bright");
        _audioMixers.SetFloat("Bright", index);
        Color color = new Color(0, 0, 0, index / 255);
        _brightContainer.style.backgroundColor = new StyleColor(color);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_isButton)
            {
                _isSettingPanelShow = false;
                _uiDocument.enabled = false;
            }
            else
            {
                _isSettingPanelShow = !_isSettingPanelShow;
                _uiDocument.enabled = _isSettingPanelShow;
                if (_isSettingPanelShow)
                    DocumentShow();
            }
        }
        
        if (!_isSettingPanelShow)
            return;

        for (int i = 0; i < 3; ++i)
        {
            PlayerPrefs.SetFloat($"Sound0{i+1}", _sliders[i].value);
            _audioMixers.SetFloat($"Sound0{i + 1}", _sliders[i].value);
        }
        
        PlayerPrefs.SetFloat("Bright", _brightSlider.value);
        Color color = new Color(0, 0, 0, _brightSlider.value / 255);
        _brightContainer.style.backgroundColor = new StyleColor(color);
    }

    public void SettingButtonDown()
    {
        _isSettingPanelShow = true;
        _uiDocument.enabled = true;
        DocumentShow();
    }

    private void DocumentShow()
    {
        _root = _uiDocument.rootVisualElement;

        for (int i = 0; i < 3; ++i)
        {
            _sliders[i] = _root.Q<Slider>($"Slider0{i + 1}");
            _sliders[i].value = PlayerPrefs.GetFloat($"Sound0{i + 1}");
        }

        _brightSlider = _root.Q<Slider>("BrightSlider");
        _brightSlider.value = PlayerPrefs.GetFloat("Bright");
        _exitButton = _root.Q<Button>("ExitButton");
        _exitButton.RegisterCallback<ClickEvent>(evt =>
        {
            _isSettingPanelShow = false;
            _uiDocument.enabled = false;
            _audioSource.Play();
        });
        _menuButton = _root.Q<Button>("MenuButton");
        _menuButton.RegisterCallback<ClickEvent>(e =>
        {
            SceneManager.LoadScene(0);
            _audioSource.Play();
        });
    }
}
