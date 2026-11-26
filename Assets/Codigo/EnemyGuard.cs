using UnityEngine;

public class EnemyGuard : MonoBehaviour
{
    [Header("Patrulla")]
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float waitTime = 1f;

    [Header("Visión")]
    [SerializeField] private Transform lightPivot;
    [SerializeField] private float viewDistance = 5f;
    [SerializeField, Range(10f, 180f)] private float viewAngle = 60f;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private Vector2 startFacing = Vector2.down;
    [SerializeField] private float turnSpeed = 180f; // grados por segundo al girar el cono

    private Transform player;
    private Vector2 facing;        // hacia dónde apunta el cono ahora
    private Vector2 targetFacing;  // hacia dónde debe apuntar
    private int index;
    private float waitTimer;

    void Awake()
    {
        facing = startFacing.normalized;
        targetFacing = facing;

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    void Update()
    {
        Patrol();
        RotateFacing();
        UpdateLight();

        if (player == null) return;

        if (CanSeePlayer() && DetectionMeter.Instance != null)
            DetectionMeter.Instance.ReportSeen();
    }

    private void Patrol()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        if (waitTimer > 0f)
        {
            waitTimer -= Time.deltaTime;

            // Mientras espera, empieza a girar hacia el siguiente punto
            Vector2 toNext = (Vector2)waypoints[index].position - (Vector2)transform.position;
            if (toNext.sqrMagnitude > 0.0001f) targetFacing = toNext.normalized;
            return;
        }

        Vector2 target = waypoints[index].position;
        Vector2 pos = transform.position;
        Vector2 dir = target - pos;

        if (dir.magnitude > 0.05f)
        {
            targetFacing = dir.normalized;
            transform.position = Vector2.MoveTowards(pos, target, speed * Time.deltaTime);
        }
        else
        {
            index = (index + 1) % waypoints.Length;
            waitTimer = waitTime;
        }
    }

    // Gira el cono poco a poco hacia targetFacing
    private void RotateFacing()
    {
        float current = Mathf.Atan2(facing.y, facing.x) * Mathf.Rad2Deg;
        float target = Mathf.Atan2(targetFacing.y, targetFacing.x) * Mathf.Rad2Deg;
        float next = Mathf.MoveTowardsAngle(current, target, turnSpeed * Time.deltaTime);

        facing = new Vector2(Mathf.Cos(next * Mathf.Deg2Rad), Mathf.Sin(next * Mathf.Deg2Rad));
    }

    private bool CanSeePlayer()
    {
        Vector2 toPlayer = player.position - transform.position;

        if (toPlayer.magnitude > viewDistance) return false;
        if (Vector2.Angle(facing, toPlayer) > viewAngle * 0.5f) return false;

        // Las paredes bloquean la visión
        if (Physics2D.Raycast(transform.position, toPlayer.normalized, toPlayer.magnitude, obstacleMask))
            return false;

        return true;
    }

    private void UpdateLight()
    {
        if (lightPivot == null) return;

        // El Spot Light 2D apunta hacia arriba (+Y) con rotación 0
        float angle = Mathf.Atan2(facing.y, facing.x) * Mathf.Rad2Deg - 90f;
        lightPivot.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    // ---------- Gizmos (cono de visión y ruta) ----------
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

        // Ruta de patrulla
        if (waypoints == null) return;
        Gizmos.color = Color.cyan;
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] == null) continue;
            Gizmos.DrawWireSphere(waypoints[i].position, 0.15f);

            Transform next = waypoints[(i + 1) % waypoints.Length];
            if (next != null) Gizmos.DrawLine(waypoints[i].position, next.position);
        }
    }
}