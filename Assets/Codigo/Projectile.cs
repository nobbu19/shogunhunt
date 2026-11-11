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
        if (other.CompareTag("Player") || other.isTrigger) return;

        if (other.CompareTag("Enemy"))
        {
            EnemyPurge purge = other.GetComponent<EnemyPurge>();
            EnemyDeath death = other.GetComponent<EnemyDeath>();

            if (purge != null) purge.Die();
            else if (death != null) death.Die();   // aquí sale el efecto de muerte
            else Destroy(other.gameObject);        // sin EnemyDeath muere sin efecto
        }

        Destroy(gameObject);
    }
}