using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneButtons : MonoBehaviour
{
    // Va a la escena con ese nombre
    public void LoadScene(string sceneName)
    {
        Time.timeScale = 1f; // por si venías de un Game Over congelado
        SceneManager.LoadScene(sceneName);
    }

    // Va a la siguiente escena de la lista de Build
    public void LoadNext()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    // Cierra el juego (no hace nada dentro del editor)
    public void Quit()
    {
        Application.Quit();
    }
}