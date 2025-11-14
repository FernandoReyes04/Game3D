using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Invencibilidad")]
    public float invincibleTime = 0.7f;
    private bool isInvincible = false;

    [Header("UI (opcional)")]
    public Slider healthBar;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateUI();
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log($"PLAYER recibió {damage} daño. Vida: {currentHealth}/{maxHealth}");

        UpdateUI();

        if (currentHealth <= 0)
            Die();
        else
            StartCoroutine(InvincibilityCoroutine());
    }

    IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibleTime);
        isInvincible = false;
    }

    void UpdateUI()
    {
        if (healthBar != null)
            healthBar.value = (float)currentHealth / maxHealth;
    }

    void Die()
    {
        Debug.Log("PLAYER murió 😵");

        // Aquí puedes recargar escena, pantalla de game over, etc.
        // Ejemplo:
        // UnityEngine.SceneManagement.SceneManager.LoadScene("GameOver");
    }
}
