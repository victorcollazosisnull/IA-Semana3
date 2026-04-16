using UnityEngine;

public class JugarState : State
{
    float timer;

    public JugarState(FSMManager fsm) : base(fsm) { }

    public override void Enter()
    {
        Debug.Log("Estado: JUGAR");
        timer = 0f;
    }

    public override void Execute()
    {
        timer -= Time.deltaTime;

        // Movimiento aleatorio en el patio
        if (timer <= 0f)
        {
            Vector3 randomPoint = fsm.patio.position + Random.insideUnitSphere * 5f;
            nav.MoveToTargetPosition(randomPoint);
            timer = 3f;
        }

        // Necesidades
        fsm.hambre += Time.deltaTime * 5f;
        fsm.energia -= Time.deltaTime * 3f;
        fsm.necesidadWC += Time.deltaTime * 4f;

        // Sensor
        if (eye.ViewPlayer != null)
        {
            fsm.juguete = eye.ViewPlayer;
            fsm.ChangeState(new SeguirJugueteState(fsm));
        }
        else if (fsm.hambre > 80f)
            fsm.ChangeState(new ComerState(fsm));
        else if (fsm.energia < 20f)
            fsm.ChangeState(new DormirState(fsm));
        else if (fsm.necesidadWC > 70f)
            fsm.ChangeState(new WCState(fsm));
    }

    public override void Exit() { }
}