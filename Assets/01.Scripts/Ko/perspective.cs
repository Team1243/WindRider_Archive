using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class perspective : MonoBehaviour
{
    [Range(0, 1)][SerializeField] private float _offset = 0.01f;
    private Vector3 origin;
    private Transform _player;
    void Start()
    {
        _player = GameObject.Find("Player").transform;
        origin = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(origin.x + (_offset * _player.transform.position.x), origin.y + (_offset * _player.transform.position.y), origin.z);
    }
}
