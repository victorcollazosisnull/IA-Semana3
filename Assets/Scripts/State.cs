using UnityEngine;

public abstract class State
{
    protected FSMManager fsm;
    protected NavMeshExample nav;
    protected AIEye eye;

    public State(FSMManager fsm)
    {
        this.fsm = fsm;
        this.nav = fsm.GetComponent<NavMeshExample>();
        this.eye = fsm.GetComponent<AIEye>();
    }

    public abstract void Enter();
    public abstract void Execute();
    public abstract void Exit();
}