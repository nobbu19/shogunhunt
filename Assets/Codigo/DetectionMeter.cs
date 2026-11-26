using UnityEngine;

public class DetectionMeter : MonoBehaviour
{
    public static DetectionMeter Instance { get; private set; }

    [SerializeField] private float maxValue = 100f;
    [SerializeField] private float drainPerSecond = 70f;   // por cada enemigo que te ve
    [SerializeField] private float recoverPerSecond = 50f; // cuando nadie te ve

    private float current;
    private int seers;   // enemigos que te ven en este frame
    private bool dead;

    // 1 = sin detectar (vida llena), 0 = detectado del todo
    public float Normalized => current / maxValue;

    void Awake()
    {
        Instance = this;
        current = maxValue;
    }

    // Cada enemigo lo llama en cada frame en que ve al jugador
    public void ReportSeen()
    {
        seers++;
    }

    // LateUpdate corre después de todos los Update, así que ya llegaron todos los avisos
    void LateUpdate()
    {
        if (dead) return;

        if (seers > 0)
            current -= drainPerSecond * seers * Time.deltaTime;
        else
            current += recoverPerSecond * Time.deltaTime;

        seers = 0;
        current = Mathf.Clamp(current, 0f, maxValue);

        if (current <= 0f)
        {
            dead = true;
            if (GameOverManager.Instance != null)
                GameOverManager.Instance.ShowGameOver(); // igual que antes
        }
    }
}