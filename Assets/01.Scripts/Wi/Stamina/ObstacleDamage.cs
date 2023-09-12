using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleDamage : MonoBehaviour
{
	[SerializeField] private float knockbackPower;
	[SerializeField] private float damageCooltime;
	private float lastDamageTime;

	[SerializeField] private float camShakeAmplitude = 10f;
	[SerializeField] private float camShakeDuration = 1f;

	[SerializeField] private float damage = 10f;
	
	private bool canDamage = true;

	private Stamina stamina;
	private Movement movement;
	private Rigidbody2D rigid;

	private void Awake()
	{
		stamina = GetComponent<Stamina>();
		movement = GetComponent<Movement>();
		rigid = GetComponent<Rigidbody2D>();
	}

	private void Update()
	{
		if (Time.time >= lastDamageTime + damageCooltime)
		{
			canDamage = true;
		}
	}

	public void Knockback(Vector2 dir, float power)
	{
		rigid.AddForce(dir * power, ForceMode2D.Impulse);
	}

	private void OnCollisionStay2D(Collision2D collision)
	{
		if (canDamage && collision.transform.CompareTag("Obstacle"))
		{
			canDamage = false;
			lastDamageTime = Time.time;
			movement.StopMove();
			Knockback(collision.GetContact(0).normal, knockbackPower);
			CameraManager.Instance?.CameraShake(camShakeAmplitude, camShakeDuration);
			stamina.ReduceStamina(damage);
		}
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		if (canDamage && collision.transform.CompareTag("Obstacle"))
		{
			canDamage = false;
			lastDamageTime = Time.time;
			movement.StopMove();
			Vector2 dir = (Vector2)transform.position - collision.ClosestPoint(transform.position);
			if (dir == Vector2.zero)
				dir = (transform.position - collision.transform.position).normalized;
			Knockback(dir, knockbackPower);
			CameraManager.Instance?.CameraShake(camShakeAmplitude, camShakeDuration);
			stamina.ReduceStamina(damage);
		}
	}
}
