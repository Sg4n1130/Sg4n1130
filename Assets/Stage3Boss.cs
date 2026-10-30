using UnityEngine;
using System.Collections;

public class Stage3Boss : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject normalBulletPrefab;
    public GameObject stalkerMissilePrefab;
    public GameObject fastLaserPrefab;
    public GameObject boomerangPrefab;

    public Transform firePoint;
    public float normalBulletSpeed = 10f;

    private Transform player;

    void Start()
    {
        GameObject target = GameObject.FindGameObjectWithTag("Player");
        if (target != null) player = target.transform;

        StartCoroutine(BossPatternLoop());
    }

    IEnumerator BossPatternLoop()
    {
        yield return new WaitForSeconds(2f);

        while (true)
        {
            yield return StartCoroutine(BasicAttack());
            yield return new WaitForSeconds(1.5f);

            yield return StartCoroutine(Pattern1_StalkerMissile());
            yield return new WaitForSeconds(2f);

            yield return StartCoroutine(BasicAttack());
            yield return new WaitForSeconds(1.5f);

            yield return StartCoroutine(Pattern2_TelegraphedSnipe());
            yield return new WaitForSeconds(3f);

            yield return StartCoroutine(BasicAttack());
            yield return new WaitForSeconds(1.5f);

            yield return StartCoroutine(Pattern3_Boomerang());
            yield return new WaitForSeconds(4f);
        }
    }

    IEnumerator BasicAttack()
    {
        if (player == null) yield break;

        for (int i = 0; i < 3; i++)
        {
            if (player == null) break;

            Vector2 direction = (Vector2)player.position - (Vector2)firePoint.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            Quaternion rotation = Quaternion.Euler(0, 0, angle);

            GameObject bullet = Instantiate(normalBulletPrefab, firePoint.position, rotation);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

            if (rb != null) rb.linearVelocity = bullet.transform.up * normalBulletSpeed;
            Destroy(bullet, 5f);

            yield return new WaitForSeconds(0.2f);
        }
    }

    IEnumerator Pattern1_StalkerMissile()
    {
        if (player == null) yield break;

        GameObject missile = Instantiate(stalkerMissilePrefab, firePoint.position, Quaternion.identity);

        BossHomingMissile script = missile.GetComponent<BossHomingMissile>();
        if (script != null)
        {
            script.speed = 4.5f;
            script.rotateSpeed = 90f;
            script.lifeTime = 12f;
        }
        yield return null;
    }

    IEnumerator Pattern2_TelegraphedSnipe()
    {
        if (player == null) yield break;

        Vector2 direction = (Vector2)player.position - (Vector2)firePoint.position;
        float baseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

        float[] angles = { -80f, -60f, -40f, -20f, 0f, 20f, 40f, 60f, 80f };

        for (int i = 0; i < 9; i++)
        {
            Quaternion rotation = Quaternion.Euler(0, 0, baseAngle + angles[i]);
            Instantiate(fastLaserPrefab, firePoint.position, rotation);
        }

        yield return new WaitForSeconds(2f);
    }

    IEnumerator Pattern3_Boomerang()
    {
        if (player == null) yield break;

        Vector2 direction = (Vector2)player.position - (Vector2)firePoint.position;
        float baseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

        float[] angles = { -40f, -20f, 0f, 20f, 40f };

        for (int i = 0; i < 5; i++)
        {
            Quaternion rotation = Quaternion.Euler(0, 0, baseAngle + angles[i]);
            Instantiate(boomerangPrefab, firePoint.position, rotation);
        }

        yield return null;
    }
}