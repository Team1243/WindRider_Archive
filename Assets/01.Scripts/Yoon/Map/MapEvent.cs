using UnityEngine;

public class MapEvent : MonoBehaviour
{
    PoolableMono thisPrefab;
    BoxCollider2D _boxCollider2D; // 이걸로 제한할 거면 MapPrefab에서 init함수에 초기화

    private void Awake()
    {
        thisPrefab = GetComponentInParent<PoolableMono>();
        _boxCollider2D = GetComponent<BoxCollider2D>();
    }

    public void CreateStage(Transform pos)
    {
        _boxCollider2D.enabled = false;
        MapManager.Instance.CreateNewStage(pos);
    }

    public void DestroyStage()
    {
        MapManager.Instance.DestroyNowStage(thisPrefab);
        _boxCollider2D.enabled = false;
    }
}
