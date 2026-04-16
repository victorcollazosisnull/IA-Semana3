using UnityEngine;
using UnityEngine.AI;

public class NavMeshExample : MonoBehaviour
{
    public NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // 1. SamplePosition
    public bool SamplePosition(Vector3 center, float range, out Vector3 result)
    {
        NavMeshHit hit;

        if (NavMesh.SamplePosition(center, out hit, range, NavMesh.AllAreas))
        {
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }

    // 2. MoveToTargetPosition
    public void MoveToTargetPosition(Vector3 targetPos)
    {
        Vector3 finalPos;

        if (SamplePosition(targetPos, 2f, out finalPos))
        {
            agent.SetDestination(finalPos);
        }
    }

    // 3. CalculatePath
    public void CalculatePath(Vector3 targetPos)
    {
        NavMeshPath path = new NavMeshPath();

        if (agent.CalculatePath(targetPos, path))
        {
            agent.SetPath(path);
        }
    }

    // 4. FindClosestEdge
    public Vector3 FindClosestEdge(Vector3 position)
    {
        NavMeshHit hit;

        if (NavMesh.FindClosestEdge(position, out hit, NavMesh.AllAreas))
        {
            return hit.position;
        }

        return position;
    }

    // 5. Raycast
    public bool NavMeshRaycast(Vector3 start, Vector3 end)
    {
        NavMeshHit hit;
        return NavMesh.Raycast(start, end, out hit, NavMesh.AllAreas);
    }
}