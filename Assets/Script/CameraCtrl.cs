using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCtrl : MonoBehaviour
{

    // public GameObject player; 

    // Update is called once per frame
    // void Update()
    // {
    //     transform.position = new Vector3(
    //         player.transform.position.x,
    //         this.player.transform.position.y,
    //         this.player.transform.position.z
    //     );

    //     if(this.transform.position.x < 0){
    //         transform.position = new Vector3(
    //             0,
    //             this.transform.position.y,
    //             this.transform.position.z
    //         );
    //     }
    // }


   void Update () {
        if (Input.GetKeyDown (KeyCode.UpArrow)) 
        {
            transform.Translate (0, 0, 1);
        }else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            transform.Translate(1, 0, 0);
        }else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            transform.Translate(0, 0, -1);
        }else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            transform.Translate(-1, 0, 0);
        }    
   }
}
