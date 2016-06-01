using UnityEngine;
using System.Collections;

public class CajaBalas : MonoBehaviour {

    public int balasAnadir = 40;
    
	void Start () {
	
	}
	
	void Update () {
        
    }

    void OnTriggerEnter(Collider other)
    {
        // Si entramos en el área definida, recogemos las balas
        if(other.gameObject.tag == "jugador")
        {
            other.gameObject.GetComponent<Disparo>().nBalasMax += balasAnadir;
            Destroy(this.gameObject);
        }
    }
}
