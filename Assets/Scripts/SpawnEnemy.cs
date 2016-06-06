using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    public GameObject enemy;                // El prefab del enemigo
    public float spawnTime = 20f;            // Tiempo entre cada spawn


    void Start()
    {
        // Llamamos a la función Spawn cada spawnTime segundos
        InvokeRepeating("Spawn", spawnTime, spawnTime);
    }


    void Spawn()
    {
        // Creamos un enemigo en esa posición
        GameObject.FindGameObjectWithTag("jugador").SendMessage("EnemigoVivo", SendMessageOptions.DontRequireReceiver);
        Instantiate(enemy, transform.position, transform.rotation);
        spawnTime-=2;
        Debug.Log("Tiempo: " + spawnTime);
    }
}