using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour
{
    [Serializable]
    public class Count
    {
        public int minimum;
        public int maximum;

        public Count (int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }

    public int columns = 8;// 设定游戏场景大小的参数 列
    public int rows = 8;// 行
    public Count wallCount = new Count(5, 9);// 控制随机生成的墙的数量
    public Count foodCount = new Count(1, 5);// 食物的数量
    public GameObject exit;
    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject[] foodTiles;
    public GameObject[] enemyTiles;
    public GameObject[] outerWallTiles;
    // Transform 变换，是场景中最常打交道的类，用于控制物体的位移，旋转，缩放等功能。
    private Transform boardHolder;// 用于保持游戏场景的整洁，防止曾经生成的游戏对象仍然保留在场景中
    private List<Vector3> gridPositions = new List<Vector3>();// 用于记录游戏对象所有可能的生成点位置，追踪一个游戏物体是否被创建在了这些位置上

    void InitialiseList()// 初始化gridPosition列表
    {
        gridPositions.Clear();

        for(int x = 1; x < columns - 1; ++x)
        {
            for(int y = 1; y < rows - 1; ++y)
            {
                gridPositions.Add(new Vector3(x, y, 0f));
            }
        }
    }

    void BoardSetup()
    {
        boardHolder = new GameObject("Board").transform;

        for(int x=-1;x<columns + 1; ++x)
        {
            for(int y=-1;y<rows + 1; ++y)
            {
                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];// 从基本地板的prefab中随机选择
                if (x == -1 || x == columns || y == -1 || y == rows)
                    toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];// 如果这个点的位置在边界上，则它为外围墙
                // 用Instantiate函数将这个prefab创建到(x,y,0f)的位置，Quaternion.identity表示无旋转，as GameObject指cast到一个GameObject上
                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
                // 把这个新生成的instance附属到boardHolder上，作为其子
                instance.transform.SetParent(boardHolder);
            }
        }
    }

    Vector3 RandomPosition()
    {
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector3 randomPosition = gridPositions[randomIndex];// 在gridPositions里随机选择
        gridPositions.RemoveAt(randomIndex);// 避免在同一个位置重复生成
        return randomPosition;
    }

    void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
    {
        int objectCount = Random.Range(minimum, maximum + 1);// 随机生成数，控制要生成的对象数量

        for(int i = 0; i < objectCount; ++i)
        {
            Vector3 randomPosition = RandomPosition();// 随机位置
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
            Instantiate(tileChoice, randomPosition, Quaternion.identity);
        }
    }

    public void SetupScene(int level)
    {
        BoardSetup();// 创建场景
        InitialiseList();// 初始化位置列表
        LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);// 随机生成墙
        LayoutObjectAtRandom(foodTiles, foodCount.minimum, foodCount.maximum);// 随机生成食物
        int enemyCount = (int)Mathf.Log(level, 2f);// 敌人的数量由关卡值取对数
        LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);// 随机生成敌人
        Instantiate(exit, new Vector3(columns - 1, rows - 1, 0f), Quaternion.identity);// 在场景右上角生成出口
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
