using UnityEngine;

public class FSMManager : MonoBehaviour
{
    State currentState;

    [Header("Necesidades")]
    public float hambre = 0;
    public float energia = 100;
    public float necesidadWC = 0;

    [Header("Sensores")]
    public bool jugueteDetectado = false;

    [Header("Targets")]
    public Transform cama;
    public Transform comida;
    public Transform wc;
    public Transform juguete;

    void Start()
    {
        currentState = new JugarState(this);
        currentState.Enter();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
            jugueteDetectado = true;

        currentState.Execute();
    }

    public void ChangeState(State newState)
    {
        currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }
}