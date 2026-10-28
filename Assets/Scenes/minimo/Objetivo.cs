using UnityEngine;
using UnityEngine.SceneManagement;

public class Objetivo : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("¡Objetivo eliminado! Ganaste");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}