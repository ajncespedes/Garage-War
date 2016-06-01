using UnityEngine;
using System.Collections;

public class luces : MonoBehaviour
{
    GameObject[] lightGameObjects = null;       // Luces de tipo GameObject
    Light[] myLight = null;                     // Luces de tipo Light
    GameObject[] target = null;                 // Interruptores disponibles
    public float initialTimeLeft;               // Tiempo que tardan las luces en apagarse
    float timeLeft;                             // Tiempo que lleva la cuenta atrás
    new AudioSource audio;                      // Sonido del interruptor al ser pulsado


    void Start()
    {
        // Buscamos todos los interruptores disponibles en la escena
        target = GameObject.FindGameObjectsWithTag("interruptor");
        timeLeft = initialTimeLeft;
    }

   

    void Update()
    {
        // Buscamos todos las luces y obtenemos su componente luz
        lightGameObjects = GameObject.FindGameObjectsWithTag("luz");
        myLight = new Light[lightGameObjects.Length];
        for (int i = 0; i < lightGameObjects.Length; i++)
        {
            myLight[i] = lightGameObjects[i].GetComponent<Light>();
        }
        // Realizamos la cuenta atrás, cuando llega a 0 las luces se apagan
        timeLeft -= Time.deltaTime;
        if (timeLeft < 0)
        {
            for (int j = 0; j < lightGameObjects.Length; j++)
            {
                myLight[j].enabled = false;
            }
        }

        // Si estamos cerca de un interruptor y pulsamos la tecla F, encendemos las luces
        for (int i = 0; i < target.Length; i++)
        {
            float distance = Vector3.Distance(transform.position, target[i].transform.position);
            if (distance < 5)
            {
                if (Input.GetKeyUp(KeyCode.F))
                {
                    audio = target[i].GetComponent<AudioSource>();
                    audio.enabled = true;
                    audio.Play();
                    for (int j = 0; j < lightGameObjects.Length; j++)
                    {
                        myLight[j].enabled = true;
                    }
                    timeLeft = initialTimeLeft;
                }
            }
        }
    }
}