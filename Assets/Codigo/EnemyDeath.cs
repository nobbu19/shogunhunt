using UnityEngine;

public class EnemyDeath : MonoBehaviour
{
    [SerializeField] private GameObject deathEffectPrefab;

    public void Die()
    {
        if (deathEffectPrefab != null)
            Instantiate(deathEffectPrefab, transform.position, transform.rotation);

        Destroy(gameObject);
    }
}