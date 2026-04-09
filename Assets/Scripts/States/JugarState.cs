using UnityEngine;

public class JugarState : State
{
    public JugarState(FSMManager fsm) : base(fsm) { }

    public override void Enter()
    {
        Debug.Log("Estado: JUGAR");
        steering.currentBehavior = Vehicle.SteeringBehaviorType.Wander;
    }

    public override void Execute()
    {
        fsm.hambre += Time.deltaTime * 5f;
        fsm.energia -= Time.deltaTime * 3f;
        fsm.necesidadWC += Time.deltaTime * 4f;

        if (fsm.jugueteDetectado)
            fsm.ChangeState(new SeguirJugueteState(fsm));
        else if (fsm.hambre > 80f)
            fsm.ChangeState(new ComerState(fsm));
        else if (fsm.energia < 20f)
            fsm.ChangeState(new DormirState(fsm));
        else if (fsm.necesidadWC > 70f)
            fsm.ChangeState(new WCState(fsm));
    }

    public override void Exit() { }
}