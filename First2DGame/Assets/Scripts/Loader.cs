using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour
{
    public GameObject gameManager;


    // Start is called before the first frame update
    void Awake()
    {
        // 通过loader脚本来访问GameManager的instance
        if (GameManager.instance == null)
            Instantiate(gameManager);// 如果该instance没有被创建，则进行instantiate(实例化)
    }
}
