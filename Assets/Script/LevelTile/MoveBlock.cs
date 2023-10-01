using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBlock : TileBase
{
    [SerializeField]
    private GameObject highlight;

    private float currentPosition;
    private float direction = 1;          // 初始方向为向右

    private const float maxPosition = 4;  // 移动最大位置（5格）

    public float moveSpeed = 1f;          // 移动速度

    void Update()
    {
        if (LevelManager.Instance.GetMode() == LevelManager.OptMode.Start)
        {
            Move();
        }
    }

    private void Move()
    {
        float movement = direction * moveSpeed * Time.deltaTime;
        transform.position += new Vector3(movement, 0f, 0f);

        currentPosition += Mathf.Abs(movement);

        if (currentPosition > maxPosition || !LevelManager.Instance.CheckInRange(transform.position))//超过最大位置或超出边界
        {
            currentPosition = 0;
            direction *= -1;        // 改变方向
        }
       
    }

    public override void SetHighLight(bool b)
    {
        highlight.SetActive(b);
    }
}
