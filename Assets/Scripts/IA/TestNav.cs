using UnityEngine;
using UnityEngine.AI;

public class TestNav : MonoBehaviour
{
    public NavMeshAgent agent;

    void Start()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        agent.SetDestination(new Vector3(5, 0, 5));
    }
}