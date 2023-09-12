//  Made by Wei Renkui

using System;
using System.Collections;
using UnityEngine;

public enum MOVEMENT_MODE
{
    NORMAL,
    LIGHT,
    HEAVY,
}

public enum MOVEMENT_STATE
{
    IDLE,
    RUNNING,
    DASHING,
    AIRDASHING,
    ONAIR,
    WALL,
}

public class Movement : MonoBehaviour
{
    [Header("STATE")]
    [SerializeField][Tooltip("모드")]
    private MOVEMENT_MODE currentMode = MOVEMENT_MODE.NORMAL;
    [HideInInspector]
    public MOVEMENT_MODE CurrentMode => currentMode;
    [SerializeField][Tooltip("행동")]
    private MOVEMENT_STATE currentAction = MOVEMENT_STATE.IDLE;
    [HideInInspector]
    public MOVEMENT_STATE CurrentAction => currentAction;
    private MOVEMENT_STATE prevAction = MOVEMENT_STATE.IDLE;
    private MovementSO currentMoveData;

    [Header("MoveData")]
    [SerializeField]
    private CameraManager cameraManager;
    [SerializeField]
    private MovementSO normalMove;
    [SerializeField]
    private MovementSO lightMove;
    [SerializeField]
    private MovementSO heavyMove;
    private SpriteRenderer rend;

    [Header("Properties")]
    public bool MoveOnAir = true;

    [Header("Speed Properties")]
    private float xInput;

    [Header("Dash Properties")]
    //[SerializeField][Tooltip("대쉬할 때의 속도")]
    //private float dashSpeed;
    [SerializeField][Tooltip("대쉬의 지속시간")]
    private float dashDuration;
    [SerializeField]
    private float dashTime;
    [SerializeField]
    private float camRotAngle;
    private bool camRotated = false;

    [Header("Jump Properties")]
    [SerializeField]
    private LayerMask groundLayer;
    [SerializeField]
    private float coyoteTime = 0.2f;
    [SerializeField]
    private float remainingCoyote;

    [Header("Variable Values")]
    [SerializeField]
    private float currentSpeed;
    [SerializeField]
    private float currentGravity;

    [Header("Values")]
    [SerializeField]
    private bool isGround;
    public bool Grounded => isGround;
    [SerializeField]
    private int lookDirection = 1;
    [SerializeField]
    private int wallDirection;
    private bool canGround = true;
    private float groundAngle;

    private float wallDistance = 0.5f;
    private IEnumerator groundDelayCo;

    private Stamina stamina;
    private Rigidbody2D rigid;
    public Rigidbody2D AttachedRigid => rigid;
    private PlayerAnimation animator;

	private AudioSource _jumpAudioSource;

    public event Action<RaycastHit2D> OnGround;
    public event Action OnAir;

	#region UNITY METHOD
	private void Awake()
	{
        stamina = GetComponent<Stamina>();
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<PlayerAnimation>();
        rend = transform.Find("Visual").GetComponent<SpriteRenderer>();
        _jumpAudioSource = GetComponent<AudioSource>();

        // 초기화
        isGround = false;
        currentSpeed = 0;
        currentMoveData = normalMove;
        currentGravity = currentMoveData.gravityScale;
    }

	private void Update()
	{
        CheckWall(); //벽 확인
        CheckGround(); //착지 확인
        CheckCoyote(); //코요테 타임 계산
        ApplySpeed(); //속도 적용

        xInput = Input.GetAxisRaw("Horizontal");

        InputActions();

        OnStateChange();
        ReduceStamina();
    }

	private void FixedUpdate()
	{
		Move(xInput); //움직임
	}
	#endregion

