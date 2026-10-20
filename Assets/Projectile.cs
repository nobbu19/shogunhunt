using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 12f;
    [SerializeField] private float lifetime = 2f;

    private Vector2 direction;

    public void Init(Vector2 dir)
    {
        direction = dir.normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Ignora al jugador y a triggers (como conos de visión)
        if (other.CompareTag("Player") || other.isTrigger) return;

        if (other.CompareTag("Enemy"))
            Destroy(other.gameObject); // por ahora el enemigo muere de un disparo

        Destroy(gameObject); // choca con enemigo o pared
    }
}