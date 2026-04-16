using UnityEngine;

public class ToySpawner : MonoBehaviour
{
    public GameObject toyPrefab;
    public int cantidad = 5;
    public float radio = 5f;
    public float altura = 1.3f;

    void Start()
    {
        for (int i = 0; i < cantidad; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * radio;

            Vector3 spawnPos = transform.position + new Vector3(
                randomCircle.x,
                altura,
                randomCircle.y
            );

            Instantiate(toyPrefab, spawnPos, Quaternion.identity);
        }
    }
}