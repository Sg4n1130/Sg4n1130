using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Stage2Boss : MonoBehaviour
{
    public float maxHp = 200f;
    private float currentHp;

    public Sprite phase2Sprite;
    public GameObject bulletPrefab;
    public Transform firePoint;

    public float bulletSpeed = 10f;
    public float phase2BulletSpeed = 18f;

    public float moveSpeed = 1f;
    public float phase2MoveSpeed = 2f;
    public float moveDistance = 3f;

    private Vector3 startPos;
    private bool isDead = false;
    private bool isPhase2 = false;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Transform playerTransform;

    void Start()
    {
        currentHp = maxHp;
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        rb.bodyType = RigidbodyType2D.Kinematic;

        startPos = transform.position;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) playerTransform = playerObj.transform;

        StartCoroutine(BossPatternRoutine());
    }

    void Update()
    {
        if (isDead) return;

        if (Input.GetKeyDown(KeyCode.F2))
        {
            if (!isPhase2)
            {
                TakeDamage(currentHp - (maxHp / 2f));
                Debug.Log("Cheat: Phase 2 Start");
            }
        }

        float newX = startPos.x + Mathf.Sin(Time.time * moveSpeed) * moveDistance;
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
    }

    void EnterPhase2()
    {
        isPhase2 = true;
        if (sr != null && phase2Sprite != null) sr.sprite = phase2Sprite;

        bulletSpeed = phase2BulletSpeed;
        moveSpeed = phase2MoveSpeed;
    }

    IEnumerator BossPatternRoutine()
    {
        yield return new WaitForSeconds(1.5f);

        while (!isDead)
        {
            yield return StartCoroutine(PatternBurstShoot());
            yield return new WaitForSeconds(isPhase2 ? 1.5f : 3.0f);

            yield return StartCoroutine(PatternSpiralShoot());
            yield return new WaitForSeconds(isPhase2 ? 1.0f : 2.0f);

            yield return StartCoroutine(PatternShotgunShoot());
            yield return new WaitForSeconds(isPhase2 ? 1.0f : 2.0f);
        }
    }

    IEnumerator PatternBurstShoot()
    {
        if (bulletPrefab == null || firePoint == null) yield break;

        int bulletCount = isPhase2 ? 24 : 18;
        float angleStep = 360f / bulletCount;
        int waves = isPhase2 ? 5 : 3;

        for (int wave = 0; wave < waves; wave++)
        {
            for (int i = 0; i < bulletCount; i++)
            {
                float angle = i * angleStep + (wave * 10f);
                Quaternion rotation = Quaternion.Euler(0, 0, angle);
                GameObject bullet = Instantiate(bulletPrefab, firePoint.position, rotation);

                Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
                if (bulletRb != null) bulletRb.linearVelocity = bullet.transform.up * bulletSpeed;
                Destroy(bullet, 5f);
            }

            yield return new WaitForSeconds(0.3f);
        }
    }

    IEnumerator PatternSpiralShoot()
    {
        if (bulletPrefab == null || firePoint == null) yield break;

        int bulletCount = isPhase2 ? 40 : 20;
        float currentAngle = 0f;
        float angleIncrease = isPhase2 ? 15f : 25f;

        for (int i = 0; i < bulletCount; i++)
        {
            Quaternion rotation = Quaternion.Euler(0, 0, currentAngle);
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, rotation);

            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
            if (bulletRb != null) bulletRb.linearVelocity = bullet.transform.up * bulletSpeed;
            Destroy(bullet, 5f);

            currentAngle += angleIncrease;
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator PatternShotgunShoot()
    {
        if (bulletPrefab == null || firePoint == null) yield break;

        int waves = isPhase2 ? 5 : 3;

        for (int wave = 0; wave < waves; wave++)
        {
            if (playerTransform == null || !playerTransform.gameObject.activeInHierarchy)
            {
                GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj != null) playerTransform = playerObj.transform;
                else break;
            }

            Vector2 direction = playerTransform.position - firePoint.position;
            float baseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

            float[] spreadAngles = isPhase2 ? new float[] { -45f, -22.5f, 0f, 22.5f, 45f } : new float[] { -30f, 0f, 30f };

            foreach (float offset in spreadAngles)
            {
                Quaternion rotation = Quaternion.Euler(0, 0, baseAngle + offset);
                GameObject bullet = Instantiate(bulletPrefab, firePoint.position, rotation);

                Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
                if (bulletRb != null) bulletRb.linearVelocity = bullet.transform.up * bulletSpeed;
                Destroy(bullet, 5f);
            }

            yield return new WaitForSeconds(0.6f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) TakeDamage(1f);
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;
        currentHp -= damage;

        if (currentHp <= maxHp / 2f && !isPhase2)
        {
            EnterPhase2();
        }

        if (currentHp <= 0)
        {
            isDead = true;
            StopAllCoroutines();
            Destroy(gameObject);
        }
    }
}