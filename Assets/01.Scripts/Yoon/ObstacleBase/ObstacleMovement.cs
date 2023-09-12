using System.Collections;
using UnityEngine;

enum MoveDir
{
    Left = -1,
    Right = 1
}

public class ObstacleMovement : MonoBehaviour
{
    [Header("Option")]
    [SerializeField] private MoveDir dir = MoveDir.Right;
    [SerializeField] private float delayTime = 0f;
    [SerializeField] private bool immediatelyStart = true;
    // dir : �ش� ������Ʈ�� dir �������� ������
    // delayTime : ���� ������Ʈ�� �ϳ��� �������� �Ϸ��Ͽ��� �� �����̸� �ְ� �ٽ� �����̰� �ϰ� ������ üũ�ؾ� ��
    // nowStart : ������ �����ϸ� �������� �ٷ� ������ ������

    [Header("MoveValue")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float moveXRange;
    [SerializeField] private float moveYRange;

    private Vector3 startPos;
    public bool IsCanMove { get; set; } = true;

    private void Start()
    {
        // for debugging
        if (moveSpeed == 0) Debug.LogError("ObstacleMovement : moveSpeed is zero");

        startPos = transform.localPosition;

        if (immediatelyStart) ActMoveRoop();
    }

    #region Move

    // ������ ���� �ڷ�ƾ ����
    public void ActMoveRoop()
    {
        StartCoroutine(MoveRoop());
    }

    // ������ ���� �ڷ�ƾ
    private IEnumerator MoveRoop()
    {
        while (IsCanMove)
        {
            Vector3 addPos = Vector3.one;

            if (moveXRange + moveYRange != 0)
            {
                addPos = new Vector3((moveXRange * (float)dir), moveYRange, 0);
            }

            yield return MoveObject(startPos + addPos);
            yield return MoveObject(startPos - addPos);
            yield return new WaitForSeconds(delayTime);
        }
    }
    
    // targetPosition���� ������Ʈ�� ������ �ӵ��� �����̴� �ڷ�ƾ
    private IEnumerator MoveObject(Vector3 targetPosition)
    {
        float distance = Vector3.Distance(transform.localPosition, targetPosition);
        float duration = distance / moveSpeed;
        float startTime = Time.time;
        Vector3 startPosition = transform.localPosition;

        while (Time.time - startTime < duration)
        {
            float t = (Time.time - startTime) / duration;
            transform.localPosition = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        transform.localPosition = targetPosition;
    }

	#endregion

	#region Option

	// ������ ����
	[ContextMenu("StopMovement")]
    public void StopMovement()
    {
        StopAllCoroutines();
    }

    // ȸ�� ���� ��ȯ
    [ContextMenu("ChangeDir")]
    public void ChangeDir()
    {
        dir = (dir == MoveDir.Right) ? MoveDir.Left : MoveDir.Right;
    }

    #endregion
}