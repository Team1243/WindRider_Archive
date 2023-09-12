using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ObstacleGravity : MonoBehaviour
{
    private Rigidbody2D _rigid;

    [SerializeField] private float gavityPower = 1.0f;

    [SerializeField] private float camShakePower = 100;
    [SerializeField] private float camShakeTime = 0.5f;

    private AudioSource _audioSource;

    private void Awake()
    {
        _rigid = GetComponent<Rigidbody2D>();
        _audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        _rigid.isKinematic = true;
    }

    public void ActiviateGravity()
    {
        _rigid.isKinematic = false;
        _rigid.gravityScale = gavityPower;

        _audioSource.Play();
        CameraManager.Instance.CameraShake(camShakePower, camShakeTime);
    }
}
