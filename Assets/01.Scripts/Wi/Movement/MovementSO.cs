using UnityEngine;

[CreateAssetMenu(menuName = "SO/Movement")]
public class MovementSO : ScriptableObject
{
    [Header("Speed")]
    [Tooltip("최고 속도")]
    public float maxSpeed;
    [Tooltip("가속도")]
    public float acceleration;
    [Tooltip("감속도")]
    public float deceleration;
    [Tooltip("대쉬 속도")]
    public float dashSpeed = 35f;

    [Header("Jump")]
    public float gravityScale;
    public float jumpPower;

    [Header("Wall")]
    [Tooltip("벽 반대로 튕겨져 나가는 힘")]
    public float wallJumpSpeedX;
    [Tooltip("점프 높이")]
    public float wallJumpSpeedY;

    [Header("Properties")]
    public float windAffect;
    public float holeAffect;
    public bool canBreakWall;
}
