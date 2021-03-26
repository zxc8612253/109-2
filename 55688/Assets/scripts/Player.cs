using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("等級")]
    [Tooltip("這是角色的等級")]
    public int lv = 1;
    [Header("移動速度"), Range(0, 300)]
    public float speed = 10.5f;
    public bool isDead = false;
    [Tooltip("這是角色的名字")]
    public string cName = "貓咪";
    [Header("虛擬搖桿")]
    public FixedJoystick joystick;
    [Header("變形元件")]
    public Transform tra;
    [Header("動畫元素")]
    public Animator ani;
    [Header("偵測範圍")]
    public float rangeAttack = 2.5f;
    [Header("音效來源")]
    public AudioSource aud;
    [Header("攻擊音效")]
    public AudioClip soundAttack;

    private void OnDrawGizmos()
    {
        //指定圖示的顏色
        Gizmos.color = new Color(1, 0, 0, 0.4f);
        //繪製圖示
        Gizmos.DrawSphere(transform.position, rangeAttack);
    }

    /// <summary>
    /// 移動
    /// </summary>

    private void Move()
    {
        float h = joystick.Horizontal;
        float v = joystick.Vertical;
 
        tra.Translate(h * speed * Time.deltaTime, v * speed * Time.deltaTime, 0);

        ani.SetFloat("水平", h);
        ani.SetFloat("垂直", v);
    }

    public void Attack()
    {
        // 音效來源,播放一次(音效片段,音量)
        aud.PlayOneShot(soundAttack, 0.5f);

        // 2D 物理 圓形碰撞(中心點,半徑,方向,距離,圖層)
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, rangeAttack, -transform.up, 0, 1 << 8);

        // 如果 碰到的物件 標籤 為 道具 就刪除
        if (hit.collider.tag == "道具") Destroy(hit.collider.gameObject);
    }

    private void Hit()
    {

    }

    private void Dead()
    {

    }

    private void Start()
    {
        
    }
    private void Update()
    {
        Move();
    }
}
