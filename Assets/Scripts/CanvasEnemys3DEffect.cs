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
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera == null) return;

        Vector3 dir = transform.position - mainCamera.transform.position;
        dir.y = 0;

        if (dir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(dir);
    }
}