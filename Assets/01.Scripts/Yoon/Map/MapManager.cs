using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance = null;

    private int stageCnt = 0;
    private bool[] isVisited = new bool[20];

    private PoolableMono nextStage = null;
    private PoolableMono beforeStage = null;
    public PoolableMono BeforeStage => beforeStage;

    [SerializeField] private Transform startIstanPos;

    private bool isStarted = false;

    private void Awake()
    {
        if (Instance != null)
            Debug.LogError("Multiple GameManager is running");
        else
            Instance = this;
    }

    private void Start()
    {
        stageCnt = GameManager.Instance.PoolingListCnt;

        // ????? ??? ?????????? ???????? ??????? ???
        int index = UnityEngine.Random.Range(1, stageCnt + 1);
        nextStage = PoolManager.Instance.Pop($"Prefab {index}") as MapPrefab;
        nextStage.transform.position = startIstanPos.position;
        isVisited[index] = true;
    }

    public void CreateNewStage(Transform targetPos)
    {
        int index = 0;

        // ?ьм?? ?Ѕх????? ?????? ???? ?????? ???? ????
        while (true)
        {
            index = UnityEngine.Random.Range(1, stageCnt + 1);
        
            if (isVisited[index]) continue;
            else break;
        }

        // ?ьм ??
        isVisited[index] = true;

        // ???? ?ьм?? ????? ??, ???? ?ьм?? ???? ???? ??ьм???? 
        if (IsRoundAllStage())
        {
            for (int i = 1; i <= stageCnt; i++)
            {
                isVisited[i] = false;
            }
        }
        
        nextStage = PoolManager.Instance.Pop($"Prefab {index}") as MapPrefab;
        nextStage.transform.position = targetPos.position;

        // ???? ???? ??? ????
        isStarted = true;
    }

    public void DestroyNowStage(PoolableMono nowStage)
    {
        if (beforeStage != null && isStarted)
        {
            PoolManager.Instance.Push(beforeStage);
            // MapPrefab target = nowStage.GetComponent<MapPrefab>();
            // target.PopFunc();
        }
        beforeStage = nowStage;
    }

    // stage?? ???? ?? ???? ?ЁР??? ?????
    private bool IsRoundAllStage()
    {
        for (int i = 1; i <= stageCnt; i++)
        {
            if (!isVisited[i])
            {
                return false;
            }
        }
        return true;    
    }
}
