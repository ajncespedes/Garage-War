using UnityEngine;
using System.Collections;
using System;

//Cada enemigo que salga será más complicado
public class IAEnemigo : MonoBehaviour {

    // GameObjects para el control de balas, jugador e interruptores de la luz
    public GameObject Bullet_Emitter1, Bullet_Emitter2;
    GameObject player;
    GameObject[] interruptores;
    GameObject[] lightGameObjects;

    // Variables relacionadas con los efectos generados al disparar
    public Transform hitEffect, hitEffectImpacto, hitEffectFuego, hitEffectSangre, hitEffectElectricidad;

    // Bool para controlar si el jugador ha sido detectado
    bool jugadorDetectado = false;

    // Variables para las animaciones del enemigo
    public String dispararCorriendo, correr, dispararAndando, andar, dispararQuieto, quieto, apuntarAndando;
    
    // Sonido al disparar el arma
    public AudioClip sonidoDisparar;
    
    // Algoritmo para ir hacia un objetivo
    NavMeshAgent agent;

    // Número aleatorio
    System.Random r = new System.Random(DateTime.Now.Millisecond);

    // Parámetros de control
    float luminosidad;
    public float velocidad = 6;
    public int danoBala = 100;
    public int punteria = 500;
    public int rangoVision = 200;
    public int rangoAudicion = 30;

    // Tiempos de refresco para controlar las balas por segundo que dispara el arma y búsqueda de jugador
    float t1 = 0;
    float t2 = 0;
    float t_diferencia = 1;
    float tiempoBuscando = -10;
    float tiempoDeOlvido = 4;

    int indiceLucesRotas = 0;

    void Start () {
        agent = GetComponent<NavMeshAgent>();
        t1 = Time.deltaTime;
        player = GameObject.FindGameObjectWithTag("jugador");
        interruptores = GameObject.FindGameObjectsWithTag("interruptor");
        lightGameObjects = GameObject.FindGameObjectsWithTag("tubo");

        // Ajuste de dificultad en función de los enemigos muertos
        int muertos = player.GetComponent<Jugador>().getMuertos();
        rangoVision = rangoVision + muertos * 10;
        rangoAudicion = rangoAudicion + muertos * 5;
        punteria = punteria + muertos * 20;
        danoBala = danoBala + muertos * 10;
        velocidad = velocidad + muertos / 10;
        Debug.Log("Vision: " + rangoVision + " Audicion: " + rangoAudicion + " Punteria: " + punteria + " Daño Bala: " + danoBala + " Velocidad: " + velocidad);

        agent.stoppingDistance = 10;
        BuscarJugador();
    }
	
    // IA básica
	void Update () {
        //Debug.Log("Detectado: "+jugadorDetectado+" Tiempo de olvido: "+ tiempoDeOlvido + " Rango de Vision: " + 100 * luminosidad + " Distancia: " + Vector3.Distance(transform.position, player.transform.position));
        
        if (jugadorDetectado)
        {
            agent.SetDestination(player.transform.position);
            if (!Vista())
            {
                tiempoDeOlvido -= Time.deltaTime;
            }
            else
            {
                tiempoDeOlvido = 4;
            }
            
            if(tiempoDeOlvido <= 0)
            {
                jugadorDetectado = false;
            }
            else
            {
                Atacar();
            }
        }
        else if(Oido())
        {
            agent.SetDestination(player.transform.position);
        }
        else
        {
            BuscarJugador();
        }
        
        luminosidad = player.GetComponent<Jugador>().getLuminosidad() * 10;

        if(luminosidad > 1)
        {
            luminosidad = 1;
        }
    }

    void disparar()
    {
        //Debug.Log("i: " + new Vector3(r.Next(-punteria, punteria), r.Next(-punteria / 10, punteria / 10), 0));
        if (t_diferencia > 0.1)
        {
            GetComponent<AudioSource>().PlayOneShot(sonidoDisparar);
            t1 += Time.deltaTime;

            RaycastHit hit;
            Vector3 direccion = Bullet_Emitter2.transform.position + new Vector3(1f / r.Next(-punteria, punteria), 1f / r.Next(-punteria, punteria), 1f / r.Next(-punteria, punteria)) - Bullet_Emitter1.transform.position ;
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
                else if (hit.collider.tag == "jugador")
                {
                    Instantiate(hitEffectSangre, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
                    hit.transform.SendMessage("AplicarDano", danoBala / hit.distance, SendMessageOptions.DontRequireReceiver);
                }
                else if (hit.collider.tag == "tubo")
                {
                    Instantiate(hitEffectElectricidad, hit.point, Quaternion.LookRotation(hit.normal));
                    hit.transform.SendMessage("RomperLuz", danoBala / hit.distance, SendMessageOptions.DontRequireReceiver);
                    indiceLucesRotas++;
                }
            }

            t2 = t1;
        }
        t2 += Time.deltaTime;
        t_diferencia = t2 - t1;
    }
    
