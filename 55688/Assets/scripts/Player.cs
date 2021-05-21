using UnityEngine;
using UnityEngine.SceneManagement;//引用 場景管理 API
using UnityEngine.UI; // 引用 介面API

public class Player : MonoBehaviour
{
    #region 欄位
    [Header("等級")]
    public int lv = 1;
    [Header("移動速度"), Range(0, 300)]
    public float speed = 10.5f;
    [Header("角色是否死亡")]
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
    [Header("血量")]
    public float hp = 200;
    [Header("血條系統")]
    public HpManager hpManager;
    [Header("攻擊力"), Range(0, 1000)]
    public float attack = 20;
    [Header("等級文字")]
    public Text textLv;
    [Header("吃金條音效")]
    public AudioClip soundEat;
    [Header("金幣數量")]
    public Text textCoin;


    private float hpMax;
    public int coin;
    public float attackWeapon;
    #endregion

    #region 方法
    /// <summary>
    /// 移動
    /// </summary>

    private void Move()
    {
        if (isDead) return;     // 如果 死亡 就跳出
        
        float h = joystick.Horizontal;
        float v = joystick.Vertical;
 
        tra.Translate(h * speed * Time.deltaTime, v * speed * Time.deltaTime, 0);

        ani.SetFloat("水平", h);
        ani.SetFloat("垂直", v);
    }

    public void Attack()
    {
        if (isDead) return;     // 如果 死亡 就跳出

        // 音效來源,播放一次(音效片段,音量)
        aud.PlayOneShot(soundAttack, 0.5f);

        // 2D 物理 圓形碰撞(中心點,半徑,方向,距離,圖層)
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, rangeAttack, -transform.up, 0, 1 << 8);

        // 如果 碰到的物件 標籤 為 道具 就取得道具腳本並呼叫掉落道具方法
        if (hit && hit.collider.tag == "道具") hit.collider.GetComponent<Item>().DropProp();
        // 如果 打到的標籤是 敵人 就對他 造成傷害
        if (hit && hit.collider.tag == "敵人") hit.collider.GetComponent<Enemy>().Hit(attack + attackWeapon);
        // 如果 打到的標籤是NPC 就開啟商店
        if (hit && hit.collider.tag == "NPC") hit.collider.GetComponent<NPC>().OpenShop();
    }

    /// <summary>
    /// 受傷
    /// </summary>
    /// <param name="damage">接收到的傷害值</param>
    public void Hit(float damage)
    {
        hp -= damage;                            // 扣除傷害值               
        hpManager.UpdateHpBar(hp, hpMax);        // 更新血條
        StartCoroutine(hpManager.ShowDamage(damage));  // 啟動協同程序 (顯示傷害值())

        if (hp <= 0) Dead();                           // 如果 血量 <= 0 就死亡
    }

    
    /// <summary>
    /// 死亡
    /// </summary>
    private void Dead()
    {
        hp = 0;
        isDead = true;
        Invoke("Replay", 2);      // 延遲呼叫("方法名稱",延遲時間)
    }

    private void Replay()
    {
        SceneManager.LoadScene("遊戲場景");
    }
    #endregion

    private float exp;

    /// <summary>
    /// 需要多少經驗值會升等，一等設定為 100
    /// </summary>
    private float expNeed = 100;

    [Header("經驗值吧條")]
    public Image imgExp;

    /// <summary>
    /// 經驗值控制
    /// </summary>
    /// <param name="getExp">接收到的經驗值</param>
    public void Exp(float getExp)
    {
        // 取得目前等級需要的經驗需求
        // 要取得的資料為 等級 減一
        expNeed = expData.exp[lv - 1];
        
        exp += getExp;
        print("經驗值：" + exp);
        imgExp.fillAmount = exp / expNeed;
        // 升級
        // 迴圈 while
        // 語法：
        // while (布林值) { 布林值 為 true 時持續執行 }
        // if (布林值) {布林值 為 true 時執行一次 }

        while(exp >= expNeed)                       // 如果 經驗值 >= 經驗需求 ex 120>100
        {
            lv++;                                // 升級 ex 2
            textLv.text = "lv" + lv;             // 介面更新 ex Lv2
            exp -= expNeed;                      // 將多餘的經驗值補回來 ex 120-100=20
            imgExp.fillAmount = exp / expNeed;   // 介面更新
            expNeed = expData.exp[lv - 1];
            LevelUp();
        }
    }

    private void LevelUp()
    {
        // 攻擊力每一等提升 10，從 20 開始
        attack = 20 + (lv - 1) * 10;
        // 血量每一等提升 50，從200開始
        hpMax = 200 + (lv - 1) * 50;

        hp = hpMax;                         // 恢復血量全滿
        hpManager.UpdateHpBar(hp, hpMax);   // 更新血條
    }

    [Header("經驗值資料")]
    public ExpData expData;

    #region 事件
    // 事件 - 特定時間會執行的方法
    // 開始事件 : 播放後執行一次
    private void Start()
    {
        // 給予玩家起始金幣
        coin = 10;
        textCoin.text = "金幣：" + coin;
        
        hpMax = hp; // 取得血量最大值

        // 利用公式寫入經驗值資料 - 一等 100，兩等 200...
        for (int i = 0; i < 99; i++)
        {
            // 經驗值資料 的 經驗值陣列[編號] = 公式
            // 公式：(編號 + 1) * 100 - 每等增加 100
            expData.exp[i] = (i + 1) * 100;
        }
    }
    private void Update()
    {
        Move();
    }

    //觸發事件-進入:瞭個物件其中1個要勾選is Trigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "金條")
        {
            coin++;
            aud.PlayOneShot(soundEat);
            Destroy(collision.gameObject);
            textCoin.text = "金幣: " + coin;
        }
    }
    private void OnDrawGizmos()
    {
        //指定圖示的顏色
        Gizmos.color = new Color(1, 0, 0, 0.4f);
        //繪製圖示
        Gizmos.DrawSphere(transform.position, rangeAttack);
    }
    #endregion
}
