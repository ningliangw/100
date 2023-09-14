using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    public Transform startPoint;            // 起点位置
    public Transform endPoint;              // 终点位置

    public GameObject[] registries;         // 注册的玩家
    public List<TileBase> tiles;            // 瓦片

    float grid_space = 1f;                  // 网格间距

    [SerializeField]
    private GameObject tool_move;           // 移动工具

    private OptMode mode = OptMode.Select;  // 当前操作模式
    Vector3 m_last;                         // 上一帧鼠标的世界坐标

    enum OptMode    // 操作模式的枚举
    {
        Select, Put
    }

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }
    enum Dir        // 方向枚举
    {
        None, XY, X, Y
    }

    Dir m_dir = Dir.None;
    // Update is called once per frame
    void Update()
    {
        Vector3 m_world = Camera.main.ScreenToWorldPoint(Input.mousePosition);  // 将鼠标位置转换为世界坐标系
        Vector3 mouse_delta = m_world - m_last;                                 // 鼠标移动的增量向量
        if (Input.GetMouseButtonDown(0))
        {
            //检测是否点击到了编辑器工具
            RaycastHit2D cast_tool = Physics2D.Raycast(m_world, Vector2.zero, 0f, LayerMask.GetMask("Editor"));

            if (cast_tool.collider)                                             // 如果点击了编辑器工具
            {
                switch (cast_tool.collider.name)
                {
                    case "right":
                        m_dir = Dir.X;
                        break;
                    case "up":
                        m_dir = Dir.Y;
                        break;
                    case "center":
                        m_dir = Dir.XY;
                        break;
                    default: 
                        break;
                }
            }
            else                                                                // 如果没有
            {
                m_dir = Dir.None;
                //检测是否点击到了关卡
                RaycastHit2D cast_level = Physics2D.Raycast(m_world, Vector2.zero, 0f, LayerMask.GetMask("Level"));

                switch (mode)
                {
                    case OptMode.Select:
                        SelectObject(cast_level);                               // 选择物体
                        break;
                    case OptMode.Put:

                        break;
                    default:
                        break;
                }
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            m_dir = Dir.None;
            select_dirty = true;                                                //选择状态
        }

        if (select_dirty)
        {
            select_dirty = false;                                               // 重置选择状态
            calSelectedCenter();                                                // 计算选择物体的中心点
            now_grid_pos = GetGridPos(select_center + mouse_delta, grid_space); // 计算当前网格位置
        }

        if (Input.GetMouseButton(0))                                            // 鼠标左键一直按下时
        {
            switch (m_dir)
            {
                case Dir.X:
                    mouse_delta.y = 0;
                    break;
                case Dir.Y:
                    mouse_delta.x = 0;
                    break;
                case Dir.XY:
                    break;
                default:
                    mouse_delta = Vector2.zero;
                    break;
            }

            select_center += mouse_delta;                                       // 更新选择物体的中心点位置
            if (Grid)                                                           // 编辑时网格对齐
            {
                Vector3 v1 = GetGridPos(select_center, grid_space);             // 计算网格对齐后的位置
                mouse_delta = v1 - now_grid_pos;                                // 更新移动增量向量
                now_grid_pos = v1;                                              // 更新当前网格位置
            }
           

            foreach (TileBase t in selected)
            {
                t.transform.position = t.transform.position + mouse_delta;
            }

            
        }


        tool_move.SetActive(selected.Count > 0);                                //激活移动工具

        tool_move.transform.position = select_center;
        m_last = m_world;
    }

    Vector3 now_grid_pos = Vector3.zero;
    Vector3 select_center = Vector3.zero;

    Vector3 GetGridPos(Vector3 pos, float g)                        // 获取网格对齐后的位置
    {
        Vector3 v1 = pos;
        Vector3 v2 = new Vector3(v1.x % g, v1.y % g, 0);
        v1 -= v2;
        if (Mathf.Abs(v2.x) > g * 0.5f)
        {
            v1.x += Mathf.Sign(v2.x) * g;
        }
        if (Mathf.Abs(v2.y) > g * 0.5f)
        {
            v1.y += Mathf.Sign(v2.y) * g;
        }
        return v1;
    }

    void calSelectedCenter()                                        // 计算选择物体的中心点位置
    {
        select_center = Vector3.zero;
        foreach (TileBase t in selected)
        {
            select_center += t.transform.localPosition / selected.Count;
        }
    }

    private bool select_dirty = false;                              // 是否选择

    public HashSet<TileBase> selected = new HashSet<TileBase>();    // 选择的物体集合
    void SelectObject(RaycastHit2D cast)                            // 选择物体
    {
        TileBase tile = null;
        if (cast.collider)                                          // 如果点击了物体
        {
            tile = cast.collider.GetComponent<TileBase>();          // 获取物体的TileBase组件
        }

        if (Input.GetKey(KeyCode.LeftShift))                        //多选
        {
            if (tile)
            {
                if (selected.Contains(tile))
                {
                    tile.SetHighLight(false);
                    selected.Remove(tile);
                }
                else
                {
                    tile.SetHighLight(true);
                    selected.Add(tile);
                }
            }
        }
        else                                                        //单选
        {
            unHighLightAllSelect();
            selected.Clear();
            if (tile)
            {
                tile.SetHighLight(true);
                selected.Add(tile);
            }
        }

        select_dirty = true;                                        //选择状态
    }

    void unHighLightAllSelect()                                     // 取消所有物体的高亮显示
    {
        foreach (TileBase t in selected)
        {
            t.SetHighLight(false);
        }
    }

    bool Grid = false;                                              //是否显示网格
    public void EnbaleGrid(bool e)                                  //设置网格可不可见
    {
        Grid = e;
    }

}
