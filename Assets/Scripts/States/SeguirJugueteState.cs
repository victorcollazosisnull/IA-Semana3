using UnityEngine;

public class SeguirJugueteState : State
{
    public SeguirJugueteState(FSMManager fsm) : base(fsm) { }

    public override void Enter()
    {
        Debug.Log("Estado: SEGUIR JUGUETE");
        steering.target = fsm.juguete;
        steering.currentBehavior = Vehicle.SteeringBehaviorType.Seek;
    }

    public override void Execute()
    {
        if (fsm.juguete != null &&
        Vector3.Distance(fsm.transform.position, fsm.juguete.position) < 1.5f)
        {
            if (fsm.juguete.CompareTag("Toy"))
            {
                GameObject.Destroy(fsm.juguete.gameObject);
            }

            fsm.juguete = null;
            fsm.ChangeState(new JugarState(fsm));
        }
    }

    public override void Exit() { }
}