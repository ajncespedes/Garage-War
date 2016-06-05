using UnityEngine;
using System.Collections;

public class Jugador : MonoBehaviour {

    public int maxVida = 100;                                               // Puntos de vida del jugador 
    int vida;                                                               // Vida actual del jugador
    public UnityEngine.UI.Text textoVida, enemigosMuertos;                  // Textos en pantalla que muestran la vida del jugador y los enemigos muertos
    GameObject[] lightGameObjects = null;                                   // Luces de tipo GameObject
    float luminosidad;                                                      // Luz reflejada por el jugador
    float t1;                                                               // Tiempo de refresco
    int muertos, vivos;                                                     // Número de enemigos muertos y vivos


    void Start () {
        vida = maxVida;
        luminosidad = 0;
        t1 = 0;
        muertos = 0;
        vivos = 1;
        enemigosMuertos.text = "Enemigos muertos: " + muertos.ToString() + "/" + vivos.ToString();
    }
	
	void Update () {

        // Añadimos un punto de vida cada segundo
        textoVida.text = vida.ToString() + " ❤";
        if (t1 > 1 && vida < maxVida)
        {
            t1 = 0;
            vida += 1;
        }
        // Calculamos la luz que nos llega desde todas las luces
        lightGameObjects = GameObject.FindGameObjectsWithTag("luz");
        luminosidad = 0;
        for (int i = 0; i < lightGameObjects.Length; i++)
        {
            float distancia = Vector3.Distance(transform.position, lightGameObjects[i].transform.position);
            float rango = lightGameObjects[i].GetComponent<Light>().range;
            float intensidad = lightGameObjects[i].GetComponent<Light>().intensity;
            // Si la luz llega al jugador
            if (distancia < rango && lightGameObjects[i].GetComponent<Light>().enabled)
            {
                // Mido cuánta luz le llega al jugador, la luz pierde energía con el cuadrado de la distancia
                luminosidad += intensidad / (distancia * distancia);
            }
        }

        // Luces de emergencia
        lightGameObjects = GameObject.FindGameObjectsWithTag("luz_emergencia");
        for (int i = 0; i < lightGameObjects.Length; i++)
        {
            float distancia = Vector3.Distance(transform.position, lightGameObjects[i].transform.position);
            float rango = lightGameObjects[i].GetComponent<Light>().range;
            float intensidad = lightGameObjects[i].GetComponent<Light>().intensity;
            
            if (distancia < rango && lightGameObjects[i].GetComponent<Light>().enabled) // Si la luz me llega
            {
                // Mido cuánta luz le llega al jugador, la luz pierde energía con el cuadrado de la distancia
                luminosidad += intensidad / (distancia * distancia);
            }
        }
        //Debug.Log("Luminosidad: " + (100*luminosidad));

        t1 += Time.deltaTime;

        if(vida < 0)
        {
            Application.Quit();
        }

    }

    // Método que recibe daño de una bala enemiga
    void AplicarDano(int dano)
    {
        vida -= dano;
    }

    // Método público para informar al enemigo de la luminosidad del jugador
    public float getLuminosidad()
    {
        return luminosidad;
    }

    // Método público para informar al enemigo de la luminosidad del jugador
    public int getMuertos()
    {
        return muertos;
    }

    // Método para informar de la muerte de un enemigo
    void EnemigoMuerto()
    {
        muertos++;
        enemigosMuertos.text = "Enemigos muertos: " + muertos.ToString() + "/" + vivos.ToString();
    }

    // Método para informar de un nuevo enemigo
    void EnemigoVivo()
    {
        vivos++;
        enemigosMuertos.text = "Enemigos muertos: " + muertos.ToString() + "/" + vivos.ToString();
    }

}
