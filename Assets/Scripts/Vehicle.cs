using UnityEngine;

public class Vehicle : MonoBehaviour
{
    // ─────────────────────────────────────────────
    //  Enum de Steering Behaviors
    // ─────────────────────────────────────────────
    public enum SteeringBehaviorType
    {
        Seek = 0,
        Flee = 1,
        Evade = 2,
        Pursuit = 3,
        Arrive = 4,
        Wander = 5,
        ObstacleAv = 6,
        WallAvoidance = 7,
        Interpose = 8,
        Hide = 9,
        PathFollowing = 10,
        OffsetPursuit = 11,
    }

    [Header("Steering Behavior activo")]
    public SteeringBehaviorType currentBehavior = SteeringBehaviorType.Seek;

    [Header("Parámetros del vehículo")]
    public float maxSpeed = 5f;
    public float maxForce = 10f;
    public float mass = 1f;

    [Header("Targets / Referencias")]
    public Transform target;
    public Transform secondTarget;
    public Transform[] path;
    public LayerMask obstacleLayer;

    private Vector3 velocity;
    private int pathIndex;
    private Vector3 wanderTarget;

    void Update()
    {
        HandleKeyboardInput();
        ExecuteSteeringBehavior();

        Vector3 pos = transform.position;
        pos.y = 1f;
        transform.position = pos;
    }

