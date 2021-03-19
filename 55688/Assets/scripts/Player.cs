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

    private void Attack()
    {

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
