using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;

public class spawner : MonoBehaviour
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
        GameObject _player = GameObject.Find("Player");
        _player.transform.position = transform.position - new Vector3(5, 0);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<Movement>())
        {
            other.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.right, ForceMode2D.Force);
            _audio.SimplePlay("break");
            _sprite.enabled = false;
            StartCoroutine(Break());
            _col.enabled = false;
        }
    }

    private IEnumerator Break()
    {
        _particle.Play();
        yield return new WaitForSeconds(_particle.main.startLifetime.constantMax);
        Destroy(gameObject);
    }
}
