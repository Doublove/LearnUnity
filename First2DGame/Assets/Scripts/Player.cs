using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class Player : MovingObject
{
    public int wallDamage = 1;
    public int pointsPerFood = 10;
    public int pointsPerSoda = 20;
    public float restartLevelDelay = 1f;
	public Text foodText;

    private Animator animator;
    private int food;

    // Start is called before the first frame update
    protected override void Start()
    {
        animator = GetComponent<Animator>();

        food = GameManager.instance.playerFoodPoints;// 切换关卡时将FoodPoints保存到变量中，就不会在切换关卡时被清理

		foodText.text = "Food: " + food;

        base.Start();// MovingObject的start
    }

    private void OnDisable()// Unity的底层api
    {
        GameManager.instance.playerFoodPoints = food;// 切换关卡时将FoodPoints恢复
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.playersTurn)
            return;

        int horizontal = 0;
        int vertical = 0;

        horizontal = (int)Input.GetAxisRaw("Horizontal");// 按键输入
        vertical = (int)Input.GetAxisRaw("Vertical");

        if (horizontal != 0)
            vertical = 0;// 防止对角移动

        if (horizontal != 0 || vertical != 0)
            AttemptMove<Wall>(horizontal, vertical);// 判断移动方向上是否是墙
    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        --food;// 每次移动减1 food
		foodText.text = "Food: " + food;

        base.AttemptMove<T>(xDir, yDir);

        RaycastHit2D hit;

        CheckIfGameOver();

        GameManager.instance.playersTurn = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 首先检查碰撞到的物体是否是Exit
        if (other.tag == "Exit")
        {
            Invoke("Restart", restartLevelDelay);// 如果是Exit，则等待1秒(参数设定值)，然后重新生成场景(下一关)
            enabled = false;// 然后玩家将为非enabled
        }
		else if (other.tag == "Food")
		{
			food += pointsPerFood;
			foodText.text = "+" + pointsPerFood + " Food: " + food;
			other.gameObject.SetActive(false);
		}
		else if (other.tag == "Soda")
		{
			food += pointsPerSoda;
			foodText.text = "+" + pointsPerSoda + " Food: " + food;
			other.gameObject.SetActive(false);
		}
    }

    protected override void OnCantMove<T>(T component)
    {
        Wall hitWall = component as Wall;
        hitWall.DamageWall(wallDamage);
        animator.SetTrigger("playerChop");
    }

    private void Restart()
    {
		SceneManager.LoadScene("Main");
		// Application.LoadLevel已过时
		//Application.LoadLevel(Application.loadedLevel);// 加载最近加载过的场景，在此即main，唯一的场景。因为场景是由脚本生成的
    }

    public void LoseFood(int loss)
    {
        animator.SetTrigger("playerHit");
        food -= loss;
		foodText.text = "-" + loss + " Food: " + food;
		CheckIfGameOver();
    }

    private void CheckIfGameOver()
    {
        if (food <= 0)
            GameManager.instance.GameOver();
    }
}
