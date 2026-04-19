using UnityEngine;

public class ComerState : State
{
    public ComerState(FSMManager fsm) : base(fsm) { }

    public override void Enter()
    {
        Debug.Log("Estado: COMER");
        //steering.target = fsm.comida;
        //steering.currentBehavior = Vehicle.SteeringBehaviorType.Arrive;
    }

    public override void Execute()
    {
        nav.MoveTo(fsm.comida);

        if (Vector3.Distance(fsm.transform.position, fsm.comida.position) < 1.5f)
        {
            fsm.hambre -= Time.deltaTime * 30f;

            if (fsm.hambre <= 10f)
                fsm.ChangeState(new JugarState(fsm));
        }
    }

    public override void Exit() { }
}