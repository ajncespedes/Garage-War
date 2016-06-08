using UnityEngine;
using System.Collections;

public class Disparo : MonoBehaviour
{
    // Variables relacionadas con la arma
    public GameObject Bullet_Emitter;
    public GameObject arma;

    // Variables relacionadas con los efectos generados al disparar
    public Transform hitEffect, hitEffectImpacto, hitEffectFuego, hitEffectSangre, hitEffectElectricidad, hitEffectTuberia;

    // Variables de los sonidos
    new AudioSource audio;
    public AudioClip sonidoDisparar;
    public AudioClip sonidoRecargar1;
    public AudioClip sonidoSinBalas;

    // Vectores de posición para controlar la animación de apuntar
    public Vector3 A, C;
    private Vector3 D;

    // Texto en pantalla para contar las balas
    public UnityEngine.UI.Text textoBalas;

    // Velocidad de la animación de apuntar con el arma
    private float velocidadX = 0.4f;
    private float velocidadY = 0.4f;
    private float velocidadZ = 0.4f;

    // Nombre de las animaciones
    public string NombreAnimacionRecarga;
    public string NombreAnimacionDisparar;
    public string NombreAnimacionDispararApuntando;

    // Variables relacionadas con las balas
    public int nBalasCargador = 40;
    public int nBalasMax = 200;
    public int danoBala = 1000;
    int nBalas;

    //Tiempos de refresco para controlar las balas por segundo que dispara el arma
    float t1 = 0;
    float t2 = 0;
    float tDiferencia = 1;
    float tiempoRecarga = 3.3f;

    bool recargando = false;


    void Start ()
    {
        nBalas = nBalasCargador;
        t1 = Time.deltaTime;
    }
	
	void Update ()
    {
        textoBalas.text = nBalas.ToString() + "/" + nBalasMax.ToString();

        // Generamos la animación de apuntar
        float newX = Mathf.SmoothDamp(arma.transform.localPosition.x, D.x, ref velocidadX, .1f);
        float newY = Mathf.SmoothDamp(arma.transform.localPosition.y, D.y, ref velocidadY, .1f);
        float newZ = Mathf.SmoothDamp(arma.transform.localPosition.z, D.z, ref velocidadZ, .1f);
        Vector3 x = new Vector3(newX, newY, newZ);
        arma.transform.localPosition = x;

        // Si pulsamos el botón derecho del ratón, se produce la animación
        if (Input.GetButton("Fire2"))
        {
            D.x = A.x;
            D.y = A.y;
            D.z = A.z;
        }
        else {
            D.x = C.x;
            D.y = C.y;
            D.z = C.z;
        }

        // Si se ha producido la diferencia de tiempo suficiente como para volver a disparar
        if (tDiferencia > 0.1 && !recargando)
        {
            // Si tenemos balas en el cargador
            if (nBalas > 0)
            {
                // Si pulsamos el botón izquierdo del ratón
                if (Input.GetButton("Fire1"))
                {
                    // Actualizamos tiempos y balas
                    t1 += Time.deltaTime;
                    nBalas--;
                    audio = arma.GetComponent<AudioSource>();
                    GetComponent<AudioSource>().PlayOneShot(sonidoDisparar);

                    // Trazamos un ray desde el centro de la pantalla
                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0));
                    if(Physics.Raycast(ray, out hit, 1000))
                    {
                        // Reproducimos el fuego del arma
                        Instantiate(hitEffectFuego, Bullet_Emitter.transform.position, Quaternion.LookRotation(hit.normal));

                        // Si la bala llega a la pared, dejamos la marca de la bala y las chispas
                        if (hit.collider.tag == "pared" || hit.collider.tag == "columna")
                        {
                            Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
                            Instantiate(hitEffectImpacto, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
                        }
                        // Si la bala llega al enemigo, mostramos la sangre y le aplicamos el daño
                        else if (hit.collider.tag == "enemigo")
                        {
                            Instantiate(hitEffectSangre, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
                            hit.transform.SendMessage("AplicarDano", 2 * danoBala / hit.distance, SendMessageOptions.DontRequireReceiver);
                            hit.transform.SendMessage("Tacto", SendMessageOptions.DontRequireReceiver);
                        }
                        // Si la bala llega a la cabeza del enemigo, mostramos la sangre y lo matamos directamente
                        else if (hit.collider.tag == "cabeza_enemigo")
                        {
                            Instantiate(hitEffectSangre, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
                            hit.transform.SendMessage("AplicarDano", 99999, SendMessageOptions.DontRequireReceiver);
                            hit.transform.SendMessage("Tacto", SendMessageOptions.DontRequireReceiver);
                        }
                        // Si la bala llega al tubo de la luz, reproducimos las chispas eléctricas y rompemos el tubo
                        else if (hit.collider.tag == "tubo")
                        {
                            Instantiate(hitEffectElectricidad, hit.point, Quaternion.LookRotation(hit.normal));
                            hit.transform.SendMessage("RomperLuz", SendMessageOptions.DontRequireReceiver);
                        }
                        // Si la bala llega a una tubería, reproducimos el agua a presión
                        else if (hit.collider.tag == "tuberia")
                        {
                            Instantiate(hitEffectImpacto, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
                            Instantiate(hitEffectTuberia, hit.point, Quaternion.LookRotation(hit.normal));
                        }

                    }
                    t2 = t1;
                }
            }
            // Si no tenemos balas
            else
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    GetComponent<AudioSource>().PlayOneShot(sonidoSinBalas);
                }
            }

            // Control de la recarga del arma
            if (Input.GetKeyDown(KeyCode.R) && nBalas < nBalasCargador)
            {
                Recargar();
                if (nBalasMax >= nBalasCargador)
                {
                    nBalasMax -= (nBalasCargador - nBalas);
                    nBalas = nBalasCargador;
                }
                else
                {
                    nBalas = nBalasMax;
                    nBalasMax = 0;
                }
            }
        }
        t2 += Time.deltaTime;
        tDiferencia = t2 - t1;
        if (recargando)
        {
            tiempoRecarga -= Time.deltaTime;
        }
        if(tiempoRecarga <= 0)
        {
            recargando = false;
            tiempoRecarga = 3.3f;
        }
    }

    void Recargar()
    {
        GetComponent<AudioSource>().PlayOneShot(sonidoRecargar1);
        arma.GetComponent<Animation>().Play(NombreAnimacionRecarga);
        recargando = true;
    }
    
    
    void recogerBalas(int balas)
    {
        nBalasMax += balas;
    }
}