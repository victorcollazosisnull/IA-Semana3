using UnityEngine;
using System.Collections;
public class FSMManager : MonoBehaviour
{
    State currentState;
    public bool puedeDetectarJuguete = false;

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
        Debug.Log("NPC activo al inicio: " + gameObject.activeSelf);
        Invoke(nameof(ActivarDeteccion), 1f);
    }
    void ActivarDeteccion()
    {
        puedeDetectarJuguete = true;
    }
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.J))
        //    jugueteDetectado = true;

        currentState.Execute();
    }
    public IEnumerator CambiarAEstadoJugar()
    {
        yield return null; 
        ChangeState(new JugarState(this));
    }
    public void ChangeState(State newState)
    {
        currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }
}