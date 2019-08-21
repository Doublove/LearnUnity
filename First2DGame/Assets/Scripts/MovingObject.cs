using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour
{
    public float moveTime = 0.1f;
    public LayerMask blockingLayer;


    private BoxCollider2D boxCollider;
    private Rigidbody2D rb2D;
    private float inverseMoveTime;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
        inverseMoveTime = 1f / moveTime;// 等于速度？
    }

    protected bool Move(int xDir,int yDir,out RaycastHit2D hit)
    {
        // out 返回引用
        // RaycastHit2D 返回有关2D物理射线投射检测对象信息。
        Vector2 start = transform.position;// 当前的transform位置，直接强制转换为Vector2类型略去Z轴
        Vector2 end = start + new Vector2(xDir, yDir);// 移动后的位置，是初始位置加上传入Move的方向xDir yDir

        boxCollider.enabled = false;// 确保在Linecast判断时不会hit到我们自己的collider
        hit = Physics2D.Linecast(start, end, blockingLayer);// 发射一条从start到end的射线检查blockingLayer上是否发生了碰撞
        boxCollider.enabled = true;

        if (hit.transform == null)
        {
            // 如果条件为true，则表示没有碰撞，可以进行移动
            StartCoroutine(SmoothMovement(end));
            return true;
        }
        return false;
    }

    protected IEnumerator SmoothMovement (Vector3 end)// IEnumerable枚举器接口/IEnumerator迭代器接口
    {
        // end是移动的目标位置
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;// magnitude是精确距离，sqrMagnitude是节省CPU的粗略距离

        while (sqrRemainingDistance > float.Epsilon)
        {
            //  Time.deltaTime是按秒来统计的，而Updata()是按每帧来统计的。MoveTowards在Lerp线性插值的基础上限制了最大速度。
            Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
            rb2D.MovePosition(newPosition);// rb2D刚体对象移动到该位置
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;// 重新计算距离
            yield return null;// yield 关键字用于指定返回的一个或多个值。 到达 yield return 语句时，会保存当前位置。 下次调用迭代器时将从此位置重新开始执行。
        }
    }

    protected virtual void AttemptMove<T>(int xDir,int yDir)
        where T:Component
    {
        RaycastHit2D hit;
        bool canMove = Move(xDir, yDir, out hit);

        if (hit.transform == null)
            return;// 如果没有检测到碰撞(路可通行)，则略过以下代码
        // GetComponent<T> 从当前游戏对象获取组件T
        T hitComponent = hit.transform.GetComponent<T>();// 如果碰撞到了，则获取该物体

        if (!canMove && hitComponent != null)
            OnCantMove(hitComponent);// 如果不能移动且碰撞到了物体，则把物体类型传给OnCantMove函数
    }

    protected abstract void OnCantMove<T>(T component)
        where T : Component;
}
