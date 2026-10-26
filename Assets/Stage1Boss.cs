using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Stage1Boss : MonoBehaviour
{
    public float maxHp = 100f;
    private float currentHp;

    public GameObject bulletPrefab;
    public Transform firePoint;

    private bool isDead = false;
    private Rigidbody2D rb;
    private Transform playerTransform;

    void Start()
    {
        currentHp = maxHp;
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }

        StartCoroutine(BossPatternRoutine());
    }

    IEnumerator BossPatternRoutine()
    {
        yield return new WaitForSeconds(1f);

        while (!isDead)
        {
            yield return StartCoroutine(PatternTargetShoot());
            yield return new WaitForSeconds(3.0f);

            yield return StartCoroutine(PatternCircleShoot());
            yield return new WaitForSeconds(1.0f);
        }
    }

    IEnumerator PatternTargetShoot()
    {
        if (bulletPrefab == null || firePoint == null) yield break;

        for (int i = 0; i < 3; i++)
        {
            if (playerTransform == null || !playerTransform.gameObject.activeInHierarchy) yield break;

            Vector2 direction = playerTransform.position - firePoint.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            Quaternion rotation = Quaternion.Euler(0, 0, angle);

            Instantiate(bulletPrefab, firePoint.position, rotation);

            yield return new WaitForSeconds(0.5f);
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

            Instantiate(bulletPrefab, firePoint.position, rotation);
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