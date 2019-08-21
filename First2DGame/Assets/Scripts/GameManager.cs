using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	public float levelStartDelay = 2f;
	public float turnDelay = .1f;// 回合之间的时间间隔
    public static GameManager instance = null;// 实例
    public BoardManager boardScript;
    public int playerFoodPoints = 100;// 
    [HideInInspector]
    public bool playersTurn = true;


	private Text levelText;// UI
	private GameObject levelImage;// UI
    private int level = 1;
	private List<Enemy> enemies;
	private bool enemiesMoving;
	private bool doingSetup;// UI。防止玩家在关卡创建期间进行移动

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(instance);// 避免出现两个或以上的instance

        DontDestroyOnLoad(gameObject);// 保证分数不会在切换关卡时被重置
		enemies = new List<Enemy>();
        boardScript = GetComponent<BoardManager>();
        InitGame();
    }

	private void OnLevelWasLoaded(int index)
	{
		++level;
		InitGame();
	}

	void InitGame()
    {
		doingSetup = true;

		levelImage = GameObject.Find("LevelImage");
		levelText = GameObject.Find("LevelText").GetComponent<Text>();
		levelText.text = "Day " + level;
		levelImage.SetActive(true);
		Invoke("HideLevelImage", levelStartDelay);

		enemies.Clear();
        boardScript.SetupScene(level);
    }

	private void HideLevelImage()
	{
		levelImage.SetActive(false);
		doingSetup = false;
	}

    public void GameOver()
    {
		levelText.text = "After " + level + "days, you starved.";
		levelImage.SetActive(true);
        enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
		// 
		if (playersTurn || enemiesMoving || doingSetup)
			return;

		StartCoroutine(MoveEnemies());
    }

	public void AddEnemyToList(Enemy script)
	{
		enemies.Add(script);
	}

	IEnumerator MoveEnemies()
	{
		enemiesMoving = true;
		yield return new WaitForSeconds(turnDelay);
		if (enemies.Count == 0)
		{
			yield return new WaitForSeconds(turnDelay);
		}

		for(int i = 0; i < enemies.Count; ++i)
		{
			enemies[i].MoveEnemy();
			yield return new WaitForSeconds(enemies[i].moveTime);
		}

		playersTurn = true;

		enemiesMoving = false;
	}
}
