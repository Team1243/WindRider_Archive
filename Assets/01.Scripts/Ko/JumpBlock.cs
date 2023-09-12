using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class JumpBlock : MonoBehaviour
{
    [Range(0f, 1f)]
    public float m_rate = 0f;

    public Transform m_p_Start;
    public Transform m_p_End;

    public Transform[] m_pos_Num = new Transform[2];

    private bool IsJump;
    private Transform player;
    private Movement movement;
    private AudioSource _audioSource;

    public UnityEvent OnEnd;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        _audioSource = GetComponent<AudioSource>();
    }
    void Update()
    {
        if (IsJump)
        {
            m_rate += Time.deltaTime * 2;
            player.transform.position = BezierCurve();
            if (m_rate >= 1)
            {
                OnEnd?.Invoke();
                sceneManager.instance.ChangeNextSceenAndTransitionsEffectNum(2);
                IsJump = false;
                movement.enabled = true;
                m_rate = 0;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<Movement>())
        {
            Debug.Log("Jump");
            _audioSource.Play();
            movement = other.gameObject.GetComponent<Movement>();
            IsJump = true;
            movement.enabled = false;

            if (SceneManager.GetActiveScene().buildIndex == 6)
                PlayerPrefs.SetInt("TutoClear", 1);
        }
    }

    Vector3 BezierCurve()
    {
        List<Vector3> t_pointList = new List<Vector3>();
        t_pointList.Add(m_p_Start.position + new Vector3(0, 1, 0));

        foreach (var T in m_pos_Num)
            t_pointList.Add(T.position);

        t_pointList.Add(m_p_End.position);

        while (t_pointList.Count > 1)
        {
            List<Vector3> t_resultList = new List<Vector3>();

            for (int i = 0; i < t_pointList.Count - 1; i++)
            {
                Vector3 result = Vector3.Lerp(t_pointList[i], t_pointList[i + 1], m_rate);
                t_resultList.Add(result);
            }

            t_pointList = t_resultList;
        }
        return t_pointList[0];
    }
}
