using UnityEngine;

public class EnemyPurge : MonoBehaviour
{
    public void Die()
    {
        EnemyLantern[] lanterns = FindObjectsByType<EnemyLantern>(FindObjectsSortMode.None);

        foreach (EnemyLantern lantern in lanterns)
            Kill(lantern.gameObject);

        Kill(gameObject);
    }

    private void Kill(GameObject target)
    {
        EnemyDeath death = target.GetComponent<EnemyDeath>();

        if (death != null) death.Die();
        else Destroy(target);
    }
}