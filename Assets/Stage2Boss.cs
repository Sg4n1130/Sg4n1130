using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Stage2Boss : MonoBehaviour
{
    public float maxHp = 200f;
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
        yield return new WaitForSeconds(1.5f);

        while (!isDead)
        {
            yield return StartCoroutine(PatternBurstShoot());
            yield return new WaitForSeconds(1.0f);

            ShootBasic();
            yield return new WaitForSeconds(1.5f);

            yield return StartCoroutine(PatternSpiralShoot());
            yield return new WaitForSeconds(1.0f);

            ShootBasic();
            yield return new WaitForSeconds(1.5f);

            yield return StartCoroutine(PatternShotgunShoot());
            yield return new WaitForSeconds(1.0f);

            ShootBasic();
            yield return new WaitForSeconds(1.5f);
        }
    }

    void ShootBasic()
    {
        if (bulletPrefab == null || firePoint == null || playerTransform == null || !playerTransform.gameObject.activeInHierarchy) return;

        Vector2 direction = playerTransform.position - firePoint.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        Instantiate(bulletPrefab, firePoint.position, rotation);
    }

    IEnumerator PatternBurstShoot()
    {
        if (bulletPrefab == null || firePoint == null) yield break;

        int bulletCount = 12;
        float angleStep = 360f / bulletCount;

        for (int wave = 0; wave < 2; wave++)
        {
            for (int i = 0; i < bulletCount; i++)
            {
                float angle = i * angleStep + (wave * 15f);
                Quaternion rotation = Quaternion.Euler(0, 0, angle);
                Instantiate(bulletPrefab, firePoint.position, rotation);
            }

            yield return new WaitForSeconds(0.4f);
        }
    }

    IEnumerator PatternSpiralShoot()
    {
        if (bulletPrefab == null || firePoint == null) yield break;

        int bulletCount = 12;
        float currentAngle = 0f;

        for (int i = 0; i < bulletCount; i++)
        {
            Quaternion rotation = Quaternion.Euler(0, 0, currentAngle);
            Instantiate(bulletPrefab, firePoint.position, rotation);

            currentAngle += 30f;
            yield return new WaitForSeconds(0.15f);
        }
    }

    IEnumerator PatternShotgunShoot()
    {
        if (bulletPrefab == null || firePoint == null) yield break;

        for (int wave = 0; wave < 2; wave++)
        {
            if (playerTransform == null || !playerTransform.gameObject.activeInHierarchy) break;

            Vector2 direction = playerTransform.position - firePoint.position;
            float baseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

            float[] spreadAngles = { -25f, 0f, 25f };

            foreach (float offset in spreadAngles)
            {
                Quaternion rotation = Quaternion.Euler(0, 0, baseAngle + offset);
                Instantiate(bulletPrefab, firePoint.position, rotation);
            }

            yield return new WaitForSeconds(0.8f);
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