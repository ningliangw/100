using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBase : MonoBehaviour
{
    public string tileName;        // 地图块的名称
    public int tileType;           // 地图块的类型

    private bool isHighlighted;    // 地图块是否被高亮显示

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void SetHighLight(bool b)
    {
        this.isHighlighted = b;

        // 根据需要设置地图块的高亮显示效果
        if (isHighlighted)
        {
            // 设置高亮的颜色、材质等等
            GetComponent<Renderer>().material.color = Color.yellow;
        }
        else
        {
            // 恢复正常的颜色、材质等等
            GetComponent<Renderer>().material.color = Color.white;
        }
    }
}
