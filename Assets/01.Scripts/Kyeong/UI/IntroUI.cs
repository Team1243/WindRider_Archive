using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class IntroUI : MonoBehaviour
{
    private UIDocument _uiDocument;
    private VisualElement _root;

    private Button _practiceButton;
    private Button _startButton;
    private Button _settingButton;
    private Button _exitButton;

    private SettingPanelUI _settingPanelUI;
    private PracticeSelectUI _practiceSelectUI;
    private AudioSource _audioSource;
    
    private void Awake()
    {
        _uiDocument = GetComponent<UIDocument>();
        _audioSource = GetComponent<AudioSource>();
        _settingPanelUI = transform.root.Find("SettingPanel").GetComponent<SettingPanelUI>();
        _practiceSelectUI = transform.root.Find("PracticeSelectUI").GetComponent<PracticeSelectUI>();
    }

    private void OnEnable()
    {
        _root = _uiDocument.rootVisualElement;

        _practiceButton = _root.Q<Button>("PracticeButton");
        _practiceButton.RegisterCallback<ClickEvent>(evt =>
        {
            _practiceSelectUI.PracticeSelectShow();
            _audioSource.Play();
        });
        _startButton = _root.Q<Button>("StartButton");
        _startButton.RegisterCallback<ClickEvent>(evt => SceneManager.LoadScene(7) );
        _settingButton = _root.Q<Button>("SettingButton");
        _settingButton.RegisterCallback<ClickEvent>(evt =>
        {
            _settingPanelUI.SettingButtonDown();
            _audioSource.Play();
        });
        _exitButton = _root.Q<Button>("ExitButton");
        _exitButton.RegisterCallback<ClickEvent>(evt =>
        {
            Application.Quit();
            _audioSource.Play();
        });

        if (PlayerPrefs.GetInt("TutoClear") == 1)
        {
            _startButton.AddToClassList("TutoClear");
        }
    }
}
