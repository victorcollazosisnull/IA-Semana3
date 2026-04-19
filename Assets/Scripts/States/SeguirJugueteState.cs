using UnityEngine;

public class SeguirJugueteState : State
{
    public SeguirJugueteState(FSMManager fsm) : base(fsm) { }

    public override void Enter()
    {
        Debug.Log("Estado: SEGUIR JUGUETE");
        //steering.target = fsm.juguete;
        //steering.currentBehavior = Vehicle.SteeringBehaviorType.Seek;
    }

    public override void Execute()
    {
        if (fsm.juguete != null)
        {
            nav.MoveTo(fsm.juguete);

            if (Vector3.Distance(fsm.transform.position, fsm.juguete.position) < 1.5f)
            {
                Debug.Log("Llegó al juguete");
                fsm.ChangeState(new JugarState(fsm));
            }
        }
        else
        {
            fsm.ChangeState(new JugarState(fsm));
        }
    }

    public override void Exit() { }
}