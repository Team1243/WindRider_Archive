using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NowStageText : MonoBehaviour
{
    TextMeshProUGUI _text;

    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
    }

    // for debug
    private void Update()
    {
        _text.text = MapManager.Instance.BeforeStage.gameObject.name.ToString();
    }
}
