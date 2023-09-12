using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class WallBreaker : MonoBehaviour
{
    AudioPlayer _audio;
    BoxCollider2D _col;
    ParticleSystem _particle;
    SpriteRenderer _sprite;
    private void Awake()
    {
        _audio = GetComponent<AudioPlayer>();
        _col = GetComponent<BoxCollider2D>();
        _sprite = GetComponent<SpriteRenderer>();
        _particle = GetComponentInChildren<ParticleSystem>();
    }

    private IEnumerator Break()
    {
        _particle.Play();
        yield return new WaitForSeconds(_particle.main.startLifetime.constantMax);
        Destroy(gameObject);
    }

    // private void OnTriggerEnter2D(Collider2D col)
    // {
    //     if (col.gameObject.GetComponent<Movement>().CurrentMode == MOVEMENT_MODE.HEAVY)
    //     {
    //         _audio.SimplePlay("break");
    //         _sprite.enabled = false;
    //         _col.enabled = false;
    //         StartCoroutine(Break());
    //         Vector2 dir = col.GetComponent<Rigidbody2D>().velocity;
    //         //col.GetComponent<Rigidbody2D>().velocity = v2Rotate(-dir, 2 * getAngle(dir, (transform.position - col.transform.position)));
    //         _particle.transform.eulerAngles = v2Rotate(-dir, 2 * getAngle(dir, (transform.position - col.transform.position)));
    //     }
    // }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.name != "Player") return;
        if (col.gameObject.GetComponent<Movement>().CurrentMode == MOVEMENT_MODE.HEAVY)
        {
            GetComponent<Light2D>().enabled = false;
            _audio.SimplePlay("break");
            _sprite.enabled = false;
            _col.enabled = false;
            StartCoroutine(Break());
            //Vector2 dir = col.GetComponent<Rigidbody2D>().velocity;
            //col.GetComponent<Rigidbody2D>().velocity = v2Rotate(-dir, 2 * getAngle(dir, (transform.position - col.transform.position)));
            //_particle.transform.eulerAngles = v2Rotate(-dir, 2 * getAngle(dir, (transform.position - col.transform.position)));
        }
    }

    float getAngle(Vector2 vec1, Vector2 vec2)
    {
        float angle = (Mathf.Atan2(vec2.y, vec2.x) - Mathf.Atan2(vec1.y, vec1.x)) * Mathf.Rad2Deg;
        return angle;
    }

    Vector2 v2Rotate(Vector2 aPoint, float aDegree)
    {
        float rad = aDegree * Mathf.Deg2Rad;
        float s = Mathf.Sin(rad);
        float c = Mathf.Cos(rad);
        return new Vector2(
            aPoint.x * c - aPoint.y * s,
            aPoint.y * c + aPoint.x * s);
    }

}
