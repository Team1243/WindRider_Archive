using UnityEngine;

public class MapEvent : MonoBehaviour
{
    PoolableMono thisPrefab;
    BoxCollider2D _boxCollider2D; // �̰ɷ� ������ �Ÿ� MapPrefab���� init�Լ��� �ʱ�ȭ

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