    void Atacar()
    {
        transform.LookAt(player.transform.position);
        agent.speed = velocidad;
        if(Vector3.Distance(transform.position, player.transform.position) > 50)
        {
            agent.speed = velocidad + 5;
            GetComponent<Animation>().Play(dispararCorriendo);
            disparar();
        }
        if (Vector3.Distance(transform.position, player.transform.position) < 50 && Vector3.Distance(transform.position, player.transform.position) > 15)
        {
            agent.speed = velocidad;
            GetComponent<Animation>().Play(dispararAndando);
            disparar();
        }
        else
        {
            agent.speed = 0; // Cubrirse de alguna forma más adelante
            GetComponent<Animation>().Play(dispararQuieto);
            disparar();
        }
        
    }

    void Defender()
    {

    }

    void Huir()
    {

    }

    void BuscarJugador()
    {
        //Debug.Log("Estoy buscandote");
        jugadorDetectado = Vista();
        if (tiempoBuscando <= 0)
        {
            tiempoBuscando = 10;
            Vector3 posicionAleatoria = new Vector3(r.Next(-30, 40), 0, r.Next(-45, 35));
            agent.SetDestination(posicionAleatoria);
            GetComponent<Animation>().Play(andar);
            agent.speed = velocidad/2;
        }
        tiempoBuscando -= Time.deltaTime;
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
        int i = indiceLucesRotas;
        agent.speed = 6;
        agent.SetDestination(lightGameObjects[i].transform.position);
        GetComponent<Animation>().Play(andar);
        float distancia = Vector3.Distance(transform.position, lightGameObjects[i].transform.position);
        
        if (distancia < 20)
        {
            GetComponent<Animation>().Play(dispararQuieto);
            agent.speed = 0;
            transform.LookAt(lightGameObjects[i].transform.position);
            //Bullet_Emitter1.transform.LookAt(lightGameObjects[i].transform.position);
            Bullet_Emitter2.transform.position = lightGameObjects[i].transform.position + new Vector3(1f/r.Next(-punteria, punteria), 1f / r.Next(-punteria, punteria), 1f / r.Next(-punteria, punteria));
            //Debug.Log("i: " + new Vector3(1f / r.Next(-10, 10), 1f / r.Next(-10, 10), 1f / r.Next(-10, 10)));
            disparar();
        }
    }

    
    

    void encenderLuz()
    {
        // IDEAS: buscar interruptor mas cercano y encender luz
        agent.SetDestination(interruptores[0].transform.position);
    }

    // Sentido del tacto
    bool Tacto()
    {
        transform.LookAt(player.transform.position);
        return true;
    }

   

    // Sentido del oido
    bool Oido()
    {
        if (Vector3.Distance(transform.position, player.transform.position) < rangoAudicion)
        {
            AudioSource audioSource = player.gameObject.GetComponent<AudioSource>(); // asumiendo que el componente audiosource estara siempre en la raiz del enemigo
            if (audioSource != null && audioSource.isPlaying)
            {
                //Debug.Log("Te oigo!!");
                return true;
            }
        }
        //Debug.Log("No te oigo");
        return false;
    }

    // Sentido de la vista
    bool Vista()
    {
        RaycastHit hit;
        Vector3 direccion = Bullet_Emitter2.transform.position - Bullet_Emitter1.transform.position;
        Vector3 direccionJugador = player.transform.position - Bullet_Emitter1.transform.position;
        float angulo = Vector3.Angle(direccion, direccionJugador);
        //Debug.Log("Angulo: " + angulo + " Detectado: " + jugadorDetectado + " Tiempo de olvido: " + tiempoDeOlvido + " Jugador: " + direccionJugador);
        //Debug.Log("Rango de visión: " +  90 * luminosidad + " Distancia: " + Vector3.Distance(player.transform.position, transform.position) + " Tiempo: " + punteria);
        if(Vector3.Distance(transform.position, player.transform.position) > 5)
        {
            if (angulo < 70)
            {
                Ray ray = new Ray(Bullet_Emitter1.transform.position, direccionJugador);
                if (Physics.Raycast(ray, out hit, rangoVision * luminosidad))
                {
                    //Debug.Log("Angulo: " + angulo+ " Tag: "+ hit.collider.tag+ " Detectado: " + jugadorDetectado + " Tiempo de olvido: " + tiempoDeOlvido);

                    if (hit.collider.tag == "jugador")
                    {
                        //Debug.Log("Te veo!!");
                        return true;
                    }
                }
            }
        }
        else
        {
            if (angulo < 120)
            {
                Ray ray = new Ray(Bullet_Emitter1.transform.position, direccionJugador);
                if (Physics.Raycast(ray, out hit, rangoVision * luminosidad))
                {
                    //Debug.Log("Angulo: " + angulo+ " Tag: "+ hit.collider.tag+ " Detectado: " + jugadorDetectado + " Tiempo de olvido: " + tiempoDeOlvido);

                    if (hit.collider.tag == "jugador")
                    {
                        //Debug.Log("Te veo!!");
                        return true;
                    }
                }
            }
        }
        
        
        return false;
    }

    void ObservarEntorno()
    {

    }
}


