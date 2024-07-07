using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMotor : MonoBehaviour
{
    [SerializeField][ReadOnly] private NavMeshAgent agent;
    [ShowNativeProperty] public Transform player { get; private set; }
    [SerializeField] private float maxFollowDistance;

    [HideInInspector] public Vector2 originalPosition;

    public void Init()
    {
        player = Scripts.Player;
        agent = GetComponent<NavMeshAgent>();
        originalPosition = transform.position;
    }

    public void UpdateMotor()
    {
        if ((transform.position - player.position).magnitude < maxFollowDistance)
        {
            agent.SetDestination(player.position);
        }
        else
        {
            agent.SetDestination(originalPosition);
        }
    }
}
