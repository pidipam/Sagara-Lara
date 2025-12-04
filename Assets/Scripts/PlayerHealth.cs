
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 5;
    public int currentHealth;
    public HeartUI heartUI;
    public GameOverManager gameOverScreen;

    private int comboCounter = 0;
    private string[] lowHPSounds = { "Heartbeat", "EarRingingHighPitch" };

    void Start()
    {
        currentHealth = maxHealth;
        heartUI.UpdateHearts(currentHealth);

        if (AudioManager.instance != null)
        {
            UpdateLowHpVolume();
            foreach (string lowHPSound in lowHPSounds)
            {
                AudioManager.instance.Play(lowHPSound);

            }
        }
        UpdateLowHpVolume();
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth < 0) currentHealth = 0;

        heartUI.UpdateHearts(currentHealth);
        UpdateLowHpVolume();

        if (AudioManager.instance != null)
        {
            AudioManager.instance.Play("LampuPecah");
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (AudioManager.instance != null)
        {
            foreach (string lowHPSound in lowHPSounds)
            {
                AudioManager.instance.Stop(lowHPSound);
            }
        }

        Debug.Log("GAME OVER!");
        gameOverScreen.ShowGameOver();
    }

    public void RegisterKill()
    {
        comboCounter++;

        if (comboCounter >= 3)
        {
            Heal(1);
            comboCounter = 0;
        }
    }

    private void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;

        heartUI.UpdateHearts(currentHealth);
        Debug.Log("Heal! Current HP: " + currentHealth);
        UpdateLowHpVolume();
    }

    public void ResetCombo()
    {
        comboCounter = 0;
    }

    private void UpdateLowHpVolume()
    {
        if (AudioManager.instance == null || lowHPSounds == null) return;
        float hpRatio = (float)currentHealth / maxHealth;
        float volume = 1f - hpRatio;

        foreach (string lowHPSound in lowHPSounds)
        {
            float finalVolume = volume;
            if (lowHPSound == "EarRingingHighPitch")
            {
                if (hpRatio > 0.3f)
                {
                    finalVolume = 0f;
                }
            }
            AudioManager.instance.SetVolume(lowHPSound, finalVolume);
        }
    }
}
