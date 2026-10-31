using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Stage2Boss : MonoBehaviour
{
    public float maxHp = 200f;
    private float currentHp;

    public GameObject bulletPrefab;
    public Transform firePoint;

    [Header("Movement")]
    public float moveSpeed = 1f;
    public float moveDistance = 3f;
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

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }

        StartCoroutine(BossPatternRoutine());
    }

    void Update()
    {
        if (isDead) return;

        float newX = startPos.x + Mathf.Sin(Time.time * moveSpeed) * moveDistance;
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
    }

    IEnumerator BossPatternRoutine()
    {
        yield return new WaitForSeconds(1.5f);

        while (!isDead)
        {
            yield return StartCoroutine(PatternBurstShoot());
            yield return new WaitForSeconds(3.0f);

            yield return StartCoroutine(PatternSpiralShoot());
            yield return new WaitForSeconds(2.0f);

            yield return StartCoroutine(PatternShotgunShoot());
            yield return new WaitForSeconds(2.0f);
        }
    }

    IEnumerator PatternBurstShoot()
    {
        if (bulletPrefab == null || firePoint == null) yield break;

        int bulletCount = 18;
        float angleStep = 360f / bulletCount;

        for (int wave = 0; wave < 3; wave++)
        {
            for (int i = 0; i < bulletCount; i++)
            {
                float angle = i * angleStep + (wave * 10f);
                Quaternion rotation = Quaternion.Euler(0, 0, angle);
                Instantiate(bulletPrefab, firePoint.position, rotation);
            }

            yield return new WaitForSeconds(0.3f);
        }
    }

    IEnumerator PatternSpiralShoot()
    {
        if (bulletPrefab == null || firePoint == null) yield break;

        int bulletCount = 20;
        float currentAngle = 0f;

        for (int i = 0; i < bulletCount; i++)
        {
            Quaternion rotation = Quaternion.Euler(0, 0, currentAngle);
            Instantiate(bulletPrefab, firePoint.position, rotation);

            currentAngle += 25f;
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator PatternShotgunShoot()
    {
        if (bulletPrefab == null || firePoint == null) yield break;

        for (int wave = 0; wave < 3; wave++)
        {
            if (playerTransform == null || !playerTransform.gameObject.activeInHierarchy) break;

            Vector2 direction = playerTransform.position - firePoint.position;
            float baseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

            float[] spreadAngles = { -30f, 0f, 30f };

            foreach (float offset in spreadAngles)
            {
                Quaternion rotation = Quaternion.Euler(0, 0, baseAngle + offset);
                Instantiate(bulletPrefab, firePoint.position, rotation);
            }

            yield return new WaitForSeconds(0.6f);
        }
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