using UnityEngine;

public class CanvasEnemys3DEffect : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        Vector3 dir = transform.position - mainCamera.transform.position;
        dir.y = 0;
        transform.rotation = Quaternion.LookRotation(dir);
    }
}