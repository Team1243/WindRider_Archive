using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundDistance : MonoBehaviour
{
    [SerializeField] private float _maxDistance;
    [SerializeField] private float _minDistance;
    
    private AudioSource _audioSource;
    private Transform _playerTrm;
    private float _distance;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        _playerTrm = GameManager.Instance.PlayerObj.transform;
    }

    private void FixedUpdate()
    {
        _distance = Vector2.Distance(transform.position, _playerTrm.position);
        if (_distance > _maxDistance)
            _audioSource.volume = 0;
        else if (_minDistance > _distance)
            _audioSource.volume = 1;
        else
            _audioSource.volume = (_minDistance / _distance);
    }

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _minDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _maxDistance);
        Gizmos.color = Color.white;
    }

#endif
}