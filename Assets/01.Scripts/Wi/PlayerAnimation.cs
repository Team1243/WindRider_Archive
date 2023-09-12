using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField]
    private Animator animator;
	private SpriteRenderer rend;

    readonly int actionHash = Animator.StringToHash("action");
    readonly int isrunningHash = Animator.StringToHash("isrunning");
    readonly int isairHash = Animator.StringToHash("isair");
    readonly int iswallHash = Animator.StringToHash("iswall");
    readonly int isslideHash = Animator.StringToHash("isslide");

	private void Awake()
	{
		rend = transform.Find("Visual").GetComponent<SpriteRenderer>();
	}

	public void LookDirect(int direction)
	{
		if (direction == 0)
			return;

        if (direction < 0)
			rend.flipX = true;
		else
			rend.flipX = false;
	}

    public void ChangeAnimation(MOVEMENT_STATE state)
	{
		animator.SetTrigger(actionHash);
		animator.ResetTrigger(isrunningHash);
		animator.ResetTrigger(isairHash);
		animator.ResetTrigger(iswallHash);

		switch (state)
		{
			case MOVEMENT_STATE.IDLE:
				break;
			case MOVEMENT_STATE.RUNNING:
				animator.SetTrigger(isrunningHash);
				break;
			case MOVEMENT_STATE.DASHING:
				//animator.SetTrigger(isslideHash);
				break;
			case MOVEMENT_STATE.AIRDASHING:
				animator.SetTrigger(isairHash);
				break;
			case MOVEMENT_STATE.ONAIR:
				animator.SetTrigger(isairHash);
				break;
			case MOVEMENT_STATE.WALL:
				animator.SetTrigger(iswallHash);
				break;
			default:
				break;
		}
	}
}
