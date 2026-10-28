using UnityEngine;

public class Jugador : MonoBehaviour
{
    public float velocidad = 5f;
    Rigidbody2D rb;
    Vector2 input;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + input * velocidad * Time.fixedDeltaTime);
    }
}