	#region Input
	private void InputActions()
	{
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            Jump(); //점프
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            //StartDash();
        }
        if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.Z))
        {
            if (currentMode == MOVEMENT_MODE.LIGHT)
                ChangeMode(MOVEMENT_MODE.NORMAL);
            else
                ChangeMode(MOVEMENT_MODE.LIGHT);
        }
        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.X))
        {
            if (currentMode == MOVEMENT_MODE.HEAVY)
                ChangeMode(MOVEMENT_MODE.NORMAL);
            else
                ChangeMode(MOVEMENT_MODE.HEAVY);
        }
    }
	#endregion

	#region Move Methods
	private void ApplySpeed()
	{
        if (currentAction == MOVEMENT_STATE.WALL)
		{
            currentSpeed = 0;
            return;
		}

        float speed;
        if (isGround)
    		speed = rigid.velocity.magnitude;
        else
            speed = Mathf.Abs(rigid.velocity.x);

        // 소수점 셋째 자리 이하의 미세한 값 차이 제거
        speed = Mathf.FloorToInt(speed * 100f) / 100f;

        if (speed > currentSpeed || !isGround)
		{
            // 움직임의 방향과 애니메이션의 방향이 다르면 방향 전환
            if (rigid.velocity.x * lookDirection < 0)
		    {
                lookDirection = -lookDirection;
                animator.LookDirect(lookDirection);
            }
            currentSpeed = speed;
		}
	}

	private void Move(float direction)
	{
        float y = rigid.velocity.y;
        if (currentAction != MOVEMENT_STATE.WALL)
		{


            if (rigid.velocity.x == 0)
			{
                if (direction != 0)
                    lookDirection = (int)direction; //속도가 0일때만 방향을 바꿀 수 있음
                animator.LookDirect(lookDirection);
			}

            if (currentAction == MOVEMENT_STATE.DASHING)
			{
                rigid.velocity = transform.right * lookDirection * currentMoveData.dashSpeed;
			}
            else
			{
                rigid.velocity = transform.right * lookDirection * CalcSpeed(direction);
			}

            if (rigid.velocity.x == 0 && rigid.velocity.y == 0 && isGround)
			{
                ChangeAction(MOVEMENT_STATE.IDLE);
			}

        }

        if (!isGround)
		{
            y -= currentGravity * Time.fixedDeltaTime * 9.81f;
            rigid.velocity = new Vector2(rigid.velocity.x, y);
		}
	}

    private float CalcSpeed(float direction) //속도 계산
	{
        bool isLower = currentSpeed <= currentMoveData.maxSpeed;

        if (rigid.velocity.x * direction >= 0 && Mathf.Abs(direction) > 0 && isLower) //움직이는 방향과 가려는 방향이 같으면 가속
		{
            currentSpeed += Time.deltaTime * currentMoveData.acceleration;

            if (isGround)
                ChangeAction(MOVEMENT_STATE.RUNNING);
            else if (currentAction == MOVEMENT_STATE.AIRDASHING)
                ChangeAction(MOVEMENT_STATE.ONAIR);
        }
        else //움직이려는 방향과 가려는 방향이 다르거나 현재 속도가 최고 속도보다 빠르면 감속
		{
            if (isGround || (!isGround && direction != 0))
			{
                currentSpeed -= Time.deltaTime * currentMoveData.deceleration;
			}
		}

        if (isLower)
            currentSpeed = Mathf.Clamp(currentSpeed, 0, currentMoveData.maxSpeed);

        return currentSpeed;
	}

    public void StopMove()
	{
        xInput = 0;
        currentSpeed = 0;
	}
	#endregion

	#region Dash Methods
	private void StartDash()
	{
        if (currentAction == MOVEMENT_STATE.RUNNING) // 달리고 있을 때의 대쉬
		{
            ChangeAction(MOVEMENT_STATE.DASHING);
            dashTime = dashDuration;
        }
        else if (currentAction == MOVEMENT_STATE.ONAIR) // 공중에 떠있을 때의 대쉬
        {
            DashDirectly(-Vector2.up);
            currentSpeed = 0;
            ChangeAction(MOVEMENT_STATE.AIRDASHING);
            dashTime = dashDuration;
        }
    }

    private void DashDirectly(Vector2 direction) // 매개변수로 들어온 방향으로 일직선 대쉬
	{
        rigid.velocity = direction * currentMoveData.dashSpeed;
    }

    private void OnDashing()
	{
		if (currentAction == MOVEMENT_STATE.DASHING || currentAction == MOVEMENT_STATE.AIRDASHING)
		{
			if (isGround)
			{
                if (!camRotated)
                {
                    cameraManager?.CameraRotation(lookDirection > 0 ? camRotAngle : -camRotAngle, 1f);
                    camRotated = true;
                }

                if (xInput * lookDirection < 0)
					dashTime -= Time.deltaTime * 5f;
				else
					dashTime -= Time.deltaTime;
			}

			if (dashTime <= 0)
			{
                ChangeAction(MOVEMENT_STATE.RUNNING);
            }
        }
		else if (camRotated)
		{
			cameraManager?.CameraRotation(0, 1f);
			camRotated = false;
		}
	}
	#endregion

	#region Jump Methods
	private void Jump()
	{
        if ((isGround || currentAction == MOVEMENT_STATE.WALL || remainingCoyote > 0) && currentMode != MOVEMENT_MODE.HEAVY)
		{
            isGround = false;
            remainingCoyote = 0;
            currentGravity = currentMoveData.gravityScale;
            transform.position = new Vector3(transform.position.x + -transform.up.x, transform.position.y);
            transform.eulerAngles = Vector3.zero;
            _jumpAudioSource.Play();

            float xVelocity;
            float yVelocity;

            if (currentAction == MOVEMENT_STATE.WALL)
			{
			    xVelocity = currentMoveData.wallJumpSpeedX * wallDirection;
                if (rigid.velocity.y > 0)
                    yVelocity = rigid.velocity.y + currentMoveData.wallJumpSpeedY;
                else
                    yVelocity = currentMoveData.wallJumpSpeedY;

                currentSpeed = Mathf.Abs(xVelocity);
                lookDirection = wallDirection;
			}
            else
			{
                xVelocity = lookDirection * currentSpeed;

                if (rigid.velocity.y > 0) //위로 움직이는 힘이 있을 때의(경사로를 오를 때) 점프 공식
                    yVelocity = rigid.velocity.y + currentMoveData.jumpPower * 0.8f;
                else
                    yVelocity = currentMoveData.jumpPower;
			}

            rigid.velocity = new Vector2(xVelocity, yVelocity);
            ChangeAction(MOVEMENT_STATE.ONAIR);

            if (groundDelayCo != null)
                StopCoroutine(groundDelayCo);
            groundDelayCo = GroundDelay(0.1f);
            StartCoroutine(groundDelayCo);

            OnAir?.Invoke();
		}
	}

    public IEnumerator GroundDelay(float delay) //다중 점프를 방지하기 위한 착지 불가 쿨타임 코루틴
	{
        canGround = false;
        yield return new WaitForSeconds(delay);
        canGround = true;
        groundDelayCo = null;
	}

    private void CheckCoyote()
	{
        if (remainingCoyote > 0 && !isGround)
		{
            remainingCoyote -= Time.deltaTime;
		}
	}
	#endregion

	// -- 착지 확인 함수 -- //
	private void CheckGround()
	{
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -transform.up, 2f, groundLayer);
        groundAngle = Vector2.SignedAngle(Vector2.up, hit.normal);

        if (hit.collider != null && hit.distance <= 1.2f && canGround && Mathf.Abs(groundAngle) <= 55f) // 바닥에 닿아 있을 때
	    {
            if (isGround == false) // 착지했을 때
			{
                if (currentAction == MOVEMENT_STATE.AIRDASHING)
                    ChangeAction(MOVEMENT_STATE.DASHING);

                // 착지한 바닥의 각도 계산
                float deg = 90 - Mathf.Abs(Vector2.SignedAngle(-hit.normal, rigid.velocity));

                // 움직임의 속도와 착지한 바닥의 각도를 계산해 밀려나는 방향을 구한다
                Vector2 calculated = rigid.velocity + hit.normal * (Mathf.Sin(deg * Mathf.Deg2Rad) * rigid.velocity.magnitude);

                lookDirection = calculated.x < 0 ? -1 : 1;
                animator.LookDirect(lookDirection);

                currentSpeed = calculated.magnitude;
                rigid.velocity = transform.right * lookDirection * currentSpeed;

                if (rigid.velocity.x == 0)
                    ChangeAction(MOVEMENT_STATE.IDLE);
                else if (currentAction != MOVEMENT_STATE.DASHING)
                    ChangeAction(MOVEMENT_STATE.RUNNING);

                OnGround?.Invoke(hit);
            }

            isGround = true;
            currentGravity = 0;
            transform.eulerAngles = new Vector3(0, 0, groundAngle);
            transform.position = (Vector3)hit.point + transform.up;
		}
        else //공중에 떠있을 때 OnAir
		{
            // 공중에 떴을 때
            if (isGround)
			{
                remainingCoyote = coyoteTime;
                if (currentAction == MOVEMENT_STATE.DASHING)
				{
                    ChangeAction(MOVEMENT_STATE.ONAIR);
                }
                currentGravity = currentMoveData.gravityScale;

                OnAir?.Invoke();
            }
            float add = transform.eulerAngles.z == 0 ? 0 : 
                transform.eulerAngles.z < 0 ? 0.5f : -0.5f; // 부드러운 움직임을 위한 보정값
            Vector3 point = new Vector3(transform.position.x + add, transform.position.y + -transform.up.y);
            if (currentAction != MOVEMENT_STATE.WALL)
                transform.eulerAngles = Vector3.zero;
            transform.position = point + transform.up;
            isGround = false;

            if (currentAction != MOVEMENT_STATE.WALL && currentAction != MOVEMENT_STATE.AIRDASHING)
                ChangeAction(MOVEMENT_STATE.ONAIR);
        }
    }

    // -- 벽 함수 -- //
    /// <summary>
    /// 좌우 벽에 닿아있는지 확인하고, 벽에 닿으면 벽을 타는 함수
    /// </summary>
    private void CheckWall()
	{
        Vector3 startPos = transform.position - transform.up * 0.5f + Vector3.up * 0.5f;
        //우측 벽 확인
        RaycastHit2D hitRight = Physics2D.Raycast(startPos, Vector2.right, 2f, groundLayer);
        //좌측 벽 확인
        RaycastHit2D hitLeft = Physics2D.Raycast(startPos, -Vector2.right, 2f, groundLayer);

        Debug.DrawRay(startPos, Vector2.right * 2f, Color.red);
        Debug.DrawRay(startPos, -Vector2.right * 2f, Color.red);

        if (currentAction == MOVEMENT_STATE.WALL) //현재 상태가 WALL이 아닐 때만
		{
            if (hitRight.collider == null && hitLeft.collider == null)
                ChangeAction(MOVEMENT_STATE.ONAIR);
            return;
		}

        wallDirection = 0;

        // 우측에 벽이 있을 때
        float angle = Vector2.SignedAngle(Vector2.up, hitRight.normal);
        if (hitRight.collider != null && hitRight.distance <= 0.52f && Mathf.Abs(angle) == 90 && rigid.velocity.x >= 0)
		{
            int wallDir = hitRight.normal.x > 0 ? 1 : -1;
            StartWallSlide(hitRight.point + hitRight.normal * 0.5f, wallDir);
        }

        // 좌측에 벽이 있을 때
        angle = Vector2.SignedAngle(Vector2.up, hitLeft.normal);
        if (hitLeft.collider != null && hitLeft.distance <= 0.52f && Mathf.Abs(angle) == 90 && rigid.velocity.x <= 0)
		{
            int wallDir = hitLeft.normal.x > 0 ? 1 : -1;
            StartWallSlide(hitLeft.point + hitLeft.normal * 0.5f, wallDir);
        }
    }

    /// <summary>
    /// 벽에 붙어있는 상태로 전환
    /// </summary>
    /// <param name="position">이동할 위치</param>
    /// <param name="direction">벽이 있는 방향의 반대 방향</param>
    private void StartWallSlide(Vector2 position, int direction)
	{
        print(position);
        ChangeAction(MOVEMENT_STATE.WALL);
        currentGravity = currentMoveData.gravityScale;
        rigid.velocity = new Vector2(0, rigid.velocity.y + (-transform.right * currentSpeed * 0.3f * direction).y);
        transform.eulerAngles = Vector3.zero;
        transform.position = position;
        currentSpeed = 0;
        wallDirection = direction;
        lookDirection = direction;
        isGround = false;
        
        animator.LookDirect(lookDirection);
    }

    // 상태가 변경될 시 작동되는 함수
    private void OnStateChange()
	{
        if (prevAction != currentAction)
		{
            // 사용 예시
            // if (prevState == MOVEMENT_STATE.IDLE)    //이전 상태가 idle인 경우
            // if (currentState == MOVEMENT_STATE.IDLE) //다음 상태가 idle인 경우

            animator.ChangeAnimation(currentAction);

            prevAction = currentAction;
        }
    }

    private void ReduceStamina()
	{
        if (currentMode != MOVEMENT_MODE.NORMAL)
		{
            stamina.ReduceStamina(Time.deltaTime);
		}
	}

    private void ChangeAction(MOVEMENT_STATE action)
	{
        currentAction = action;
	}

    private void ChangeMode(MOVEMENT_MODE mode)
	{
        currentMode = mode;

		switch (mode)
		{
			case MOVEMENT_MODE.NORMAL:
                rend.color = Color.white;
                currentMoveData = normalMove;
				break;
			case MOVEMENT_MODE.LIGHT:
                rend.color = Color.cyan;
                currentMoveData = lightMove;
				break;
			case MOVEMENT_MODE.HEAVY:
                rend.color = Color.red;
                currentMoveData = heavyMove;
				break;
		}

        currentGravity = currentMoveData.gravityScale;

        InGameUI.Instance?.StateChange(currentMode);
	}
}
