using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class BoomerangBullet : MonoBehaviour
{
    public float damage = 10f;
    public float throwSpeed = 15f;
    public float returnSpeed = 25f;
    public float flyTime = 1.5f;
    public float stopTime = 1f;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(BoomerangRoutine());
    }

    IEnumerator BoomerangRoutine()
    {
        rb.linearVelocity = transform.up * throwSpeed;
        yield return new WaitForSeconds(flyTime);

        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(stopTime);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Vector2 direction = (player.transform.position - transform.position).normalized;
            rb.linearVelocity = direction * returnSpeed;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        Destroy(gameObject, 5f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
            Destroy(gameObject);
        }
    }
}