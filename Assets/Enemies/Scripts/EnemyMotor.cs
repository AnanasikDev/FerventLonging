using NaughtyAttributes;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMotor : MonoBehaviour
{
    [Required] public NavMeshAgent agent;
    public Transform player { get; private set; }
    [SerializeField] private float minDistanceToHeater;
    [SerializeField] private float maxFollowDistance;

    [HideInInspector] public Vector2 originalPosition;

    [SerializeField][ReadOnly] private bool isDestinationSet = false;

    [ShowNativeProperty] public bool isOnNavmesh { get { return agent && agent.isOnNavMesh; } }
    [ShowNativeProperty] public bool isAgentActive { get { return isOnNavmesh && agent.hasPath && agent.pathStatus == NavMeshPathStatus.PathComplete; } }

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

        if (!isOnNavmesh) return;

        float distanceToHeater = (transform.position - Scripts.Heater.transform.position).magnitude;
        float playerDistanceToHeater = (Scripts.Player.transform.position - Scripts.Heater.transform.position).magnitude;

        if ((transform.position - player.position).magnitude < maxFollowDistance && distanceToHeater >= minDistanceToHeater && playerDistanceToHeater >= minDistanceToHeater)
        {
            agent.SetDestination(player.position);
            isDestinationSet = true;
            return;
        }
        if (isDestinationSet == true || distanceToHeater < minDistanceToHeater || playerDistanceToHeater < minDistanceToHeater)
        {
            agent.SetDestination(originalPosition);
            isDestinationSet = false;
            return;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxFollowDistance);
    }
}
