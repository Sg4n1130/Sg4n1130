using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class BossHomingMissile : MonoBehaviour
{
    public float damage = 10f;
    public float lifeTime = 5f;
    public float speed = 7f;
    public float rotateSpeed = 200f;
    public float homingDelay = 0.5f;

    private Transform playerTarget;
    private Rigidbody2D rb;
    private float aliveTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifeTime);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTarget = player.transform;
        }
    }

    void FixedUpdate()
    {
        aliveTimer += Time.fixedDeltaTime;

        if (aliveTimer > homingDelay && playerTarget != null && playerTarget.gameObject.activeInHierarchy)
        {
            Vector2 direction = (Vector2)playerTarget.position - rb.position;
            direction.Normalize();

            float rotateAmount = Vector3.Cross(direction, transform.up).z;
            rb.angularVelocity = -rotateAmount * rotateSpeed;
            rb.linearVelocity = transform.up * speed;
        }
        else
        {
            rb.linearVelocity = transform.up * speed;
            rb.angularVelocity = 0f;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
            Destroy(gameObject);
        }
    }
}