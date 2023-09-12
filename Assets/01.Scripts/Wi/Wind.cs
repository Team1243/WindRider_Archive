using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour
{
	[SerializeField]
	private float windForce;
	[SerializeField]
	private float maxForce = 10;

	public Vector2 maxForceVec;
	public Vector2 force;

	private Vector2 velocity;

	private Movement movement;

	private void Start()
	{
		maxForceVec = transform.up * maxForce;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			movement = collision.GetComponent<Movement>();
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		movement = null;
	}

	private void FixedUpdate()
	{
		if (movement != null)
		{
			if (movement.CurrentMode != MOVEMENT_MODE.LIGHT) return;

			force = Vector2.zero;

			velocity = movement.AttachedRigid.velocity;
			
			// x√‡ »˚ ∞ËªÍ
			if (velocity.x * transform.up.x < 0 || Mathf.Abs(velocity.x) < Mathf.Abs(maxForceVec.x))
			{
				force.x = transform.up.x * windForce;
			}
			// y√‡ »˚ ∞ËªÍ
			if (velocity.y * transform.up.y < 0 || Mathf.Abs(velocity.y) < Mathf.Abs(maxForceVec.y))
			{
				force.y = transform.up.y * windForce;
			}

			// ¿ß∑Œ ∂ÁøÏ¥¬ »˚¿Ã ¿÷¿ª ∞ÊøÏ ∂ÁøÏ±‚
			if (movement.Grounded && force.y > 0)
			{
				StartCoroutine(movement.GroundDelay(0.1f));
			}

			movement.AttachedRigid.AddForce(force, ForceMode2D.Impulse);
		}
	}
}
