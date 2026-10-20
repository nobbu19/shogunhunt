using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;

    [Header("Dash")]
    [SerializeField] private KeyCode dashKey = KeyCode.LeftShift;
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private float dashCooldown = 0.5f;

    [Header("Disparo")]
    [SerializeField] private KeyCode shootKey = KeyCode.J;
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private float spawnOffset = 0.5f;
    [SerializeField] private int maxShots = 5;

    private Rigidbody2D rb;
    private Vector2 input;
    private Vector2 facing = Vector2.down;

    private bool isDashing;
    private float dashTimer;
    private float cooldownTimer;
    private Vector2 dashDirection;

    private int shotsLeft;

    private static readonly int IsDashingHash = Animator.StringToHash("IsDashing");
    private static readonly int IsMovingHash = Animator.StringToHash("IsMoving");
    private static readonly int FacingUpHash = Animator.StringToHash("FacingUp");

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        shotsLeft = maxShots;
    }

    void Update()
    {



        if (Time.timeScale == 0f) return;
        cooldownTimer -= Time.deltaTime;

        // Mientras dura el dash no se lee input ni se dispara
        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0f) EndDash();
            return;
        }

        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        // 8 direcciones; normalizado para que las diagonales no sean más rápidas
        input = new Vector2(x, y).normalized;

        if (input != Vector2.zero) facing = input;

        // Invertir sprite según la dirección horizontal
        if (x > 0) spriteRenderer.flipX = false;
        else if (x < 0) spriteRenderer.flipX = true;

        UpdateAnimator();

        if (Input.GetKeyDown(dashKey) && cooldownTimer <= 0f)
            StartDash();

        if (Input.GetKeyDown(shootKey) && shotsLeft > 0)
            Shoot();
    }

    void FixedUpdate()
    {
        rb.linearVelocity = isDashing ? dashDirection * dashSpeed : input * speed;
    }

    private void UpdateAnimator()
    {
        if (animator == null) return;
        animator.SetBool(IsMovingHash, input != Vector2.zero);
        animator.SetBool(FacingUpHash, facing.y > 0f); // arriba o diagonales hacia arriba
    }

    private void StartDash()
    {
        isDashing = true;
        dashTimer = dashDuration;
        // Si no se está moviendo, dashea hacia donde mira
        dashDirection = input != Vector2.zero ? input : facing;

        if (animator != null) animator.SetBool(IsDashingHash, true);
    }

    private void EndDash()
    {
        isDashing = false;
        cooldownTimer = dashCooldown;

        if (animator != null) animator.SetBool(IsDashingHash, false);
    }

    private void Shoot()
    {
        shotsLeft--;
        Vector3 spawnPos = transform.position + (Vector3)(facing * spawnOffset);
        Projectile p = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
        p.Init(facing);

        Debug.Log($"Disparos restantes: {shotsLeft}");
        if (animator != null) animator.SetTrigger("Shoot");
    }
}