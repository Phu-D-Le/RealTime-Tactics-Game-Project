using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public PawnObject pawnData; // Reference to the ScriptableObject
    private int currentHealth;
    private Slider healthBar;

    public void Initialize(PawnObject pawn)
    {
        pawnData = pawn; // Assign the pawn data
        currentHealth = pawnData.Health; // Initialize health from the ScriptableObject

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
            Debug.Log($"'F' key pressed. Decreasing health of {pawnData.name}.");
            UpdateHealth(1); // Damage pawn by 1
        }
    }

    public void UpdateHealth(int damage)
    {
        currentHealth -= damage;
        pawnData.Health = currentHealth; // Update the ScriptableObject's health directly

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
        Debug.Log($"{pawnData.name} is dead!");
        Destroy(gameObject); // Destroy the pawn's GameObject
    }
}
