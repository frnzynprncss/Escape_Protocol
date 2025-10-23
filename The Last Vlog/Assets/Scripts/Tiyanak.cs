using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tiyanak : MonoBehaviour
{
   void OnTriggerEnter2D(Collider2D col){
    if(col.gameObject.CompareTag("Player")){
        print("JumpScare");
    }
   }
}
