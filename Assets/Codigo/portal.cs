using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    [SerializeField] private string sceneName;            // escena a la que lleva
    [SerializeField] private float rotationSpeed = 90f;   // grados por segundo (negativo = sentido horario)
    [SerializeField] private Transform visual;            // opcional: qué objeto gira

    void Update()
    {
        Transform target = visual != null ? visual : transform;
        target.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }
}