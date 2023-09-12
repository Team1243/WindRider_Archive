using UnityEngine;
using UnityEngine.Events;

public class ObstacleActTrigger : MonoBehaviour
{
    public UnityEvent OnTriggetEvent;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("ObstacleActTrigger : Player Enter");
            OnTriggetEvent?.Invoke();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawCube(transform.position, new Vector2(0.4f, 0.4f));
    }
}
