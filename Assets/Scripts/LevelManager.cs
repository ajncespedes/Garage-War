using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {

    public void CargarMenu(string escena)
    {
        if(escena == "salir")
        {
            Application.Quit();
        }
        else
        {
            SceneManager.LoadScene(escena);
        }
    }
}
