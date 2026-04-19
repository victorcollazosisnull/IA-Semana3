using UnityEngine;

public class DormirState : State
{
    public DormirState(FSMManager fsm) : base(fsm) { }

    public override void Enter()
    {
        Debug.Log("Estado: DORMIR");
        //steering.target = fsm.cama;
        //steering.currentBehavior = Vehicle.SteeringBehaviorType.Arrive;
    }

    public override void Execute()
    {
        nav.MoveTo(fsm.cama);

        if (Vector3.Distance(fsm.transform.position, fsm.cama.position) < 1.5f)
        {
            fsm.energia += Time.deltaTime * 25f;

            if (fsm.energia >= 90f)
                fsm.ChangeState(new JugarState(fsm));
        }
    }

    public override void Exit() { }
}