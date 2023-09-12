using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PracticeSelectUI : MonoBehaviour
{
    private UIDocument _uiDocument;
    private VisualElement _root;

    private VisualElement[] _stages = new VisualElement[6];

    private int _currentIndex = 0;
    private bool _isPracticeSelectUIShow = false;

    private void Awake()
    {
        _uiDocument = GetComponent<UIDocument>();
        _uiDocument.enabled = false;
    }

    private void Update()
    {
        if (!_isPracticeSelectUIShow)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _isPracticeSelectUIShow = false;
            _uiDocument.enabled = false;
        }

        if (Input.GetKeyDown(KeyCode.Space))
            SceneManager.LoadScene(_currentIndex + 1);

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            PressToLeft();
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            PressToRight();
    }

    public void PracticeSelectShow()
    {
        _isPracticeSelectUIShow = true;
        _uiDocument.enabled = true;
        
        _root = _uiDocument.rootVisualElement;

        for (int i = 0; i < 6; ++i)
            _stages[i] = _root.Q<VisualElement>($"Stage0{i+1}");
    }

    private void PressToRight()
    {
        if (_currentIndex == 5)
            return;
        
        _stages[_currentIndex].RemoveFromClassList("show");
        _stages[_currentIndex].AddToClassList("unshow");

        ++_currentIndex;
        
        _stages[_currentIndex].AddToClassList("show");
    }
    
    private void PressToLeft()
    {
        if (_currentIndex == 0)
            return;
        
        _stages[_currentIndex].RemoveFromClassList("show");

        --_currentIndex;
        
        _stages[_currentIndex].RemoveFromClassList("unshow");
        _stages[_currentIndex].AddToClassList("show");
    }
}
