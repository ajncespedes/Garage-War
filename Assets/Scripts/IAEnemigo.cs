using UnityEngine;
using System.Collections;
using System;

//Cada enemigo que salga será más complicado
public class IAEnemigo : MonoBehaviour {

    // GameObjects para el control de balas, jugador e interruptores de la luz
    public GameObject Bullet_Emitter1, Bullet_Emitter2;
    public GameObject Bullet;
    GameObject player;
    GameObject[] interruptores;

    // Variables relacionadas con los efectos generados al disparar
    public Transform hitEffect, hitEffectImpacto, hitEffectFuego, hitEffectSangre, hitEffectElectricidad;

    // Bool para controlar si el jugador ha sido detectado
    bool jugadorDetectado = true;

    // Variables para las animaciones del enemigo
    public String dispararCorriendo, correr, dispararAndando, andar, dispararQuieto, quieto;
    
    // Sonido al disparar el arma
    public AudioClip sonidoDisparar;
    
    // Algoritmo para ir hacia un objetivo
    NavMeshAgent agent;

    // Número aleatorio
    System.Random r = new System.Random(DateTime.Now.Millisecond);

    // Parámetros de control
    public float Bullet_Forward_Force;
    float velocidad = 6;
    float maxDist = 200;
    float minDist = 10;
    float luminosidad;
    public int danoBala = 1000;

    //Tiempos de refresco para controlar las balas por segundo que dispara el arma
    float t1 = 0;
    float t2 = 0;
    float t_diferencia = 1;
    
    void Start () {
        agent = GetComponent<NavMeshAgent>();
        BuscarJugador(true);
        t1 = Time.deltaTime;
        player = GameObject.FindGameObjectWithTag("jugador");
        interruptores = GameObject.FindGameObjectsWithTag("interruptor");
    }
	
    // IA MUY básica
	void Update () {
        luminosidad = player.GetComponent<Jugador>().getLuminosidad();
        Transform posicionEnemigo = transform;
        float distancia = Vector3.Distance(posicionEnemigo.position, player.transform.position);
        if(distancia < maxDist && distancia > minDist && jugadorDetectado)
        {
            //transform.LookAt(player.position);
            //transform.position += transform.forward * velocidad * Time.deltaTime;
            agent.SetDestination(player.transform.position);
            
            if (r.Next(0, 10) > 5)
            {
                Atacar();
            }
            
        }
        else
        {
            
            EstarQuieto();

        }

        
    }
    
    void Atacar()
    {
        // IDEAS: Empezar a disparar al menos cuando la direccion del enemigo al jugador sea de un error de 20º
        agent.speed = velocidad;
        GetComponent<Animation>().Play(dispararAndando);

        if (t_diferencia > 0.1)
        {
            GetComponent<AudioSource>().PlayOneShot(sonidoDisparar);
            t1 += Time.deltaTime;

            RaycastHit hit;
            Vector3 direccion = Bullet_Emitter2.transform.position - Bullet_Emitter1.transform.position;
            Vector3 origen = Bullet_Emitter1.transform.position;
            Ray ray = new Ray(origen, direccion);
            if (Physics.Raycast(ray, out hit, 1000))
            {
                Instantiate(hitEffectFuego, Bullet_Emitter1.transform.position, Quaternion.LookRotation(hit.normal));
                if (hit.collider.tag == "pared")
                {
                    Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    Instantiate(hitEffectImpacto, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
                }
                else if (hit.collider.tag == "jugador" )
                {
                    Instantiate(hitEffectSangre, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
                    hit.transform.SendMessage("AplicarDano", danoBala / hit.distance, SendMessageOptions.DontRequireReceiver);
                }else if (hit.collider.tag == "tubo")
                {
                    Instantiate(hitEffectElectricidad, hit.point, Quaternion.LookRotation(hit.normal));
                    hit.transform.SendMessage("RomperLuz", danoBala / hit.distance, SendMessageOptions.DontRequireReceiver);
                }
            }

            t2 = t1;
        }
        t2 += Time.deltaTime;
        t_diferencia = t2 - t1;
    }

    void Defender()
    {

    }

    void Huir()
    {

    }

    void BuscarJugador(bool aleatorio)
    {
        
        GetComponent<Animation>().Play(andar);
    }

    void EstarQuieto()
    {
        agent.speed = 0;

        GetComponent<Animation>().Play(quieto);
    }

    void cubrir()
    {
        // IDEAS: buscar punto en el cual si lanzo un Ray al jugador, el tag de la colision no es jugador y asignarlo al A*
    }

    void dispararLuz()
    {

    }

    void encenderLuz()
    {
        // IDEAS: buscar interruptor mas cercano y encender luz
        agent.SetDestination(interruptores[0].transform.position);
    }

    public void OnTriggerStay(Collider other)
    {
        AudioSource audioSource = other.transform.root.GetComponent<AudioSource>(); // asumiendo que el componente audiosource estara siempre en la raiz del enemigo
        if (audioSource != null && audioSource.isPlaying)
        {
            // implementacion de atenuacion...
            soundDirection = (other.transform.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(soundDirection), Time.deltaTime * turnSpeed);
        }
    }
}


