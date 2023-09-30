using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UIManager;

public class LevelManager : MonoBehaviour, IModuleSelection
{
    public GameObject currentSelectedModule; // 存储当前选中的模块

    public static LevelManager Instance;
    public Transform startPoint;            // 起点位置
    public Transform endPoint;              // 终点位置

    public TileBase[] registries;         // 地块注册表
    public List<TileBase> tiles;            // 瓦片



    float grid_space = 1f;                  // 网格间距

    [SerializeField]
    private GameObject tool_move;           // 移动工具

    [SerializeField]
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



    Vector3 drug_start_pos = Vector3.zero;
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
                drug_start_pos = m_world;
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
                //检测是否点击到了关卡物体
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

            if (mode == OptMode.Put)// 放置tile
            {
                print("put");
                float _x = m_world.x % grid_space;
                float _y = m_world.y % grid_space;
                float hs = grid_space / 2;

                if (_x > 0)
                {
                    if (_x > hs) _x = mouse_delta.x + 1 - _x;
                    else _x = mouse_delta.x - _x;
                }
                else
                {
                    if (_x < -hs) _x = mouse_delta.x + 1 - _x;
                    else _x = mouse_delta.x - _x;
                }

                
                if (_y > 0)
                {
                    if (_y > hs) _y = mouse_delta.y + 1 - _y;
                    else _y = mouse_delta.y - _y;
                }
                else
                {
                    if (_y < -hs) _y = mouse_delta.y + 1 - _y;
                    else _y = mouse_delta.y - _y;
                }

                GameObject obj = GetCurrentSelectedModule();
                TileBase t = obj.GetComponent<TileBase>();
                if(t)
                {
                    print("put2");
                    obj = GameObject.Instantiate(obj);
                    t = obj.GetComponent<TileBase>();
                    tiles.Add(t);
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
        }


        

        if (Input.GetMouseButton(0))                                            // 鼠标左键一直按下时
        {
            mouse_delta = m_world - drug_start_pos;

            float _x = mouse_delta.x % grid_space;
            float _y = mouse_delta.y % grid_space;
            float hs = grid_space / 2;

            if (_x > 0)
            {
                if (_x > hs) mouse_delta.x += 1 - _x;
                else mouse_delta.x -= _x;
            }
            else
            {
                if (_x < -hs) mouse_delta.x += 1 - _x;
                else mouse_delta.x -= _x;
            }


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

            foreach (TileBase t in selected)
            {
                t.transform.position = t.transform.position + mouse_delta;
            }


        }


        tool_move.SetActive(selected.Count > 0);                                //激活移动工具

        tool_move.transform.position = select_center;
        m_last = m_world;
    }

    Vector3 select_center = Vector3.zero;



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

    bool start = false;

    public bool IsPlaying { get { return start; } }
    public void Start_End()
    {
        UIManager.Instance.CloseGrid();
        UIManager.Instance.panel.SetActive(false);
        start = !start;
        if (start)
        {
            ScriptManager.Instance.CreatePlayer();
        }
        else
        {
            ScriptManager.Instance.DestroyPlayer();
            CameraContorller.Instance.LerpCam2Zero();
        }

    }



    public void SetSelectMode()
    {
        mode = OptMode.Select;
    }

    public void SetPutMode()
    {
        mode = OptMode.Put;
    }

    public void PutObject()
    {

    }

    public Vector3 GetGridPosition()
    {
        // 返回地图的位置信息
        return new Vector3(0, 0, 0); // 假设地图的位置为(0, 0, 0)
    }

    public Vector2Int GetGridSize()
    {
        // 返回地图的大小信息
        return new Vector2Int(15, 10); // 假设地图的大小为15x10
    }

    public float GetGridCellSize()
    {
        // 返回网格单元的大小
        return 1f; // 假设每个网格单元的大小为1
    }

    public GameObject GetCurrentSelectedModule()
    {
        return currentSelectedModule;
    }

}
