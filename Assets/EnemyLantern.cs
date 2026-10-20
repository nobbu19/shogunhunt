using UnityEngine;

public class EnemyLantern : MonoBehaviour
{
    [Header("Giro")]
    [SerializeField] private float rotationSpeed = 40f; // grados por segundo (negativo = sentido horario)
    [SerializeField] private Vector2 startFacing = Vector2.down;

    [Header("Visión")]
    [SerializeField] private Transform lightPivot;
    [SerializeField] private float viewDistance = 5f;
    [SerializeField, Range(10f, 180f)] private float viewAngle = 60f;
    [SerializeField] private LayerMask obstacleMask;

    [Header("Detección")]
    [SerializeField] private float detectionTime = 2f;
    [SerializeField] private float cooldownSpeed = 1f;

    private Transform player;
    private Vector2 facing;
    private float detectionTimer;
    private bool detected;

    public float DetectionProgress => detectionTimer / detectionTime;

    void Awake()
    {
        facing = startFacing.normalized;

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
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
        facing = Quaternion.Euler(0f, 0f, rotationSpeed * Time.deltaTime) * facing;
    }

    private bool CanSeePlayer()
    {
        Vector2 toPlayer = player.position - transform.position;

        if (toPlayer.magnitude > viewDistance) return false;
        if (Vector2.Angle(facing, toPlayer) > viewAngle * 0.5f) return false;

        if (Physics2D.Raycast(transform.position, toPlayer.normalized, toPlayer.magnitude, obstacleMask))
            return false;

        return true;
    }

    private void UpdateLight()
    {
        if (lightPivot == null) return;

        float angle = Mathf.Atan2(facing.y, facing.x) * Mathf.Rad2Deg - 90f;
        lightPivot.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    void OnDrawGizmos()
    {
        Vector3 dir = Application.isPlaying ? (Vector3)facing : (Vector3)startFacing.normalized;
        Vector3 pos = transform.position;
        float half = viewAngle * 0.5f;

        Gizmos.color = Color.yellow;
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