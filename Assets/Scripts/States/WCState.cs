using UnityEngine;

public class WCState : State
{
    public WCState(FSMManager fsm) : base(fsm) { }

    public override void Enter()
    {
        Debug.Log("Estado: WC");
        //steering.target = fsm.wc;
        //steering.currentBehavior = Vehicle.SteeringBehaviorType.Arrive;
    }

    public override void Execute()
    {
        nav.MoveToTargetPosition(fsm.wc.position);

        if (Vector3.Distance(fsm.transform.position, fsm.wc.position) < 1.5f)
        {
            fsm.necesidadWC -= Time.deltaTime * 40f;

            if (fsm.necesidadWC <= 5f)
                fsm.ChangeState(new JugarState(fsm));
        }
    }

    public override void Exit() { }
}