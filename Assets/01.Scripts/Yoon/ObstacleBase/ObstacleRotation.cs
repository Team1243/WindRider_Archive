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
    // nowStart : ������ �����ϸ� �������� �ٷ� ������ ������


    [Header("RotateValue")]
    [SerializeField] private float rotateSpeed;

    public bool IsCanRotate { get; set; } = true;

    private void Start()
    {
        // for debugging
        if (rotateSpeed == 0) Debug.LogError("ObstacleRotation : rotateSpeed is zero");

        if (immediatelyStart) ActRotateRoop();
    }

    // ȸ�� ���� �ڷ�ƾ ����
    public void ActRotateRoop()
    {
        StartCoroutine(RotateRoop());
    }

    // ȸ�� ����
    private IEnumerator RotateRoop()
    {
        while (IsCanRotate)
        {
            transform.rotation *= Quaternion.Euler(transform.rotation.x, transform.rotation.y, rotateSpeed * Time.deltaTime * (float)dir);
            yield return null;
        }
    }

    // ȸ�� ���� ��ȯ
    public void ChangeDir()
    {
        dir = (dir == RotateDir.Right) ? RotateDir.Left : RotateDir.Right;
    }

}