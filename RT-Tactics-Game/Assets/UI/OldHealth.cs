//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//public class OldHealth : MonoBehaviour
//{
    //script for health bar
  //  public int health;
    //Player pawn;
   // Slider healthBar;
    //public void init()
    //{
      //  pawn = GetComponent<Player>();
        //health = pawn.health;

        //currently, health bar is a slider, NEEDS CHANGING
        //set healthbar max to max hp
        //healthBar = GetComponentInChildren<Slider>();
        //healthBar.maxValue = health;
        //healthBar.value = health;

    //}
    //place holder for testing healthbar updating
    //needs to be integrated into GameManager to properly work
    //void updateHealth(int damage)
    //{
      //  health -= damage;
        //healthBar.value = health;
        //pawn.health = health;
    //}

    // Update is called once per frame
    //void Update()
    //{
      //  if(Input.GetKeyDown(KeyCode.F))
        //{
          //  updateHealth(1);
        //}
    //}
//}