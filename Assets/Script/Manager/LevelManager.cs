using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    public Transform startPoint;
    public Transform endPoint;

    public GameObject[] registries;
    public List<TileBase> tiles;

    float grid_space = 1f;

    [SerializeField]
    private GameObject tool_move;

    private OptMode mode = OptMode.Select;
    Vector3 m_last;

    enum OptMode
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
    enum Dir
    {
        None, XY, X, Y
    }

    Dir m_dir = Dir.None;
    // Update is called once per frame
    void Update()
    {
        Vector3 m_world = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 mouse_delta = m_world - m_last;
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D cast_tool = Physics2D.Raycast(m_world, Vector2.zero, 0f, LayerMask.GetMask("Editor"));

            if (cast_tool.collider)
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
            else
            {
                m_dir = Dir.None;
                RaycastHit2D cast_level = Physics2D.Raycast(m_world, Vector2.zero, 0f, LayerMask.GetMask("Level"));

                switch (mode)
                {
                    case OptMode.Select:
                        SelectObject(cast_level);
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
            select_dirty = true;
        }

        if (select_dirty)
        {
            select_dirty = false;
            calSelectedCenter();
            now_grid_pos = GetGridPos(select_center + mouse_delta, grid_space);
        }

        if (Input.GetMouseButton(0))
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

            select_center += mouse_delta;
            if (Grid)
            {
                Vector3 v1 = GetGridPos(select_center, grid_space);
                mouse_delta = v1 - now_grid_pos;
                now_grid_pos = v1;
            }
           

            foreach (TileBase t in selected)
            {
                t.transform.position = t.transform.position + mouse_delta;
            }

            
        }


        tool_move.SetActive(selected.Count > 0);

        tool_move.transform.position = select_center;
        m_last = m_world;
    }

    Vector3 now_grid_pos = Vector3.zero;
    Vector3 select_center = Vector3.zero;

    Vector3 GetGridPos(Vector3 pos, float g)
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

    void calSelectedCenter()
    {
        select_center = Vector3.zero;
        foreach (TileBase t in selected)
        {
            select_center += t.transform.localPosition / selected.Count;
        }
    }

    private bool select_dirty = false;

    public HashSet<TileBase> selected = new HashSet<TileBase>();
    void SelectObject(RaycastHit2D cast)
    {
        TileBase tile = null;
        if (cast.collider)
        {
            tile = cast.collider.GetComponent<TileBase>();
        }

        if (Input.GetKey(KeyCode.LeftShift)) //多选
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
        else //单选
        {
            unHighLightAllSelect();
            selected.Clear();
            if (tile)
            {
                tile.SetHighLight(true);
                selected.Add(tile);
            }
        }

        select_dirty = true;
    }

    void unHighLightAllSelect()
    {
        foreach (TileBase t in selected)
        {
            t.SetHighLight(false);
        }
    }

    bool Grid = false;
    public void EnbaleGrid(bool e)
    {
        Grid = e;
    }

}
