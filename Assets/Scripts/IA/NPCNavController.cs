using UnityEngine;

public class NPCNavController : MonoBehaviour
{
    public NavMeshExample navMesh;
    public Transform patio;

    float timer;
    public float intervalo = 3f;

    void Start()
    {
        if (navMesh == null)
            navMesh = GetComponent<NavMeshExample>();
    }

    public void WanderPatio()
    {
        if (navMesh == null) return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            timer = intervalo;

            Vector3 random = patio.position + Random.insideUnitSphere * 10f;

            Vector3 finalPos;
            if (navMesh.SamplePosition(random, 10f, out finalPos))
            {
                navMesh.MoveToTargetPosition(finalPos);
            }
        }
    }

    public void MoveTo(Transform target)
    {
        navMesh.MoveToTargetPosition(target.position);
    }
}