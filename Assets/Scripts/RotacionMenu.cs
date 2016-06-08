using UnityEngine;
using System.Collections;

public class RotacionMenu : MonoBehaviour {

    public UnityEngine.UI.Text ultimo, record;
    // Use this for initialization
    void Start () {
        Cursor.visible = true;
        ultimo.text = "Puntuación últ vez: " + PlayerPrefs.GetInt("ultimo", 0).ToString();
        record.text = "Puntuación récord: " + PlayerPrefs.GetInt("record", 0).ToString();
    }
	
	// Update is called once per frame
	void Update () {
        //transform.Rotate(new Vector3(0, 0.5f, 0));
        transform.Rotate(new Vector3(0, 0.2f, 0));
    }
}
