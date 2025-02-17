using UnityEngine;
using Mirror;
using System.Linq;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class EnemyAI : NetworkBehaviour
{
    public Transform player; // Цель, если она задана
    public float speed = 2.0f;
    public float minDesiredDistance = 10.0f;
    public float maxDesiredDistance = 15.0f;
    public float retreatDistance = 8.0f;
    public bool isClone = false;
    public int damage = 10;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 1.0f;
    public float bulletSpeed = 10.0f;
    public float rotationSpeed = 200.0f;
    public float shootAngleThreshold = 5.0f;
    public float acceleration = 5.0f;
    public float deceleration = 5.0f;
    public float floatAmplitude = 0.5f;
    public float floatFrequency = 1.0f;

    private float fireTimer;
    private Rigidbody2D rb;
    private Vector2 currentVelocity;
    private Vector2 recoilForce;
    private float recoilDuration = 0.2f;
    private float recoilTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        fireTimer = 0.0f;
        currentVelocity = Vector2.zero;
        recoilForce = Vector2.zero;
        recoilTimer = 0.0f;

        CheckIfClone();
    }

    void Update()
    {
        CheckIfClone();

        // Если цель не задана, ищем ближайшего игрока
        if (player == null)
        {
            FindNearestPlayer();
        }
    }

    void CheckIfClone()
    {
        if (gameObject.name.Contains("Clone"))
        {
            isClone = true;
        }
    }

    void FindNearestPlayer()
    {
        // Находим все объекты с компонентом NetworkIdentity, представляющие игроков
        var players = GameObject.FindGameObjectsWithTag("Playerataka")
            .Select(obj => obj.transform)
            .Where(transform => transform != null)
            .ToArray();

        // Ищем ближайшего игрока
        if (players.Length > 0)
        {
            player = players
                .OrderBy(p => Vector3.Distance(transform.position, p.position))
                .FirstOrDefault();
        }
    }

    void FixedUpdate()
    {
        if (isClone && player != null)
        {
            Vector3 direction = player.position - transform.position;
            float distanceToPlayer = direction.magnitude;
            Vector2 targetVelocity = Vector2.zero;

            float floatOffset = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;

            if (distanceToPlayer < retreatDistance)
            {
                targetVelocity = -direction.normalized * speed;
            }
            else if (distanceToPlayer >= minDesiredDistance && distanceToPlayer <= maxDesiredDistance)
            {
                targetVelocity = new Vector2(0, floatOffset);
            }
            else if (distanceToPlayer < minDesiredDistance)
            {
                targetVelocity = -direction.normalized * speed;
            }
            else
            {
                targetVelocity = new Vector2(direction.normalized.x, direction.normalized.y) * speed + new Vector2(0, floatOffset);
            }

            currentVelocity = Vector2.Lerp(currentVelocity, targetVelocity, (targetVelocity == Vector2.zero ? deceleration : acceleration) * Time.fixedDeltaTime);

            if (recoilTimer > 0)
            {
                recoilTimer -= Time.fixedDeltaTime;
                rb.velocity = recoilForce;
            }
            else
            {
                rb.velocity = currentVelocity;
            }

            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 0f;
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.z, targetAngle, rotationSpeed * Time.fixedDeltaTime);
            rb.MoveRotation(angle);

            float angleDifference = Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.z, targetAngle));
            if (angleDifference < shootAngleThreshold && distanceToPlayer <= maxDesiredDistance)
            {
                fireTimer += Time.fixedDeltaTime;
                if (fireTimer >= 1.0f / fireRate)
                {
                    Fire();
                    fireTimer = 0.0f;
                }
            }
        }
    }

    void Fire()
    {
        GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bulletObj.transform.Rotate(0, 0, -90);
        Rigidbody2D bulletRB = bulletObj.GetComponent<Rigidbody2D>();
        if (bulletRB != null)
        {
            bulletRB.velocity = firePoint.right * bulletSpeed;
        }
        Bullet bullet = bulletObj.GetComponent<Bullet>();
        if (bullet != null)
        {
            bullet.damage = damage;
        }
    }

    public void ApplyRecoil(Vector2 force)
    {
        recoilForce = force;
        recoilTimer = recoilDuration;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Vector2 repelDirection = (transform.position - collision.transform.position).normalized;
            rb.AddForce(repelDirection * speed, ForceMode2D.Impulse);
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }

    public void TakeDamage(int damageAmount, Vector2 impactDirection)
    {
        ApplyRecoil(impactDirection * 2.0f);
    }
}
