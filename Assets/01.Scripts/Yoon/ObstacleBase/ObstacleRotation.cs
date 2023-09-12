using System.Collections;
using UnityEngine;

enum RotateDir
{
    Right = -1,
    Left = 1
}

public class ObstacleRotation : MonoBehaviour
{
    [Header("Option")]
    [SerializeField] private RotateDir dir = RotateDir.Right;
    [SerializeField] private bool immediatelyStart = true;
    // nowStart : 게임이 시작하면 움직임을 바로 시작할 것인지


    [Header("RotateValue")]
    [SerializeField] private float rotateSpeed;

    public bool IsCanRotate { get; set; } = true;

    private void Start()
    {
        // for debugging
        if (rotateSpeed == 0) Debug.LogError("ObstacleRotation : rotateSpeed is zero");

        if (immediatelyStart) ActRotateRoop();
    }

    // 회전 루프 코루틴 실행
    public void ActRotateRoop()
    {
        StartCoroutine(RotateRoop());
    }

    // 회전 루프
    private IEnumerator RotateRoop()
    {
        while (IsCanRotate)
        {
            transform.rotation *= Quaternion.Euler(transform.rotation.x, transform.rotation.y, rotateSpeed * Time.deltaTime * (float)dir);
            yield return null;
        }
    }

    // 회전 방향 전환
    public void ChangeDir()
    {
        dir = (dir == RotateDir.Right) ? RotateDir.Left : RotateDir.Right;
    }

}
