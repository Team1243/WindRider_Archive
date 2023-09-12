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
    // dir : 해당 오브젝트가 dir 방향으로 움직임
    // delayTime : 만약 오브젝트가 하나의 움직임을 완료하였을 때 딜레이를 주고 다시 움직이게 하고 싶으면 체크해야 함
    // nowStart : 게임이 시작하면 움직임을 바로 시작할 것인지

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

    // 움직임 루프 코루틴 실행
    public void ActMoveRoop()
    {
        StartCoroutine(MoveRoop());
    }

    // 움직임 루프 코루틴
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
    
    // targetPosition으로 오브젝트를 일정한 속도로 움직이는 코루틴
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

	// 움직임 정지
	[ContextMenu("StopMovement")]
    public void StopMovement()
    {
        StopAllCoroutines();
    }

    // 회전 방향 전환
    [ContextMenu("ChangeDir")]
    public void ChangeDir()
    {
        dir = (dir == MoveDir.Right) ? MoveDir.Left : MoveDir.Right;
    }

    #endregion
}