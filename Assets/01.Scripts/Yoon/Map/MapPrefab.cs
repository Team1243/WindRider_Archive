using UnityEngine;

public class MapPrefab : PoolableMono
{
    [SerializeField] private BoxCollider2D startObjCol;
    [SerializeField] private BoxCollider2D endObjCol;
 
    public override void Init()
    {
        startObjCol.enabled = true;
        endObjCol.enabled = true;
    }

    public void PopFunc()
    {
        PoolManager.Instance.Push(this);
    }
}
