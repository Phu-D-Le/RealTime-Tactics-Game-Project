using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    //script for health bar
    public int health;
    Player pawn;
    //Canvas canvas;
    //Slider healthBar;
    public void init()
    {
        pawn = GetComponent<Player>();
        health = pawn.health;

        //currently, health bar is a slider, would probably need changing later
        //set healthbar max to max hp
        GetComponentInChildren<Slider>().maxValue = health;
        GetComponentInChildren<Slider>().value = health;
        //canvas = GetComponent<Canvas>(); 

        //healthBar = canvas.GetComponent<Slider>();
        //healthBar.maxValue = health;

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
