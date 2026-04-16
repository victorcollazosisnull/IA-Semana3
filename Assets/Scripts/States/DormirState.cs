using UnityEngine;

public class DormirState : State
{
    public DormirState(FSMManager fsm) : base(fsm) { }

    public override void Enter()
    {
        Debug.Log("Estado: DORMIR");
    }

    public override void Execute()
    {
        nav.MoveToTargetPosition(fsm.cama.position);

        if (Vector3.Distance(fsm.transform.position, fsm.cama.position) < 2f)
        {
            fsm.energia += Time.deltaTime * 25f;

            if (fsm.energia >= 90f)
                fsm.ChangeState(new JugarState(fsm));
        }
    }

    public override void Exit() { }
}