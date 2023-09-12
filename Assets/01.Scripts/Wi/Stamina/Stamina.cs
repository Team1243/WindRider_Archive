using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Stamina : MonoBehaviour
{
    [Header("Setting Values")]
    [SerializeField] private float maxStamina;
    [SerializeField] private float currentStamina;

	private InGameUI inGameUI;

	public UnityEvent OnDie = null;

	private void Awake()
	{
		inGameUI = FindObjectOfType<InGameUI>();
	}

	private void Start()
	{
		HealStamina(maxStamina);
	}

	public void HealStamina(float value)
	{
		ChangeStamina(value);
	}

	public void ReduceStamina(float value)
	{
		ChangeStamina(-value);
	}

	public void ChangeStamina(float value)
	{
		currentStamina = Mathf.Clamp(currentStamina + value, 0, maxStamina);

		inGameUI?.ChangeStaminaValue(currentStamina / maxStamina * 100f);

		if (currentStamina <= 0)
		{
			sceneManager.instance?.ReloadSceen();
			OnDie?.Invoke();
		}
	}
}
