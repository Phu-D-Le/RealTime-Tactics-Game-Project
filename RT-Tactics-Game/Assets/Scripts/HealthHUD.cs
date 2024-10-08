using UnityEngine;
using UnityEngine.UI;

// Basic slider updater. Pawn is where the health is changing. This is assigned to each Pawn and are children to small, heavy, etc. ZO
public class HealthHUD : MonoBehaviour
{
    public Slider healthBar;
    void Awake()
    {
        healthBar = this.GetComponentInChildren<Slider>();
    }
    public void SetHealthHUD(Pawn pawn)
    {
        healthBar.maxValue = pawn.maxHP;
        healthBar.value = pawn.currentHP;
    }
    public void SetHP(int hp)
    {
        healthBar.value = hp;
    }
}
