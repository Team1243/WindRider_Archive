using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementPlatform : MonoBehaviour
{
	private Movement movement;

	private void Awake()
	{
		movement = GetComponent<Movement>();

		movement.OnGround += OnGround;
		movement.OnAir += OnAir;
	}

	public void OnGround(RaycastHit2D info)
	{
		if (info.transform.GetComponent<ObstacleMovement>() != null)
		{
			transform.SetParent(info.transform);
		}
	}

	public void OnAir()
	{
		transform.SetParent(null);
	}
}
