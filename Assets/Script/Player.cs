using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{

    // 変数名はキャメルケースで書く

    public int currentHp; // 現在HP
    public int maxHp; // 最大HP

    public Slider hpBar; // スライダー用の変数
    public Text hpText; // テキスト用の変数

    // Start is called before the first frame update
    void Start()
    {
        // 最大HPを固定
        maxHp = 90;
        // 現在値を最大に設定
        currentHp = maxHp;

        // スライダーの最大値を変更(maxValueはint型)
        hpBar.maxValue = maxHp;
    }

    // Update is called once per frame
    void Update()
    {
        hpBar.value = currentHp;

        hpText.text = currentHp.ToString() + " / " + maxHp.ToString();
    }

    public void Damage()
    {
        // currentHpから-10
        currentHp -= 30;
        if (currentHp == 0)
        {
            this.gameObject.GetComponent<PlayerCtrl>().StartCoroutine("Dead");
        }
    }
}
