using UnityEngine;
using System.Collections;

public class luces : MonoBehaviour
{
    GameObject[] lightGameObjects = null;
    Light[] myLight = null;
    GameObject[] target = null;
    float timeLeft;
    AudioSource audio;


    void Start()
    {
        timeLeft = 120;
        target = GameObject.FindGameObjectsWithTag("interruptor");
        lightGameObjects = GameObject.FindGameObjectsWithTag("luz");
        //private Light myLight;
        myLight = new Light[lightGameObjects.Length];
        for (int i = 0; i < lightGameObjects.Length; i++)
        {
            myLight[i] = lightGameObjects[i].GetComponent<Light>();
        }
        
        //myLight = GetComponent<Light>();
    }

   

    void Update()
    {
        
        timeLeft -= Time.deltaTime;
        if (timeLeft < 0)
        {
            for (int j = 0; j < lightGameObjects.Length; j++)
            {
                myLight[j].enabled = false;
            }
        }
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
                    timeLeft = 120;
                }
            }
        }
        Debug.Log("Tiempo restante: " + timeLeft);
    }
}