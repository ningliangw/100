using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyclicBlock : TileBase
{
    [SerializeField]
    private GameObject highlight;

    private float direction = -1;          // 初始方向为向下
    private Vector3 initialPosition;       // 初始位置

    [SerializeField]
    private float moveSpeed = 5f;          // 移动速度

    [SerializeField]
    private float intervalTime = 2f;      // 间隔时间

    void Start()
    {
        StartCoroutine(MoveCoroutine());    // 启动协程
    }

    void Update()
    {
        if (LevelManager.Instance.GetMode() != LevelManager.OptMode.Start)
        {
            transform.position = new Vector3(transform.position.x, 4.5f, 0f);
            initialPosition = transform.position;
        }   
    }

    private IEnumerator MoveCoroutine()
    {
        while (true)
        {
            if (LevelManager.Instance.GetMode() == LevelManager.OptMode.Start)
            {
                float movement = direction * moveSpeed * Time.deltaTime;
                transform.position += new Vector3(0f, movement, 0f);

                if (!LevelManager.Instance.CheckInRange(transform.position))//超出边界
                {
                    transform.position = initialPosition;
                    // 等待一段时间
                    yield return new WaitForSeconds(intervalTime);
                }
            }
            yield return null;
        }
    }


    public override void SetHighLight(bool b)
    {
        highlight.SetActive(b);
    }
}
