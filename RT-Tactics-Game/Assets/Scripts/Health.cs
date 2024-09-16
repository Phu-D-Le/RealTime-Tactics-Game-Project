using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    private int currentHealth;
    private Slider healthBar;
    private GameManager gameManager;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    public void Initialize(int baseHealth)
    {
        currentHealth = baseHealth;

        healthBar = GetComponentInChildren<Slider>();
        if (healthBar != null)
        {
            healthBar.maxValue = currentHealth;
            healthBar.value = currentHealth;
        }
    }

    private void Update()
    {
        // Check if 'F' key is pressed
        if (Input.GetKeyDown(KeyCode.F))
        {
            GameObject selectedPawn = gameManager.GetSelectedPawn();
            if (selectedPawn == gameObject) // Check if this is the selected pawn
            {
                Debug.Log($"'F' key pressed. Decreasing health of {gameObject.name}.");
                UpdateHealth(1);
            }
        }
    }

    public void UpdateHealth(int damage)
    {
        currentHealth -= damage;
        if (healthBar != null)
        {
            healthBar.value = currentHealth;
        }

        if (currentHealth <= 0)
        {
            HandleDeath();
        }
    }

    private void HandleDeath()
    {
        Debug.Log($"{gameObject.name} is dead!");
        Destroy(gameObject); // Destroy the pawn
    }
}

