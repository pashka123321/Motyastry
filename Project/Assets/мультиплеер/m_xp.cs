using UnityEngine;
using System.Collections;
using Mirror;

public class mPlayerHealth : NetworkBehaviour
{
    public float maxHealth = 100f;
    
    [SyncVar] private float currentHealth; // Поле для синхронизации здоровья между сервером и клиентами
    [SyncVar] private bool isAlive = true; // Поле для состояния "живой/мертвый" игрока

    public HealthBar healthBar;
    public AudioClip[] hurtSounds;
    public AudioClip[] deathSounds;
    private AudioSource audioSource;

    public CameraFollow cameraFollow;
    private PlayerMovement playerMovement;
    
    public bool IsAlive => isAlive; // Открытое свойство для доступа к состоянию игрока
    public bool isInvincible = false;

    public override void OnStartServer()
    {
        base.OnStartServer();
        currentHealth = maxHealth;
    }

    void Start()
    {
        if (isLocalPlayer)
        {
            if (healthBar != null)
            {
                healthBar.SetHealth(currentHealth, maxHealth);
            }

            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }

            if (cameraFollow == null)
            {
                cameraFollow = Camera.main.GetComponent<CameraFollow>();
            }
        }

        playerMovement = GetComponent<PlayerMovement>();
    }

    [Command]
    public void CmdTakeDamage(float damage)
    {
        if (isInvincible || !isAlive) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        RpcUpdateHealth(currentHealth);

        if (currentHealth <= 0f)
        {
            Die();
            isInvincible = true;
            Invoke(nameof(InvincibleTimer), 1.5f);
        }
        else
        {
            isInvincible = true;
            Invoke(nameof(InvincibleTimer), 0.25f);
        }
    }

    [ClientRpc]
    private void RpcUpdateHealth(float newHealth)
    {
        currentHealth = newHealth;
        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth, maxHealth);
        }
    }

    private void InvincibleTimer()
    {
        isInvincible = false;
    }

    private void Die()
    {
        isAlive = false;
        RpcDieEffects();

        StartCoroutine(Respawn());
    }

    [ClientRpc]
    private void RpcDieEffects()
    {
        if (!isLocalPlayer) return;

        PlayRandomDeathSound();
        if (cameraFollow != null)
        {
            cameraFollow.StopCamera();
        }

        if (playerMovement != null)
        {
            playerMovement.canControl = false;
        }
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(1f);

        GameObject respawnCore = GameObject.FindWithTag("Core");
        if (respawnCore != null)
        {
            transform.position = respawnCore.transform.position;
            currentHealth = maxHealth;
            RpcUpdateHealth(currentHealth);
            isAlive = true;

            RpcRespawnEffects();
        }
        else
        {
            Debug.LogWarning("Respawn point not set!");
        }
    }

    [ClientRpc]
    private void RpcRespawnEffects()
    {
        if (!isLocalPlayer) return;

        if (cameraFollow != null)
        {
            cameraFollow.ResumeCamera();
        }

        if (playerMovement != null)
        {
            playerMovement.canControl = true;
        }
    }

    void PlayRandomDeathSound()
    {
        if (deathSounds.Length > 0)
        {
            int randomIndex = Random.Range(0, deathSounds.Length);
            AudioClip deathSound = deathSounds[randomIndex];
            audioSource.PlayOneShot(deathSound);
        }
    }
}
