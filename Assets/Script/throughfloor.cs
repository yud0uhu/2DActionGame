using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class throughfloor : MonoBehaviour
{
    BoxCollider2D box;
    // Start is called before the first frame update
    void Start()
    {
        box = this.GetComponent<BoxCollider2D>();
    }

    //上に立っている時に下入力をしたらすり抜ける
    void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.tag == "Player"&&Input.GetAxisRaw("Vertical") == -1)
        {
            box.isTrigger = true;
        }
    }

    //離れた時に衝突判定を復活させる
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            box.isTrigger = false;
        }
    }
}
