using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Stage1Boss : MonoBehaviour
{
    public float maxHp = 100f;
    private float currentHp;

    public Sprite phase2Sprite;
    public GameObject bulletPrefab;
    public Transform firePoint;

    public float bulletSpeed = 15f;
    public float phase2BulletSpeed = 25f;

    public float moveSpeed = 1f;
    public float phase2MoveSpeed = 2f;
    public float moveDistance = 2.5f;

    private Vector3 startPos;
    private bool isDead = false;
    private bool isPhase2 = false;

    private Rigidbody2D rb;
    private SpriteRenderer sr;

    void Start()
    {
        currentHp = maxHp;
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        rb.bodyType = RigidbodyType2D.Kinematic;

        startPos = transform.position;

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
        yield return new WaitForSeconds(1f);

        while (!isDead)
        {
            yield return StartCoroutine(PatternTargetShoot());
            yield return new WaitForSeconds(isPhase2 ? 0.5f : 1.0f);

            yield return StartCoroutine(PatternCircleShoot());
            yield return new WaitForSeconds(isPhase2 ? 0.5f : 1.0f);
        }
    }

    IEnumerator PatternTargetShoot()
    {
        if (bulletPrefab == null || firePoint == null) yield break;

        int shotCount = isPhase2 ? 5 : 3;

        for (int i = 0; i < shotCount; i++)
        {
            GameObject currentPlayer = GameObject.FindGameObjectWithTag("Player");

            if (currentPlayer != null && currentPlayer.activeInHierarchy)
            {
                Vector2 direction = currentPlayer.transform.position - firePoint.position;
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

        int bulletCount = isPhase2 ? 18 : 12;
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