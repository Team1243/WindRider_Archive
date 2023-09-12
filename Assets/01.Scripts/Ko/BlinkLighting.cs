using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BlinkLighting : MonoBehaviour
{
    [SerializeField] private float Mintime = 0.1f, Maxtime = 0.5f;
    [SerializeField] private Light2D[] _lights;
    private float timer;
    private void Start() => timer = Random.Range(Mintime, Maxtime);

    private void Update()
    {
        if (timer > 0)
            timer -= Time.deltaTime;
        else
        {
            foreach (Light2D lights in _lights)
            {
                lights.enabled = !lights.enabled;
            }
            timer = Random.Range(Mintime, Maxtime);
        }
    }
}
