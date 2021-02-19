using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantCtrl : MonoBehaviour
{
    private Animator anim;
    public GameObject  player;

    // Start is called before the first frame update
    void Start()
    {
        this.anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 pPos = player.transform.position;
        Vector2 myPos = this.transform.position;
        float distance = Vector2.Distance( pPos , myPos );

        if ( distance < 1.5 & ( ( pPos.y - myPos.y )<1 ) ) {
            anim.SetTrigger("TrgAttack");
        }
    }
}