    void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0)) currentBehavior = SteeringBehaviorType.Seek;
        else if (Input.GetKeyDown(KeyCode.Alpha1)) currentBehavior = SteeringBehaviorType.Flee;
        else if (Input.GetKeyDown(KeyCode.Alpha2)) currentBehavior = SteeringBehaviorType.Evade;
        else if (Input.GetKeyDown(KeyCode.Alpha3)) currentBehavior = SteeringBehaviorType.Pursuit;
        else if (Input.GetKeyDown(KeyCode.Alpha4)) currentBehavior = SteeringBehaviorType.Arrive;
        else if (Input.GetKeyDown(KeyCode.Alpha5)) currentBehavior = SteeringBehaviorType.Wander;
        else if (Input.GetKeyDown(KeyCode.Alpha6)) currentBehavior = SteeringBehaviorType.ObstacleAv;
        else if (Input.GetKeyDown(KeyCode.Alpha7)) currentBehavior = SteeringBehaviorType.WallAvoidance;
        else if (Input.GetKeyDown(KeyCode.Alpha8)) currentBehavior = SteeringBehaviorType.Interpose;
        else if (Input.GetKeyDown(KeyCode.Alpha9)) currentBehavior = SteeringBehaviorType.Hide;
    }

    void ExecuteSteeringBehavior()
    {
        Vector3 steeringForce = Vector3.zero;

        switch (currentBehavior)
        {
            case SteeringBehaviorType.Seek:
                steeringForce = Seek(target.position);
                break;

            case SteeringBehaviorType.Flee:
                steeringForce = Flee(target.position);
                break;

            case SteeringBehaviorType.Evade:
                steeringForce = Evade(target);
                break;

            case SteeringBehaviorType.Pursuit:
                steeringForce = Pursuit(target);
                break;

            case SteeringBehaviorType.Arrive:
                steeringForce = Arrive(target.position);
                break;

            case SteeringBehaviorType.Wander:
                steeringForce = Wander();
                break;

            case SteeringBehaviorType.ObstacleAv:
                steeringForce = ObstacleAvoidance();
                break;

            case SteeringBehaviorType.WallAvoidance:
                steeringForce = WallAvoidance();
                break;

            case SteeringBehaviorType.Interpose:
                steeringForce = Interpose(target, secondTarget);
                break;

            case SteeringBehaviorType.Hide:
                steeringForce = Hide(target);
                break;

            case SteeringBehaviorType.PathFollowing:
                steeringForce = PathFollowing(path);
                break;

            case SteeringBehaviorType.OffsetPursuit:
                steeringForce = OffsetPursuit(secondTarget);
                break;
        }

        ApplyForce(steeringForce);
    }

    void ApplyForce(Vector3 force)
    {
        Vector3 acceleration = Vector3.ClampMagnitude(force, maxForce) / mass;
        velocity = Vector3.ClampMagnitude(velocity + acceleration * Time.deltaTime, maxSpeed);
        transform.position += velocity * Time.deltaTime;

        if (velocity.sqrMagnitude > 0.01f)
        {
            Vector3 dir = velocity.normalized;
            dir.y = 0f;
            transform.forward = dir;
        }
    }

    // ─────────────────────────────────────────────
    // Steering Behaviors
    // ─────────────────────────────────────────────

    Vector3 Seek(Vector3 targetPosition)
    {
        Vector3 desired = (targetPosition - transform.position).normalized * maxSpeed;
        return desired - velocity;
    }

    Vector3 Flee(Vector3 targetPosition)
    {
        Vector3 desired = (transform.position - targetPosition).normalized * maxSpeed;
        return desired - velocity;
    }

    Vector3 Pursuit(Transform prey)
    {
        Vector3 futurePos = prey.position + prey.GetComponent<Rigidbody>().linearVelocity;
        return Seek(futurePos);
    }

    Vector3 Evade(Transform prey)
    {
        Vector3 futurePos = prey.position + prey.GetComponent<Rigidbody>().linearVelocity;
        return Flee(futurePos);
    }

    Vector3 Arrive(Vector3 targetPosition)
    {
        float slowingRadius = 3f;

        Vector3 toTarget = targetPosition - transform.position;
        float distance = toTarget.magnitude;

        if (distance < 0.1f) return Vector3.zero;

        float speed = maxSpeed * (distance / slowingRadius);
        speed = Mathf.Min(speed, maxSpeed);

        Vector3 desired = toTarget * speed / distance;
        return desired - velocity;
    }

    Vector3 Wander()
    {
        wanderTarget += new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
        wanderTarget.Normalize();
        wanderTarget *= 2f;

        Vector3 targetLocal = wanderTarget + Vector3.forward * 3f;
        Vector3 targetWorld = transform.TransformPoint(targetLocal);

        return Seek(targetWorld);
    }

    Vector3 ObstacleAvoidance()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 3f, obstacleLayer))
        {
            return hit.normal * maxForce;
        }
        return Vector3.zero;
    }

    Vector3 WallAvoidance()
    {
        RaycastHit hit;
        Vector3 left = Quaternion.AngleAxis(-30, transform.up) * transform.forward;
        Vector3 right = Quaternion.AngleAxis(30, transform.up) * transform.forward;

        if (Physics.Raycast(transform.position, transform.forward, out hit, 3f, obstacleLayer))
            return hit.normal * maxForce;

        if (Physics.Raycast(transform.position, left, out hit, 3f, obstacleLayer))
            return hit.normal * maxForce;

        if (Physics.Raycast(transform.position, right, out hit, 3f, obstacleLayer))
            return hit.normal * maxForce;

        return Vector3.zero;
    }

    Vector3 Interpose(Transform agentA, Transform agentB)
    {
        Vector3 midPoint = (agentA.position + agentB.position) * 0.5f;
        return Seek(midPoint);
    }

    Vector3 Hide(Transform threat)
    {
        Vector3 hideDir = (transform.position - threat.position).normalized;
        Vector3 hidePos = transform.position + hideDir * 5f;
        return Seek(hidePos);
    }

    Vector3 PathFollowing(Transform[] waypoints)
    {
        if (waypoints.Length == 0) return Vector3.zero;

        Vector3 force = Seek(waypoints[pathIndex].position);

        if (Vector3.Distance(transform.position, waypoints[pathIndex].position) < 1f)
            pathIndex = (pathIndex + 1) % waypoints.Length;

        return force;
    }

    Vector3 OffsetPursuit(Transform leader)
    {
        Vector3 offset = leader.right * 2f;
        return Seek(leader.position + offset);
    }
}