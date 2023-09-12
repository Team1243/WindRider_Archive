using UnityEngine;

[CreateAssetMenu(menuName = "SO/Movement")]
public class MovementSO : ScriptableObject
{
    [Header("Speed")]
    [Tooltip("�ְ� �ӵ�")]
    public float maxSpeed;
    [Tooltip("���ӵ�")]
    public float acceleration;
    [Tooltip("���ӵ�")]
    public float deceleration;
    [Tooltip("�뽬 �ӵ�")]
    public float dashSpeed = 35f;

    [Header("Jump")]
    public float gravityScale;
    public float jumpPower;

    [Header("Wall")]
    [Tooltip("�� �ݴ�� ƨ���� ������ ��")]
    public float wallJumpSpeedX;
    [Tooltip("���� ����")]
    public float wallJumpSpeedY;

    [Header("Properties")]
    public float windAffect;
    public float holeAffect;
    public bool canBreakWall;
}
