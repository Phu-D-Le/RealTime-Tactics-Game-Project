using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTurn : MonoBehaviour
{
    public bool curr { get; private set; }
    public Players playerType;
    public enum Players //Declare players
    {
        First,
        Second,
        Robot
    }
    public void Flow(Players type)
    {
        playerType = type;
        switch (type)
        {
            case Players.First:
                curr = true;
                break;
            case Players.Second:
                curr = false;
                break;
            case Players.Robot:
                curr = false;
                break;
        }
    }

    public void ChangeFlow()
    {
        curr = !curr;
    }
}
