using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    //List<Pawn> = FindObjectsOfType<Pawn>;
    /*
     * 
     * would need to reference map tiles
     * needs a ui to indicate where to move player pawns and send intructions from, I have no clue who's doing this and god help me if I'm doing it
     * 
     * List pawnList
     */
    void Start()
    {
        /*
         * get all pawns, perhaps get pawns by tag?
         * 
         * sort pawns by speed, would need another criteria for speed ties
         * 
         */

    }
    void Update()
    {
        /*
         * go through pawn list and execute their movement
         * 
         * depending on where pawns move, activate tile effect
         * cannot let pawns move into or over each other
         * if pawn meets enemy, call pawn attack
         * 
         */
    }
}
