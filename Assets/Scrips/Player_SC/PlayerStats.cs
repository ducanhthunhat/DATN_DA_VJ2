using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField]
    private float maxHealth;

    [SerializeField]
    private GameObject
        deathChunkParticle,
        deathBloodParticle;

    private float currentHealth;

    private GameManager GM;

    private void Start()
    {
        currentHealth = maxHealth;
        GM = FindObjectOfType<GameManager>();
        if (GM == null)
        {
            Debug.LogError("GameManager component not found in scene!");
        }
    }

    public void DecreaseHealth(float amount)
    {
        currentHealth -= amount;

        if(currentHealth <= 0.0f)
        {
            Die();
        }
    }

    private void Die()
    {
        if (deathChunkParticle != null)
            Instantiate(deathChunkParticle, transform.position, deathChunkParticle.transform.rotation);
        else
            Debug.LogWarning("Death chunk particle not assigned!");

        if (deathBloodParticle != null)
            Instantiate(deathBloodParticle, transform.position, deathBloodParticle.transform.rotation);
        else
            Debug.LogWarning("Death blood particle not assigned!");

        if (GM != null)
            GM.Respawn();
        else
            Debug.LogError("GameManager reference is null in Die()!");

        Destroy(gameObject);
    }
}
