using System.Collections;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ObstacleLazer : MonoBehaviour
{
    LineRenderer _lineRenderer;
    BoxCollider2D _laserCollider; 
    // 자식 오브젝트에 있는 플레이어 판정용 콜라이더 (레이저의 콜라이더라고 생각하면 편함)

    [SerializeField] private LayerMask groundLayer; // 땅과 장애물 판단용 Layer

    [Header("Option")]
    [SerializeField] private bool isAlwaysOn = false;
    // 레이저가 꺼지는 시간이 존재하지 않음 계속 켜져있음

    [Header("Value")]
    [SerializeField] private float maxDistance = 10f; // ray의 최대 발사 길이
    [SerializeField] private float delayTime = 1f;    // 레이저 발사 주기 딜레이 타임
    [SerializeField] private ParticleSystem laserEffect;

    private AudioSource _audioSource;

    private bool isParitlceOn = false; // 파티클이 켜져 있는지 아닌지

    private Stamina playerStamina;
    [SerializeField] private float damage; // 레이저 데미지

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
                ActivateLine(transform.position, hit.point); // 라인 활성화

                OnLaserCollider(hit.distance); // 콜라이더 크기 조정

                UpdatePariclePosition(hit.point); // 파티클 위치 조정
                PlayParticle(); // 파티클 재생
                _audioSource.Play();
            }
            else
            {
                Vector3 endoPos = transform.position + transform.up * maxDistance; 
                ActivateLine(transform.position, endoPos); // 최대 발사 길이로 라인 활성화

                OnLaserCollider(maxDistance); // 충돌 콜라이더 활성화

                StopParicle(); // 파티클 정지
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
