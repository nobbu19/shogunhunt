using UnityEngine;
using UnityEngine.SceneManagement;

public class Guardia : MonoBehaviour
{
    public Transform[] puntos;          // Puntos por los que patrulla el guardia
    public Transform jugador;           // Referencia al jugador (arrastrar en el Inspector)
    public float velocidad = 2f;        // Velocidad de patrulla
    public float rangoVision = 5f;      // Qué tan lejos puede ver
    public float anguloVision = 60f;    // Qué tan ancho es su cono de visión (en grados)

    int indice = 0;                     // Punto al que se dirige ahora mismo

    void Update()
    {
        // Cada frame: se mueve y revisa si ve al jugador
        Patrullar();

        if (VeAlJugador())
        {
            // Si lo ve, se recarga la escena actual (Game Over)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    void Patrullar()
    {
        // Punto al que tiene que ir y dirección hacia él
        Transform destino = puntos[indice];
        Vector2 direccion = destino.position - transform.position;

        // Si ya llegó (está a menos de 0.1), pasa al siguiente punto
        // El % hace que al llegar al último vuelva al primero
        if (direccion.magnitude < 0.1f)
        {
            indice = (indice + 1) % puntos.Length;
            return;
        }

        // Gira el guardia para que "mire" hacia donde camina (su arriba apunta al destino)
        transform.up = direccion;

        // Lo avanza hacia el punto
        transform.position += (Vector3)(direccion.normalized * velocidad * Time.deltaTime);
    }

    bool VeAlJugador()
    {
        // Dirección y distancia desde el guardia hasta el jugador
        Vector2 haciaJugador = jugador.position - transform.position;


        // Segunda condición: el jugador debe estar dentro del cono
        // Se compara el ángulo entre donde mira el guardia y donde está el jugador
        // Se divide entre 2 porque el cono se abre a ambos lados
        return Vector2.Angle(transform.up, haciaJugador) < anguloVision / 2f;
    }

    // Solo sirve para ver el cono en la ventana Scene (no afecta al juego)
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        // Calcula los dos bordes del cono rotando la dirección "arriba" a cada lado
        Vector3 izq = Quaternion.Euler(0, 0, anguloVision / 2f) * transform.up;
        Vector3 der = Quaternion.Euler(0, 0, -anguloVision / 2f) * transform.up;

        // Dibuja las dos líneas
        Gizmos.DrawLine(transform.position, transform.position + izq * rangoVision);
        Gizmos.DrawLine(transform.position, transform.position + der * rangoVision);
    }
}