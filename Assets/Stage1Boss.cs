using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Stage1Boss : MonoBehaviour
{
    public float maxHp = 100f;
    private float currentHp;

    public GameObject bulletPrefab;
    public Transform firePoint;

    public float bulletSpeed = 15f;

    public float moveSpeed = 1f;
    public float moveDistance = 2.5f;
    private Vector3 startPos;

    private bool isDead = false;
    private Rigidbody2D rb;
    private Transform playerTransform;

    void Start()
    {
        currentHp = maxHp;
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;

        startPos = transform.position;

        FindPlayer();

        StartCoroutine(BossPatternRoutine());
    }

    void Update()
    {
        if (isDead) return;

        float newX = startPos.x + Mathf.Sin(Time.time * moveSpeed) * moveDistance;
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
    }

    void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
    }

    IEnumerator BossPatternRoutine()
    {
        yield return new WaitForSeconds(1f);

        while (!isDead)
        {
            yield return StartCoroutine(PatternTargetShoot());
            yield return new WaitForSeconds(1.0f);

            yield return StartCoroutine(PatternCircleShoot());
            yield return new WaitForSeconds(1.0f);
        }
    }

    IEnumerator PatternTargetShoot()
    {
        if (bulletPrefab == null || firePoint == null) yield break;

        for (int i = 0; i < 3; i++)
        {
            if (playerTransform == null || !playerTransform.gameObject.activeInHierarchy)
            {
                FindPlayer();
            }

            if (playerTransform != null && playerTransform.gameObject.activeInHierarchy)
            {
                Vector2 direction = playerTransform.position - firePoint.position;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
                Quaternion rotation = Quaternion.Euler(0, 0, angle);

                GameObject bullet = Instantiate(bulletPrefab, firePoint.position, rotation);

                Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
                if (bulletRb != null) bulletRb.linearVelocity = bullet.transform.up * bulletSpeed;
                Destroy(bullet, 5f);
            }

            yield return new WaitForSeconds(0.2f);
        }
    }

    IEnumerator PatternCircleShoot()
    {
        if (bulletPrefab == null || firePoint == null) yield break;

        int bulletCount = 12;
        float angleStep = 360f / bulletCount;

        for (int i = 0; i < bulletCount; i++)
        {
            float currentAngle = i * angleStep;
            Quaternion rotation = Quaternion.Euler(0, 0, currentAngle);

            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, rotation);

            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
            if (bulletRb != null) bulletRb.linearVelocity = bullet.transform.up * bulletSpeed;
            Destroy(bullet, 5f);
        }

        yield return null;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            TakeDamage(1f);
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;
        currentHp -= damage;

        if (currentHp <= 0)
        {
            isDead = true;
            StopAllCoroutines();
            Destroy(gameObject);
        }
    }
}