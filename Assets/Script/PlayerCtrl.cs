using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{

    public float speed = 5; // publicで変数を作るとUnity上でパラメータをかえられます
    public float jumpForce = 900f;
    public LayerMask groundLayer;

    private Rigidbody2D rd2d;
    private Animator anim;
    private SpriteRenderer spRenderer;

    private bool isGround; // ジャンプフラグを返す変数
    private bool isSloped;
    private bool isDead = false; // 死亡フラグ

    // Start is called before the first frame update
    void Start()
    {
        // コンストラクタで初期化する
        this.rd2d = GetComponent<Rigidbody2D>();
        this.anim = GetComponent<Animator>();
        this.spRenderer = GetComponent<SpriteRenderer>();

        Sound.LoadSe("gameover","img11");
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal"); // Aキー-1・なにもしない0・Dキーで1が帰ってくる関数

        // スプライトの向きを変える
        if ( x < 0 ) {
            spRenderer.flipX = true;
        } else if ( x > 0 ) {
            spRenderer.flipX = false;
        }

        if(isDead==false){
            rd2d.AddForce( Vector2.right * x * speed );　// 2Dベクトルの右方向に速度変数を掛けている(横方向に力を加えている)
        }
        anim.SetFloat("Speed", Mathf.Abs( x * speed )); // 歩くアニメーション

        // Translate(物理演算無視)でvelocity(移動量)ゼロになる
        if(isSloped){
            this.gameObject.transform.Translate( 0.1f * x, 0.0f, 0.0f );
        }

        if ( Input.GetButtonDown("Jump") & isGround==false ){ // Input.GetKeyDown->キーを押していない状態から押した時
        
            // ジャンプキーが押された&地面にいるならジャンプ
            anim.SetBool("isJump", true);

            rd2d.AddForce( Vector2.up * jumpForce );

        }

        if ( isGround==false ) { // 地面にいるときはジャンプモーションOFF
            anim.SetBool( "isJump" , false );
            anim.SetBool( "isFall" , false );
        }

        // 早くなりすぎないようにvelcotyを調整
        float velX = rd2d.velocity.x;
        float velY = rd2d.velocity.y;

        if ( velY > 0.5f ) { // velocityが上向きに働いていたらジャンプ
            anim.SetBool("isJump", true);
        }
        if ( velY < -0.1f ) { // velocityが下向きに働いていたら落下
            anim.SetBool("isFall", true);
        }

        if ( Mathf.Abs(velX) > 5 ){

            if (velX > 5.0f) {
                rd2d.velocity = new Vector2( 5.0f, velY );
            }
            if (velX < -5.0f) {
                rd2d.velocity = new Vector2( -5.0f, velY );
            }
        }

    }

    IEnumerator Dead(){

        anim.SetBool("isDamage",true);

        // コルーチンでウェイトを入れる
        // yield returnで処理を中断できる
        yield return new WaitForSeconds(0.5f);

        rd2d.AddForce(Vector2.up * jumpForce);
        // ダメージを落ちた後下に落ちていくため、当たり判定をオフに
        GetComponent<CircleCollider2D>().enabled = false;
        Sound.PlaySe("gameover",0);
    }

    // 敵オブジェクトのコライダーを識別
    void OnTriggerEnter2D( Collider2D col ){ // 通り抜けたかどうか

        if(col.gameObject.tag == "Enemy"){
            isDead = true;
            // コルーチンを実行
            StartCoroutine("Dead");
        }

    }

    // タイルマップのコリジョンを識別
    void OnCollisionEnter2D( Collision2D col ){ // 乗ったか

        if(col.gameObject.tag == "Damage"){
            // 障害物に当たったら死亡フラグをONにする
            // TODO:ライフを減らす処理に変更する
            isDead = true;
            StartCoroutine("Dead");
        }

    }

    private void FixedUpdate()
    {
        isGround = false;

        float x = Input.GetAxisRaw("Horizontal");

        // 自分の立っている場所
        Vector2 groundPos =
            new Vector2 (
                transform.position.x , 
                transform.position.y
            );

        // 地面判定エリア
        Vector2 groundArea = new Vector2( 0.5f, 0.4f );

        Vector2 wallArea1 = new Vector2( x *  0.8f, 1.5f );
        Vector2 wallArea2 = new Vector2( x * 0.3f, 1.0f ); // 左向きのときは当たり判定が-になるようにする

        Vector2 wallArea3 = new Vector2( x *  1.5f, 0.6f );
        Vector2 wallArea4 = new Vector2( x * 1.0f, 0.1f );

        Debug.DrawLine ( groundPos + wallArea1 , groundPos - wallArea2 , Color.red );
        Debug.DrawLine ( groundPos + wallArea3 , groundPos - wallArea4 , Color.red );

        isGround = 
            Physics2D.OverlapArea(
                groundPos + groundArea,
                groundPos + groundArea,
                groundLayer // 地面とか敵とかでレイヤーを分けておく
            );

        bool area1 = false; // こめかみ
        bool area2 = false; // 足の手前

        area1 =
            Physics2D.OverlapArea(
                groundPos + wallArea1,
                groundArea + wallArea2,
                groundLayer
            );

        area2 =
            Physics2D.OverlapArea(
                groundPos + wallArea3,
                groundArea + wallArea4,
                groundLayer
            );

        // 坂道の時(area1がtureでarea2がfalse,つまりarea1とarea2が並行ではないとき)
        if( !area1 & area2 ){
            isSloped = true;
        } else{
            isSloped = false;
        }
        Debug.Log(isSloped);

        // Debug.Log("接地判定" + isGround);

    }

}
