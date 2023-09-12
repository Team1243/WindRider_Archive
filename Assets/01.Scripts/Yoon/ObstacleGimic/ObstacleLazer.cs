using System.Collections;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ObstacleLazer : MonoBehaviour
{
    LineRenderer _lineRenderer;
    BoxCollider2D _laserCollider; 
    // �ڽ� ������Ʈ�� �ִ� �÷��̾� ������ �ݶ��̴� (�������� �ݶ��̴���� �����ϸ� ����)

    [SerializeField] private LayerMask groundLayer; // ���� ��ֹ� �Ǵܿ� Layer

    [Header("Option")]
    [SerializeField] private bool isAlwaysOn = false;
    // �������� ������ �ð��� �������� ���� ��� ��������

    [Header("Value")]
    [SerializeField] private float maxDistance = 10f; // ray�� �ִ� �߻� ����
    [SerializeField] private float delayTime = 1f;    // ������ �߻� �ֱ� ������ Ÿ��
    [SerializeField] private ParticleSystem laserEffect;

    private AudioSource _audioSource;

    private bool isParitlceOn = false; // ��ƼŬ�� ���� �ִ��� �ƴ���

    private Stamina playerStamina;
    [SerializeField] private float damage; // ������ ������

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _laserCollider = GetComponentInChildren<BoxCollider2D>();
        _audioSource = GetComponent<AudioSource>();

        _lineRenderer.positionCount = 2;
        _lineRenderer.enabled = false;

        _laserCollider.enabled = false;
    }

    private void Start()
    {
        if (isAlwaysOn) delayTime = 0;
        StartCoroutine(LaserRoop());
    }

    private IEnumerator LaserRoop()
    {
        while (true)
        {
            if (playerStamina != null)
			{
                playerStamina.ReduceStamina(damage * Time.deltaTime);
			}

            RaycastHit2D hit;
            hit = Physics2D.Raycast(transform.position, transform.up, maxDistance, groundLayer);

            if (hit)
            {
                ActivateLine(transform.position, hit.point); // ���� Ȱ��ȭ

                OnLaserCollider(hit.distance); // �ݶ��̴� ũ�� ����

                UpdatePariclePosition(hit.point); // ��ƼŬ ��ġ ����
                PlayParticle(); // ��ƼŬ ���
                _audioSource.Play();
            }
            else
            {
                Vector3 endoPos = transform.position + transform.up * maxDistance; 
                ActivateLine(transform.position, endoPos); // �ִ� �߻� ���̷� ���� Ȱ��ȭ

                OnLaserCollider(maxDistance); // �浹 �ݶ��̴� Ȱ��ȭ

                StopParicle(); // ��ƼŬ ����
                _audioSource.Stop();
            }

            yield return new WaitForSeconds(delayTime);

            if (!isAlwaysOn)
            {
                DeactivateLine();
                OffLaserCollider();
                StopParicle();
                _audioSource.Stop();
            }

            yield return new WaitForSeconds(delayTime);
        }
    }

    #region Line

    private void ActivateLine(Vector3 startPos, Vector3 endPos)
    {
        _lineRenderer.enabled = true;
        _lineRenderer.SetPosition(0, startPos);
        _lineRenderer.SetPosition(1, endPos);
    }

    private void DeactivateLine()
    {
        _lineRenderer.enabled = false;
    }

    #endregion

    #region Collider

    private void OnLaserCollider(float distance)
    {
        _laserCollider.enabled = true;

        Vector2 colliderSize = new Vector2(1, distance / 2);
        _laserCollider.size = colliderSize;

        Vector2 colliderOffset = new Vector2(0, colliderSize.y / 2);
        _laserCollider.offset = colliderOffset;
    }

    private void OffLaserCollider()
    {
        _laserCollider.enabled = false;
    }

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.CompareTag("Player"))
		{
            collision.gameObject.TryGetComponent(out playerStamina);
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.gameObject.CompareTag("Player"))
		{
            playerStamina = null;
		}
	}

	#endregion

	#region Particle

	private void PlayParticle()
    {
        if (isParitlceOn) return;

        isParitlceOn = true;
        laserEffect.Play();
    }

    private void UpdatePariclePosition(Vector3 pos)
    {
        laserEffect.transform.position = pos;
    }

    private void StopParicle()
    {
        laserEffect.Stop();
        isParitlceOn = false;
    }

    #endregion
}
