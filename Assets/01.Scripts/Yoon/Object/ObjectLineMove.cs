using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ObjectLineMove : MonoBehaviour
{
    private LineRenderer _lineRenderer;
    private Transform triggerTr;

    [SerializeField] private Vector3[] pointList;
    public Vector3[] PointList => pointList;
    private int index = 0;

    public bool IsCanMove { get; set; } = true;

    [SerializeField] private bool targetIsPlayer = false; // Ÿ���� �÷��̾�����
    private Transform targetTr;

    [SerializeField] private float moveSpeed = 10;

    [SerializeField] private ParticleSystem lineEffect;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        triggerTr = transform.Find("Trigger");
    }

    private void Start()
    {
        // for error defence
        if (pointList.Length < 4) Debug.LogError("ObstacleLineMove : Point is not enough");

        DrawLine();
        ZPosCheck();

        if (targetIsPlayer)
        {
            targetTr = GameManager.Instance.PlayerObj.transform;
            triggerTr.position = pointList[0];
        }
        else targetTr = this.transform;

        if (!targetIsPlayer) AcvitateMoveRoop();
    }

    private void Update()
    {
        if (lineEffect == null) return;

        if (lineEffect.isPlaying && lineEffect != null)
        {
            lineEffect.transform.position = targetTr.position;
        }
    }

    #region ETC

    private void DrawLine()
    {
        if (targetIsPlayer)
        {
            _lineRenderer.positionCount = pointList.Length;
            for (int i = 0; i < pointList.Length; i++)
            {
                _lineRenderer.SetPosition(i, pointList[i]);
            }
        }
        else
        {
            _lineRenderer.positionCount = pointList.Length + 1;
            for (int i = 0; i <= pointList.Length; i++)
            {
                if (i == pointList.Length)
                {
                    _lineRenderer.SetPosition(i, pointList[0]);
                    break;
                }
                _lineRenderer.SetPosition(i, pointList[i]);
            }
        }
    }

    private void ZPosCheck()
    {
        for (int i = 0; i < pointList.Length; i++)
        {
            if (pointList[i].z >= 0)
            {
                pointList[i] = new Vector3(pointList[i].x, pointList[i].y, -0.1f);
            }
        }
    }

    #endregion

    #region MoveRoop

    public void AcvitateMoveRoop()
    {
        StartCoroutine(LineMoveRoop());
    }

    private IEnumerator LineMoveRoop()
    {
        bool isPlus = true;

        while (IsCanMove)
        {
            yield return MoveObject(transform, pointList[index]);

            if (isPlus) index++;
            else index = 0;

            if (index == pointList.Length - 1 || index == 0)
            {
                isPlus = !isPlus;
            }
        }
    }

    #endregion

    #region MoveEnd

    public void GoToEnd()
    {
        StartCoroutine(MoveToEnd());
        lineEffect.Play();
    }

    private IEnumerator MoveToEnd()
    {
        while (IsCanMove)
        {
            yield return MoveObject(targetTr, pointList[index]);

            if (index == pointList.Length - 1)
            {
                lineEffect.Stop();
                index = 0;
                yield break;
            }
            index++;
        }
    }

    #endregion

    private IEnumerator MoveObject(Transform tr, Vector3 targetPosition)
    {
        float distance = Vector3.Distance(tr.position, targetPosition);
        float duration = distance / moveSpeed;
        float startTime = Time.time;
        Vector3 startPosition = tr.position;

        while (Time.time - startTime < duration)
        {
            float t = (Time.time - startTime) / duration;
            tr.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        tr.position = targetPosition;
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < pointList.Length - 1; i++)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(pointList[i], radius: 0.3f);

            if (i < pointList.Length)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawLine(pointList[i], pointList[i + 1]);
            }
        }
    }
}

#if UNITY_EDITOR

[CanEditMultipleObjects]
[CustomEditor(typeof(ObjectLineMove))]
public class PointEditor : Editor
{
    private void OnSceneGUI()
    {
        ObjectLineMove _obstacleLineMove = (ObjectLineMove)target;
        for (int i = 0; i < _obstacleLineMove.PointList.Length; i++)
        {
            _obstacleLineMove.PointList[i] = Handles.PositionHandle(_obstacleLineMove.PointList[i], Quaternion.identity);

            GUIStyle setWords = new GUIStyle();
            setWords.fontSize = 15;
            setWords.fontStyle = FontStyle.Bold;
            setWords.normal.textColor = Color.magenta;
            Vector3 textAllignment = Vector3.down * 0.2f;

            Handles.Label(_obstacleLineMove.PointList[i] + textAllignment, text: $"{i + 1}", setWords);

            EditorGUI.EndChangeCheck();
        }
    }
}

#endif
