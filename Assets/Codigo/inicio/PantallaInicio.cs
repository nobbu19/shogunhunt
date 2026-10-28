using UnityEngine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PantallaInicio : MonoBehaviour
{
    [SerializeField] private string escenaSiguiente = "Juego";

    private void OnEnable()
    {
        if (Keyboard.current != null)
            Keyboard.current.onTextInput += AlEscribir;
    }

    private void OnDisable()
    {
        if (Keyboard.current != null)
            Keyboard.current.onTextInput -= AlEscribir;
    }

    private void AlEscribir(char c)
    {
        if (char.IsLetter(c))
            SceneManager.LoadScene(escenaSiguiente);
    }
}