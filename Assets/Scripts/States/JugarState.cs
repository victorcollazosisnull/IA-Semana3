using System;
using UnityEngine;

public class JugarState : State
{
    public JugarState(FSMManager fsm) : base(fsm) { }

    public override void Enter()
    {
        Debug.Log("Estado: JUGAR");

        //nav.MoveTo(fsm.transform);
    }

    public override void Execute()
    {
        nav.WanderPatio();

        fsm.hambre += Time.deltaTime * 5f;
        fsm.energia -= Time.deltaTime * 3f;
        fsm.necesidadWC += Time.deltaTime * 4f;

        if (fsm.puedeDetectarJuguete && _AIEye.ViewPlayer != null)
        {
            fsm.juguete = _AIEye.ViewPlayer;
            fsm.ChangeState(new SeguirJugueteState(fsm));
        }
        else if (fsm.hambre > 80f)
            fsm.ChangeState(new ComerState(fsm));
        else if (fsm.energia < 20f)
            fsm.ChangeState(new DormirState(fsm));
        else if (fsm.necesidadWC > 70f)
            fsm.ChangeState(new WCState(fsm));

        Debug.Log($"Estado: JUGAR - Hambre: {fsm.hambre}, Energía: {fsm.energia}, NecesidadWC: {fsm.necesidadWC}");
    }

    public override void Exit() { }
}