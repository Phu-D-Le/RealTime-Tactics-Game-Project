using System.Collections.Generic;
using UnityEngine;

public class AttackManager : MonoBehaviour
{
    private static Dictionary<GameObject, List<Attack>> playerAttacks = new Dictionary<GameObject, List<Attack>>();

    public static void InitializeDefaultAttack(GameObject player, PlayerType.Size type)
    {
        if (!playerAttacks.ContainsKey(player))
        {
            playerAttacks[player] = new List<Attack>();
        }

        List<Attack> learnedAttacks = playerAttacks[player];
        if (learnedAttacks.Count == 0)
        {
            Attack defaultAttack = GetDefaultAttack(type);
            if (defaultAttack != null)
            {
                learnedAttacks.Add(defaultAttack);
                Debug.Log($"{player.name} learned default attack: {defaultAttack.attackName}");
            }
        }
    }

    private static Attack GetDefaultAttack(PlayerType.Size type)
    {
        switch (type)
        {
            case PlayerType.Size.Small:
                return Resources.Load<Attack>("Attacks/ShortSwing");
            case PlayerType.Size.Medium:
                return Resources.Load<Attack>("Attacks/RangedAttack");
            case PlayerType.Size.Heavy:
                return Resources.Load<Attack>("Attacks/WideSwing");
            default:
                return null;
        }
    }

    public static List<Attack> GetLearnedAttacks(GameObject player)
    {
        if (playerAttacks.ContainsKey(player))
        {
            List<Attack> attacks = playerAttacks[player];
            Debug.Log($"Retrieved {attacks.Count} learned attacks for {player.name}");
            return attacks;
        }
        else
        {
            Debug.LogWarning($"No learned attacks found for {player.name}");
            return new List<Attack>();
        }
    }

    public static void LearnAttack(GameObject player, Attack newAttack)
    {
        if (playerAttacks.ContainsKey(player))
        {
            List<Attack> learnedAttacks = playerAttacks[player];
            if (learnedAttacks.Count < 3)
            {
                learnedAttacks.Add(newAttack);
                Debug.Log($"{player.name} learned {newAttack.attackName}");
            }
            else
            {
                Debug.LogWarning($"{player.name} already has maximum attacks.");
            }
        }
        else
        {
            Debug.LogWarning($"No attack list found for {player.name}");
        }
    }


    public static List<Attack> GetAvailableAttacksForTurn(PlayerType.Size type, int turn)
    {
        List<Attack> availableAttacks = new List<Attack>();

        if (turn % 3 == 0)
        {
        // Check and add attacks based on type and turn, ensure these attacks exist in Resources/Attacks
            string path = $"Attacks/{type}/{turn}";
            Attack[] attacksArray = Resources.LoadAll<Attack>(path);

            availableAttacks.AddRange(attacksArray);
        }

        return availableAttacks;
    }
}
