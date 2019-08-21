using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovingObject
{
	public int playerDamage;


	private Animator animator;
	private Transform target;// 保存玩家位置，即敌人移动方向位置
	private bool skipMove;// 回合移动

    // Start is called before the first frame update
    protected override void Start()
    {
		GameManager.instance.AddEnemyToList(this);// 把enemy加到GameManager的AddEnemyToList中，这样GameManager就可以调用MoveEnemy
		animator = GetComponent<Animator>();
		target = GameObject.FindGameObjectWithTag("Player").transform;
		base.Start();
    }

	protected override void AttemptMove<T>(int xDir, int yDir)
	{
		if (skipMove)
		{
			skipMove = false;
			return;
		}

		base.AttemptMove<T>(xDir, yDir);

		skipMove = true;
	}

	public void MoveEnemy()
	{
		int xDir = 0;
		int yDir = 0;

		if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)// 检查玩家和敌人是否在同一column(列)
			yDir = target.position.y > transform.position.y ? 1 : -1;// 否则向y方向移动1，方向由玩家位置决定
		else
			xDir = target.position.x > transform.position.x ? 1 : -1;

		AttemptMove<Player>(xDir, yDir);
	}

	protected override void OnCantMove<T>(T component)
	{
		Player hitPlayer = component as Player;

		animator.SetTrigger("enemyAttack");

		hitPlayer.LoseFood(playerDamage);
	}
}
