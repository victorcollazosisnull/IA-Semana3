using UnityEngine;
using UnityEngine.AI;

public class NavMeshExample : MonoBehaviour
{
    public NavMeshAgent agent;

    void Start()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();
    }

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

    public void MoveToTargetPosition(Vector3 targetPos)
    {
        Vector3 finalPos;

        if (SamplePosition(targetPos, 10f, out finalPos))
        {
            agent.SetDestination(finalPos);
        }
    }

    public NavMeshPath CalculatePath(Vector3 targetPos)
    {
        NavMeshPath path = new NavMeshPath();
        agent.CalculatePath(targetPos, path);
        return path;
    }

    public Vector3 FindClosestEdge(Vector3 position)
    {
        NavMeshHit hit;

        if (NavMesh.FindClosestEdge(position, out hit, NavMesh.AllAreas))
        {
            return hit.position;
        }

        return position;
    }

    public bool Raycast(Vector3 start, Vector3 end)
    {
        NavMeshHit hit;
        return NavMesh.Raycast(start, end, out hit, NavMesh.AllAreas);
    }
}