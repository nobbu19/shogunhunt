using UnityEngine;

public class EnemySentinel : MonoBehaviour
{
    [System.Serializable]
    public struct LookPoint
    {
        public float angle;    // 0 = arriba, 90 = izquierda, 180 = abajo, -90 = derecha
        public float waitTime; // segundos que se queda mirando ahí
    }

    [Header("Giro")]
    [SerializeField] private LookPoint[] lookPoints;
    [SerializeField] private float rotateSpeed = 90f; // grados por segundo

    [Header("Visión")]
    [SerializeField] private Transform lightPivot;
    [SerializeField] private float viewDistance = 5f;
    [SerializeField, Range(10f, 180f)] private float viewAngle = 60f;
    [SerializeField] private LayerMask obstacleMask;

    [Header("Detección")]
    [SerializeField] private float detectionTime = 0.3f;
    [SerializeField] private float cooldownSpeed = 1f;

    private Transform player;
    private float currentAngle;
    private int index;
    private bool waiting;
    private float waitTimer;
    private float detectionTimer;
    private bool detected;

    private Vector2 Facing => Quaternion.Euler(0f, 0f, currentAngle) * Vector3.up;

    public float DetectionProgress => detectionTimer / detectionTime;

    void Awake()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;

        // Empieza mirando el primer punto y esperando ahí
        if (lookPoints != null && lookPoints.Length > 0)
        {
            currentAngle = lookPoints[0].angle;
            waiting = true;
            waitTimer = lookPoints[0].waitTime;
        }
    }

    void Update()
    {
        Rotate();
        UpdateLight();

        if (player == null) return;

        if (CanSeePlayer())
            detectionTimer += Time.deltaTime;
        else
            detectionTimer = Mathf.Max(0f, detectionTimer - cooldownSpeed * Time.deltaTime);

        if (detectionTimer >= detectionTime && !detected)
        {
            detected = true;
            Debug.Log("¡Te vio!");
            if (GameOverManager.Instance != null)
                GameOverManager.Instance.ShowGameOver();
        }

        if (detectionTimer < detectionTime)
            detected = false;
    }

    private void Rotate()
    {
        if (lookPoints == null || lookPoints.Length == 0) return;

        if (waiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
            {
                waiting = false;
                index = (index + 1) % lookPoints.Length;
            }
            return;
        }

        float target = lookPoints[index].angle;
        currentAngle = Mathf.MoveTowardsAngle(currentAngle, target, rotateSpeed * Time.deltaTime);

        if (Mathf.Abs(Mathf.DeltaAngle(currentAngle, target)) < 0.1f)
        {
            waiting = true;
            waitTimer = lookPoints[index].waitTime;
        }
    }

    private bool CanSeePlayer()
    {
        Vector2 toPlayer = player.position - transform.position;

        if (toPlayer.magnitude > viewDistance) return false;
        if (Vector2.Angle(Facing, toPlayer) > viewAngle * 0.5f) return false;

        if (Physics2D.Raycast(transform.position, toPlayer.normalized, toPlayer.magnitude, obstacleMask))
            return false;

        return true;
    }

    private void UpdateLight()
    {
        if (lightPivot == null) return;

        // El Spot Light 2D apunta hacia arriba con rotación 0, igual que angle = 0
        lightPivot.rotation = Quaternion.Euler(0f, 0f, currentAngle);
    }

    // ---------- Gizmos ----------
    void OnDrawGizmos()
    {
        // Todas las direcciones configuradas, en tenue
        if (lookPoints != null)
        {
            foreach (LookPoint lp in lookPoints)
                DrawCone(lp.angle, new Color(1f, 1f, 0f, 0.3f));
        }

        // Hacia donde mira ahora, en amarillo fuerte
        if (Application.isPlaying)
            DrawCone(currentAngle, Color.yellow);
    }

    private void DrawCone(float angle, Color color)
    {
        Vector3 dir = Quaternion.Euler(0f, 0f, angle) * Vector3.up;
        Vector3 pos = transform.position;
        float half = viewAngle * 0.5f;

        Gizmos.color = color;
        Vector3 prev = pos + Quaternion.Euler(0, 0, half) * dir * viewDistance;
        Gizmos.DrawLine(pos, prev);

        int steps = 20;
        for (int i = 1; i <= steps; i++)
        {
            float a = Mathf.Lerp(half, -half, i / (float)steps);
            Vector3 p = pos + Quaternion.Euler(0, 0, a) * dir * viewDistance;
            Gizmos.DrawLine(prev, p);
            prev = p;
        }
        Gizmos.DrawLine(pos, prev);
    }
}