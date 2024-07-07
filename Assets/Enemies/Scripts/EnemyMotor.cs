using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMotor : MonoBehaviour
{
    [SerializeField][Required] private NavMeshAgent agent;
    public Transform player { get; private set; }
    [SerializeField] private float maxFollowDistance;

    [HideInInspector] public Vector2 originalPosition;

    [SerializeField][ReadOnly] private bool isDestinationSet = false;

    public void Init()
    {
        player = Scripts.Player.transform;
        originalPosition = transform.position;

        agent.enabled = false;
        agent.transform.position = transform.position;
        agent.transform.localPosition = Vector3.zero;
        agent.enabled = true;
    }

    public void UpdateMotor()
    {
        transform.position = agent.transform.position;
        agent.transform.localPosition = Vector3.zero;

        if (!agent.isOnNavMesh) return;

        if ((transform.position - player.position).magnitude < maxFollowDistance)
        {
            agent.SetDestination(player.position);
            isDestinationSet = true;
        }
        else if (isDestinationSet == true)
        {
            agent.SetDestination(originalPosition);
            isDestinationSet = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxFollowDistance);
    }
}
