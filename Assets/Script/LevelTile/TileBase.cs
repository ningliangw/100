using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileBase : MonoBehaviour
{
    public string tileName;        // ��ͼ�������
    public string detail;

    private bool isHighlighted;    // ��ͼ���Ƿ񱻸�����ʾ

    public Sprite imgOnGui;        //�ŵ���ͼUI����


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

        // ������Ҫ���õ�ͼ��ĸ�����ʾЧ��
        if (isHighlighted)
        {
            // ���ø�������ɫ�����ʵȵ�
            GetComponent<Renderer>().material.color = Color.yellow;
        }
        else
        {
            // �ָ���������ɫ�����ʵȵ�
            GetComponent<Renderer>().material.color = Color.white;
        }
    }

    public virtual void OnStart()
    {

    }

    public virtual void OnEnd(bool sucess)
    {

    }
}
