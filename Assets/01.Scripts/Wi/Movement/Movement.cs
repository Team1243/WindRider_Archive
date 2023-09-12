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
    [SerializeField][Tooltip("���")]
    private MOVEMENT_MODE currentMode = MOVEMENT_MODE.NORMAL;
    [HideInInspector]
    public MOVEMENT_MODE CurrentMode => currentMode;
    [SerializeField][Tooltip("�ൿ")]
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
    //[SerializeField][Tooltip("�뽬�� ���� �ӵ�")]
    //private float dashSpeed;
    [SerializeField][Tooltip("�뽬�� ���ӽð�")]
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

        // �ʱ�ȭ
        isGround = false;
        currentSpeed = 0;
        currentMoveData = normalMove;
        currentGravity = currentMoveData.gravityScale;
    }

	private void Update()
	{
        CheckWall(); //�� Ȯ��
        CheckGround(); //���� Ȯ��
        CheckCoyote(); //�ڿ��� Ÿ�� ���
        ApplySpeed(); //�ӵ� ����

        xInput = Input.GetAxisRaw("Horizontal");

        InputActions();

        OnStateChange();
        ReduceStamina();
    }

	private void FixedUpdate()
	{
		Move(xInput); //������
	}
	#endregion

	#region Input
	private void InputActions()
	{
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            Jump(); //����
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

        // �Ҽ��� ��° �ڸ� ������ �̼��� �� ���� ����
        speed = Mathf.FloorToInt(speed * 100f) / 100f;

        if (speed > currentSpeed || !isGround)
		{
            // �������� ����� �ִϸ��̼��� ������ �ٸ��� ���� ��ȯ
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
                    lookDirection = (int)direction; //�ӵ��� 0�϶��� ������ �ٲ� �� ����
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

    private float CalcSpeed(float direction) //�ӵ� ���
	{
        bool isLower = currentSpeed <= currentMoveData.maxSpeed;

        if (rigid.velocity.x * direction >= 0 && Mathf.Abs(direction) > 0 && isLower) //�����̴� ����� ������ ������ ������ ����
		{
            currentSpeed += Time.deltaTime * currentMoveData.acceleration;

            if (isGround)
                ChangeAction(MOVEMENT_STATE.RUNNING);
            else if (currentAction == MOVEMENT_STATE.AIRDASHING)
                ChangeAction(MOVEMENT_STATE.ONAIR);
        }
        else //�����̷��� ����� ������ ������ �ٸ��ų� ���� �ӵ��� �ְ� �ӵ����� ������ ����
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
        if (currentAction == MOVEMENT_STATE.RUNNING) // �޸��� ���� ���� �뽬
		{
            ChangeAction(MOVEMENT_STATE.DASHING);
            dashTime = dashDuration;
        }
        else if (currentAction == MOVEMENT_STATE.ONAIR) // ���߿� ������ ���� �뽬
        {
            DashDirectly(-Vector2.up);
            currentSpeed = 0;
            ChangeAction(MOVEMENT_STATE.AIRDASHING);
            dashTime = dashDuration;
        }
    }

    private void DashDirectly(Vector2 direction) // �Ű������� ���� �������� ������ �뽬
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

                if (rigid.velocity.y > 0) //���� �����̴� ���� ���� ����(���θ� ���� ��) ���� ����
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

    public IEnumerator GroundDelay(float delay) //���� ������ �����ϱ� ���� ���� �Ұ� ��Ÿ�� �ڷ�ƾ
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

	// -- ���� Ȯ�� �Լ� -- //
	private void CheckGround()
	{
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -transform.up, 2f, groundLayer);
        groundAngle = Vector2.SignedAngle(Vector2.up, hit.normal);

        if (hit.collider != null && hit.distance <= 1.2f && canGround && Mathf.Abs(groundAngle) <= 55f) // �ٴڿ� ��� ���� ��
	    {
            if (isGround == false) // �������� ��
			{
                if (currentAction == MOVEMENT_STATE.AIRDASHING)
                    ChangeAction(MOVEMENT_STATE.DASHING);

                // ������ �ٴ��� ���� ���
                float deg = 90 - Mathf.Abs(Vector2.SignedAngle(-hit.normal, rigid.velocity));

                // �������� �ӵ��� ������ �ٴ��� ������ ����� �з����� ������ ���Ѵ�
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
        else //���߿� ������ �� OnAir
		{
            // ���߿� ���� ��
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
                transform.eulerAngles.z < 0 ? 0.5f : -0.5f; // �ε巯�� �������� ���� ������
            Vector3 point = new Vector3(transform.position.x + add, transform.position.y + -transform.up.y);
            if (currentAction != MOVEMENT_STATE.WALL)
                transform.eulerAngles = Vector3.zero;
            transform.position = point + transform.up;
            isGround = false;

            if (currentAction != MOVEMENT_STATE.WALL && currentAction != MOVEMENT_STATE.AIRDASHING)
                ChangeAction(MOVEMENT_STATE.ONAIR);
        }
    }

    // -- �� �Լ� -- //
    /// <summary>
    /// �¿� ���� ����ִ��� Ȯ���ϰ�, ���� ������ ���� Ÿ�� �Լ�
    /// </summary>
    private void CheckWall()
	{
        Vector3 startPos = transform.position - transform.up * 0.5f + Vector3.up * 0.5f;
        //���� �� Ȯ��
        RaycastHit2D hitRight = Physics2D.Raycast(startPos, Vector2.right, 2f, groundLayer);
        //���� �� Ȯ��
        RaycastHit2D hitLeft = Physics2D.Raycast(startPos, -Vector2.right, 2f, groundLayer);

        Debug.DrawRay(startPos, Vector2.right * 2f, Color.red);
        Debug.DrawRay(startPos, -Vector2.right * 2f, Color.red);

        if (currentAction == MOVEMENT_STATE.WALL) //���� ���°� WALL�� �ƴ� ����
		{
            if (hitRight.collider == null && hitLeft.collider == null)
                ChangeAction(MOVEMENT_STATE.ONAIR);
            return;
		}

        wallDirection = 0;

        // ������ ���� ���� ��
        float angle = Vector2.SignedAngle(Vector2.up, hitRight.normal);
        if (hitRight.collider != null && hitRight.distance <= 0.52f && Mathf.Abs(angle) == 90 && rigid.velocity.x >= 0)
		{
            int wallDir = hitRight.normal.x > 0 ? 1 : -1;
            StartWallSlide(hitRight.point + hitRight.normal * 0.5f, wallDir);
        }

        // ������ ���� ���� ��
        angle = Vector2.SignedAngle(Vector2.up, hitLeft.normal);
        if (hitLeft.collider != null && hitLeft.distance <= 0.52f && Mathf.Abs(angle) == 90 && rigid.velocity.x <= 0)
		{
            int wallDir = hitLeft.normal.x > 0 ? 1 : -1;
            StartWallSlide(hitLeft.point + hitLeft.normal * 0.5f, wallDir);
        }
    }

    /// <summary>
    /// ���� �پ��ִ� ���·� ��ȯ
    /// </summary>
    /// <param name="position">�̵��� ��ġ</param>
    /// <param name="direction">���� �ִ� ������ �ݴ� ����</param>
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

    // ���°� ����� �� �۵��Ǵ� �Լ�
    private void OnStateChange()
	{
        if (prevAction != currentAction)
		{
            // ��� ����
            // if (prevState == MOVEMENT_STATE.IDLE)    //���� ���°� idle�� ���
            // if (currentState == MOVEMENT_STATE.IDLE) //���� ���°� idle�� ���

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
