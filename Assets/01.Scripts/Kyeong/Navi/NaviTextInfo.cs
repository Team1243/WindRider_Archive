using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class NaviTextInfo : MonoBehaviour
{
    private TextMeshProUGUI _text;

    private void Awake()
    {
        _text = transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        _text.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        _text.enabled = true;
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        _text.enabled = false;
    }
}
