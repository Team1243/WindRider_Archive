
using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;
    private CinemachineVirtualCamera _followCam;

    [Header("Camera Rotation")] 
    private IEnumerator _rotationCo;
    private float _currentRotationTime;

    [Header("Camera Shake")] 
    private IEnumerator _shakeCo;
    private CinemachineBasicMultiChannelPerlin _noise = null;

    private void Awake()
    {
        if (Instance != null)
            Debug.LogError("Multiple CameraManager is running");
        Instance = this;
    }

    private void Start()
    {
        _followCam = GetComponent<CinemachineVirtualCamera>();
        _noise = _followCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        _rotationCo = CameraRotationCo(0, 0);
        _shakeCo = CameraShakeCo(0, 0);

        CameraReset();
    }

    private void CameraReset()
    {
        _followCam.m_Lens.Dutch = 0;
    }

    #region Camera Rotation

    // 오른쪽으로 회전 -
    // 왼쪽으로 회전 +
    public void CameraRotation(float value, float duration, Action callBack = null) 
    {
        StopCoroutine(_rotationCo);
        _rotationCo = CameraRotationCo(value, duration, callBack);
        StartCoroutine(_rotationCo);
        CameraShake(5, duration);
    }

    private IEnumerator CameraRotationCo(float value, float duration, Action callBack = null)
    {
        float firstIndex = _followCam.m_Lens.Dutch; 
        _currentRotationTime = 0;
        
        while (_currentRotationTime < duration)
        {
            yield return null;
            _currentRotationTime += Time.deltaTime;

            if (_currentRotationTime >= duration)
                _currentRotationTime = duration;

            float time = _currentRotationTime / duration;
            time = (float)(time == 1 ? 1 : 1 - Math.Pow(2, -10 * time));
            _followCam.m_Lens.Dutch = Mathf.Lerp(firstIndex, value, time);
        }
        
        callBack?.Invoke();
    }

    #endregion

    #region Camera Shake

    public void CameraShake(float amplitude, float duration)
    {
        StopCoroutine(_shakeCo);
        _shakeCo = CameraShakeCo(amplitude, duration);
        StartCoroutine(_shakeCo);   
    }

    private IEnumerator CameraShakeCo(float amplitude, float duration)
    {
        yield return new WaitForEndOfFrame();
        
        float time = duration;
        while (time > 0)
        {
            _noise.m_AmplitudeGain = Mathf.Lerp(0, amplitude, time / duration);

            yield return null;
            time -= Time.deltaTime;
        }
        _noise.m_AmplitudeGain = 0;

    }

    #endregion
}
