using System.Collections;
using UnityEngine;

public class ObjectTelepoteMove : MonoBehaviour
{
    private Transform startPos; // tr : child 0
    private Transform endPos;   // tr : child 1

    private Transform targetTr;
    private bool isCanMove = true;

    [SerializeField] private float camShakePower = 100;
    [SerializeField] private float camShakeTime = 0.5f;

    [SerializeField] private ParticleSystem moveEffect;

    private AudioSource _audioSource;

    private void Awake()
    {
        startPos = transform.GetChild(0);
        endPos = transform.GetChild(1);
        _audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        targetTr = GameManager.Instance.PlayerObj.transform;
    }

    public void TelepoteMove(string whichSide)
    {
        StartCoroutine(Col_TelepoteMove(whichSide));
    }

    private IEnumerator Col_TelepoteMove(string WhichSide)
    {
        targetTr.transform.position = (WhichSide == "start") ? endPos.position: startPos.position;

        moveEffect.transform.position = targetTr.transform.position;
        moveEffect.Play();
        _audioSource.Play();

        CameraManager.Instance.CameraShake(camShakePower, camShakeTime);

        yield return new WaitForEndOfFrame();
    }
}
