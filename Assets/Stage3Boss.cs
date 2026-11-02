using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
public class Stage3Boss : MonoBehaviour
{
    public float maxHp = 300f;
    private float currentHp;

    public Sprite phase2Sprite;
    public GameObject normalBulletPrefab;
    public GameObject stalkerMissilePrefab;
    public GameObject fastLaserPrefab;
    public GameObject boomerangPrefab;

    public Transform firePoint;

    public float normalBulletSpeed = 15f;
    public float phase2BulletSpeed = 25f;

    public float moveSpeed = 1.2f;
    public float phase2MoveSpeed = 3f;
    public float moveDistance = 3.5f;

    private Transform player;
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
        StartCoroutine(BossPatternLoop());
    }

    void Update()
    {
        if (isDead) return;

        if (Input.GetKeyDown(KeyCode.F2))
        {
            if (!isPhase2)
            {
                TakeDamage(currentHp - (maxHp / 2f));
            }
        }

        float newX = startPos.x + Mathf.Sin(Time.time * moveSpeed) * moveDistance;
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
    }

    void EnterPhase2()
    {
        isPhase2 = true;
        if (sr != null && phase2Sprite != null) sr.sprite = phase2Sprite;

        normalBulletSpeed = phase2BulletSpeed;
        moveSpeed = phase2MoveSpeed;
    }

    IEnumerator BossPatternLoop()
    {
        yield return new WaitForSeconds(1.5f);

        while (!isDead)
        {
            yield return StartCoroutine(BasicAttack());
            yield return new WaitForSeconds(isPhase2 ? 0.4f : 0.8f);

            yield return StartCoroutine(Pattern1_StalkerMissile());
            yield return new WaitForSeconds(isPhase2 ? 1f : 1.5f);

            yield return StartCoroutine(BasicAttack());
            yield return new WaitForSeconds(isPhase2 ? 0.4f : 0.8f);

            yield return StartCoroutine(Pattern2_TelegraphedSnipe());
            yield return new WaitForSeconds(isPhase2 ? 1f : 2f);

            yield return StartCoroutine(BasicAttack());
            yield return new WaitForSeconds(isPhase2 ? 0.4f : 0.8f);

            yield return StartCoroutine(Pattern3_Boomerang());
            yield return new WaitForSeconds(isPhase2 ? 1.5f : 2.5f);
        }
    }

    void FindPlayer()
    {
        GameObject target = GameObject.FindGameObjectWithTag("Player");
        if (target != null) player = target.transform;
    }

    IEnumerator BasicAttack()
    {
        int shotCount = isPhase2 ? 8 : 5;

        for (int i = 0; i < shotCount; i++)
        {
            if (player == null || !player.gameObject.activeInHierarchy) FindPlayer();
            if (player == null) break;

            Vector2 direction = (Vector2)player.position - (Vector2)firePoint.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            Quaternion rotation = Quaternion.Euler(0, 0, angle);

            GameObject bullet = Instantiate(normalBulletPrefab, firePoint.position, rotation);
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();

            if (bulletRb != null) bulletRb.linearVelocity = bullet.transform.up * normalBulletSpeed;
            Destroy(bullet, 3f);

            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator Pattern1_StalkerMissile()
    {
        float[] spawnAngles = isPhase2 ? new float[] { -45f, -15f, 15f, 45f } : new float[] { -30f, 30f };

        for (int i = 0; i < spawnAngles.Length; i++)
        {
            Quaternion rot = Quaternion.Euler(0, 0, spawnAngles[i]);
            GameObject missile = Instantiate(stalkerMissilePrefab, firePoint.position, rot);

            BossHomingMissile script = missile.GetComponent<BossHomingMissile>();
            if (script != null)
            {
                script.speed = isPhase2 ? 9f : 6f;
                script.rotateSpeed = isPhase2 ? 200f : 150f;
            }
        }
        yield return null;
    }

    IEnumerator Pattern2_TelegraphedSnipe()
    {
        if (player == null || !player.gameObject.activeInHierarchy) FindPlayer();
        if (player == null) yield break;

        Vector2 direction = (Vector2)player.position - (Vector2)firePoint.position;
        float baseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

        float[] angles = isPhase2 ?
            new float[] { -100f, -80f, -60f, -40f, -20f, -10f, 0f, 10f, 20f, 40f, 60f, 80f, 100f } :
            new float[] { -100f, -80f, -60f, -40f, -20f, 0f, 20f, 40f, 60f, 80f, 100f };

        List<GameObject> warningLines = new List<GameObject>();
        for (int i = 0; i < angles.Length; i++)
        {
            GameObject lineObj = new GameObject("WarningLine");
            lineObj.transform.position = firePoint.position;
            lineObj.transform.rotation = Quaternion.Euler(0, 0, baseAngle + angles[i]);

            LineRenderer lr = lineObj.AddComponent<LineRenderer>();
            lr.startWidth = 0.05f;
            lr.endWidth = 0.05f;
            lr.material = new Material(Shader.Find("Sprites/Default"));
            lr.startColor = new Color(1f, 0f, 0f, 0.6f);
            lr.endColor = new Color(1f, 0f, 0f, 0f);

            lr.SetPosition(0, firePoint.position);
            lr.SetPosition(1, firePoint.position + lineObj.transform.up * 30f);

            warningLines.Add(lineObj);
        }

        yield return new WaitForSeconds(0.6f);

        for (int i = 0; i < warningLines.Count; i++)
        {
            if (warningLines[i] != null)
            {
                GameObject laserBullet = Instantiate(fastLaserPrefab, firePoint.position, warningLines[i].transform.rotation);
                Rigidbody2D rb2d = laserBullet.GetComponent<Rigidbody2D>();

                float fastSpeed = isPhase2 ? 45f : 30f;
                if (rb2d != null) rb2d.linearVelocity = laserBullet.transform.up * fastSpeed;

                Destroy(laserBullet, 3f);
                Destroy(warningLines[i]);
            }
        }
    }

    IEnumerator Pattern3_Boomerang()
    {
        if (player == null || !player.gameObject.activeInHierarchy) FindPlayer();
        if (player == null) yield break;

        Vector2 direction = (Vector2)player.position - (Vector2)firePoint.position;
        float baseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

        float[] angles = isPhase2 ?
            new float[] { -75f, -50f, -25f, 0f, 25f, 50f, 75f } :
            new float[] { -60f, -40f, -20f, 0f, 20f, 40f, 60f };

        for (int i = 0; i < angles.Length; i++)
        {
            Quaternion rotation = Quaternion.Euler(0, 0, baseAngle + angles[i]);
            GameObject boomerang = Instantiate(boomerangPrefab, firePoint.position, rotation);

            Rigidbody2D boomerangRb = boomerang.GetComponent<Rigidbody2D>();
            if (boomerangRb != null) boomerangRb.linearVelocity = boomerang.transform.up * normalBulletSpeed;
            Destroy(boomerang, 5f);
        }

        yield return null;
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