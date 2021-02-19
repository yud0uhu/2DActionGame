using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject player;
    public GameObject target;

    // Update is called once per frame
    void OnTriggerEnter2D(Collider2D colision)
    {
        Destroy(colision.gameObject);
//        if(collision.gameObject.tag == "Player") {
//            Debug.Log("onTriggerEnter2D" + other.gameObject.name);
            //target.transform.position = new Vector3(transform.position.x , transform.position.y + 0.2f , transform.position.z);
            //GetComponent<SpringJoint2D>().enabled = true;
            //target.GetComponent<SpringJoint2D>().enabled = true;
//        }
    }
}
