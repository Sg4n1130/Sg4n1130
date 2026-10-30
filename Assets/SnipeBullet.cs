using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(SpriteRenderer))]
public class SnipeBullet : MonoBehaviour
{
    public float damage = 10f;
    public float bulletSpeed = 40f;
    public float warningTime = 1.5f;

    private Rigidbody2D rb;
    private LineRenderer lr;
    private SpriteRenderer sr;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lr = GetComponent<LineRenderer>();
        sr = GetComponent<SpriteRenderer>();
        StartCoroutine(SnipeRoutine());
    }

    IEnumerator SnipeRoutine()
    {
        sr.enabled = false;

        lr.positionCount = 2;
        lr.startWidth = 0.05f;
        lr.endWidth = 0.05f;

        float timer = 0f;
        bool isBlinking = true;

        while (timer < warningTime)
        {
            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, transform.position + transform.up * 50f);

            lr.enabled = isBlinking;
            isBlinking = !isBlinking;

            yield return new WaitForSeconds(0.1f);
            timer += 0.1f;
        }

        lr.enabled = false;
        sr.enabled = true;
        rb.linearVelocity = transform.up * bulletSpeed;

        Destroy(gameObject, 3f);
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