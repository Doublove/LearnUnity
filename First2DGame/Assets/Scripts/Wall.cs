using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public Sprite dmgSprite;// 被攻击的sprite
    public int hp = 4;


    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void DamageWall(int loss)
    {
        spriteRenderer.sprite = dmgSprite;// 攻击墙的视觉反馈
        hp -= loss;
        if (hp <= 0)
            gameObject.SetActive(false);// 对象置非活跃
    }
}
