﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCtrl : MonoBehaviour
{

    public float speed = 5; // publicで変数を作るとUnity上でパラメータをかえられます
    public float ladderspeed = 20;
    public float jumpForce = 900f;
    public LayerMask groundLayer;

    private Rigidbody2D rd2d;
    private Animator anim;
    private SpriteRenderer spRenderer;
    [SerializeField] private PhysicsMaterial2D friction1;//摩擦を弄るためのマテリアル
    [SerializeField] private PhysicsMaterial2D friction0;
    private bool isGround; // ジャンプフラグを返す変数
    private bool isSloped;
    private bool isDead = false; // 死亡フラグ
    private int jump_power = 0;
    private int Max_jump_power = 1;//空中ジャンプの回数
    private bool touchladder;
    private bool laddermode;

    // Start is called before the first frame update
    void Start()
    {
        // コンストラクタで初期化する
        this.rd2d = GetComponent<Rigidbody2D>();
        this.anim = GetComponent<Animator>();
        this.spRenderer = GetComponent<SpriteRenderer>();
        Sound.LoadSe("gameover", "img11");
        Sound.LoadSe("damage", "se_maoudamashii_retro22");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        { // Input.GetKeyDown->キーを押していない状態から押した時

            if (jump_power <= 0)
            {
                return;
            }
            else
            {
                jump_power--;
                //パワーを消費してジャンプ。地面にいるならジャンプパワーは離陸前に補充される
                //ジャンプの前にかかっているベクトルを0にする
                rd2d.velocity = Vector3.zero;
                rd2d.AddForce(Vector2.up * jumpForce);
            }


        }

        //ハシゴに触れている時に上下の入力があったら"ハシゴモード"になる
        if (touchladder == true && Input.GetAxisRaw("Vertical") != 0)
        {
            laddermode = true;
            rd2d.velocity = Vector3.zero;
        }
        //ハシゴモードの時、落下したり踏み外さないようにする
        if (laddermode == true)
        {
            rd2d.gravityScale = 0;
            if (Input.GetAxisRaw("Horizontal") == 0)
            {
                rd2d.velocity = new Vector2(0, 0);
            }
            //ハシゴから離れたらハシゴモード解除
            if (touchladder == false)
            {
                laddermode = false;
            }
            //ジャンプ入力でハシゴモード解除
            else if (Input.GetButtonDown("Jump"))
            {
                laddermode = false;
            }
        }       
        else
        {
            rd2d.gravityScale = 1;
        }
    }

    IEnumerator Dead()
    {

        isDead = true;
        anim.SetBool("Dead", true);

        // コルーチンでウェイトを入れる
        // yield returnで処理を中断できる
        yield return new WaitForSeconds(0.5f);

        rd2d.AddForce(Vector2.up * jumpForce);
        // ダメージを落ちた後下に落ちていくため、当たり判定をオフに
        GetComponent<CircleCollider2D>().enabled = false;
        Sound.PlaySe("gameover", 0);
        SceneManager.LoadScene("GameOver", LoadSceneMode.Additive);
    }

    // 敵オブジェクトのコライダーを識別
    void OnTriggerEnter2D(Collider2D col)
    { // 通り抜けたかどうか

        if (col.gameObject.tag == "Enemy")
        {
            // isDead = true;
            // // コルーチンを実行
            // StartCoroutine("Dead");

            StartCoroutine("Damage");
            // this.gameObjectはそのクラスがアタッチされているゲームオブジェクトを指す
            this.gameObject.GetComponent<Player>().Damage();

        }
        //ハシゴに触れているか判定
        if (col.gameObject.tag == "ladder")
        {
            touchladder = true;

        }
    }
    void OnTriggerExit2D(Collider2D col)
    { 
        //ハシゴから離れたか判定
        if (col.gameObject.tag == "ladder")
        {
            touchladder = false;
        }
    }


    IEnumerator Damage()
    {

        anim.SetBool("isDamage", true);
        Sound.PlaySe("damage");
        // コルーチンでウェイトを入れる
        // yield returnで処理を中断できる
        yield return new WaitForSeconds(0.5f);
        anim.SetBool("isDamage", false);

    }

    // タイルマップのコリジョンを識別
    void OnCollisionEnter2D(Collision2D col)
    { // 乗ったか

        if (col.gameObject.tag == "Damage")
        {
            // 障害物に当たったら死亡フラグをONにする
            // TODO:ライフを減らす処理に変更する

            isDead = true;
            StartCoroutine("Dead");
        }

    }

    private void FixedUpdate()
    {
        float x = Input.GetAxisRaw("Horizontal"); // Aキー-1・なにもしない0・Dキーで1が帰ってくる関数

        // スプライトの向きを変える
        if (x < 0)
        {
            spRenderer.flipX = true;
        }
        else if (x > 0)
        {
            spRenderer.flipX = false;
        }

        //移動処理にAddForceを使う場合はFixedUpdateに書く方がいいのでこっちに移植
        //依存関係の整理が面倒だったのでUpdate内の処理である必要のないものはまとめてFixedUpdateにもってきた
        if (isDead == false)
        {
            rd2d.AddForce(Vector2.right * x * speed);　// 2Dベクトルの右方向に速度変数を掛けている(横方向に力を加えている)
        }
        anim.SetFloat("Speed", Mathf.Abs(x * speed)); // 歩くアニメーションを判定する数値の更新

        // Translate(物理演算無視)でvelocity(移動量)ゼロになる

        if (isSloped)
        {
            this.gameObject.transform.Translate(0.1f * x, 0.0f, 0.0f);
        }
        
        //はしごの移動処理
        if(laddermode==true)
        {
            rd2d.AddForce(Vector2.up * Input.GetAxisRaw("Vertical") * ladderspeed);
        }
        


        // 早くなりすぎないようにvelcotyを調整
        float velX = rd2d.velocity.x;
        float velY = rd2d.velocity.y;

        if (Mathf.Abs(velX) > 5)
        {

            if (velX > 5.0f)
            {
                rd2d.velocity = new Vector2(5.0f, velY);
            }
            if (velX < -5.0f)
            {
                rd2d.velocity = new Vector2(-5.0f, velY);
            }
        }

        //↓もともとFixUpdateの部分
        isGround = false;

        //float x = Input.GetAxisRaw("Horizontal");処理がかぶったのでコメントアウト

        // 自分の立っている場所
        Vector2 groundPos =
            new Vector2(
                transform.position.x,
                transform.position.y
            );

        // 地面判定エリア
        //接地判定が大きすぎたので小さめに調整
        Vector2 groundArea = new Vector2(0.1f, 0.1f);
        //接地判定を長方形とした時の対角線を青線でデバック中のシーンビューに表示
        Debug.DrawLine(groundPos + groundArea, groundPos - groundArea, Color.blue);

        Vector2 wallArea1 = new Vector2(x * 0.8f, 1.5f);
        Vector2 wallArea2 = new Vector2(x * 0.3f, 1.0f); // 左向きのときは当たり判定が-になるようにする

        Vector2 wallArea3 = new Vector2(x * 1.5f, 0.6f);
        Vector2 wallArea4 = new Vector2(x * 1.0f, 0.1f);

        Debug.DrawLine(groundPos + wallArea1, groundPos - wallArea2, Color.red);
        Debug.DrawLine(groundPos + wallArea3, groundPos - wallArea4, Color.red);

        //接地判定
        isGround =
            Physics2D.OverlapArea(
                groundPos + groundArea,
                groundPos - groundArea,//符号ミス
                groundLayer // 地面とか敵とかでレイヤーを分けておく
            );
        //ハシゴのアニメーション切り替え
        if(laddermode==true)
        {
            jump_power = Max_jump_power;
            anim.SetBool("isClimb", true);
            anim.SetBool("isJump", false);
            anim.SetBool("isFall", false);
        }
        //地面にいる間は常に1なのでジャンプできる
        //空中ではjumppowerを消費して一度だけジャンプできる
        else if (isGround == true)
        {
            jump_power = Max_jump_power;
            anim.SetBool("isJump", false);
            anim.SetBool("isFall", false);
            anim.SetBool("isClimb", false);
        }
        else if (velY < 0f)
        { // velocityが下向きに働いていたら落下
            anim.SetBool("isFall", true);
            anim.SetBool("isJump", false);
            anim.SetBool("isClimb", false);
        }
        else
        { // velocityが上向きに働いていたらジャンプ
            anim.SetBool("isJump", true);
            anim.SetBool("isFall", false);
            anim.SetBool("isClimb", false);
        }
        
        //空中にいる時は摩擦を０にする
        if (isGround == false)
        {
            rd2d.sharedMaterial=friction0;
        }
        else
        {
            rd2d.sharedMaterial=friction1;
        }
        

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
        if (!area1 & area2)
        {
            isSloped = true;
        }
        else
        {
            isSloped = false;
        }
    }
}
