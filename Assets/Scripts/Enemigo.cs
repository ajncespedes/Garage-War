using UnityEngine;
using System.Collections;

public class Enemigo : MonoBehaviour {

    public Light myLight;                       // Luz del tubo
    new AudioSource audio;                      // Sonido al romperse el cristal
    public GameObject balasNuevas;              // Caja de balas que suelta el enemigo al morir
    public int vida = 100;                      // Puntos de vida del enemigo
    
    void Start () {
	
	}
	
	void Update () {
        // Si el enemigo muere, desaparece y deja caer una caja de balas
        //Debug.Log("Vida: " + vida);
        if (vida < 0)
        {
            Instantiate(balasNuevas, transform.position+ new Vector3(0,2,0), transform.rotation);
            GameObject.FindGameObjectWithTag("jugador").SendMessage("EnemigoMuerto", SendMessageOptions.DontRequireReceiver);
            Destroy(this.gameObject);
        }
    }

    // Método que se llama cuando una bala llega al enemigo
    void AplicarDano(int dano)
    {
        vida -= dano;
    }

    // En caso de ser un tubo fluorescente, destruye la luz y reproduce un sonido
    void RomperLuz(){
        audio = GetComponent<AudioSource>();
        audio.enabled = true;
        audio.Play();
        Destroy(myLight.gameObject);
    }
    
}
