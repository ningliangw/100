using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TileRegistryUIManager : MonoBehaviour
{
    public GameObject scrollViewContent; // Scroll View�����ķ�Χ
    public GameObject tileButtonPrefab;  // ��ť


    private TileBase[] registries; // �ؿ�ע���

    public static TileRegistryUIManager Instance;

    public LevelManager levelManager;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        registries = LevelManager.Instance.registries;

        float currentButtonY = 0f; // ����׷�ٰ�ť�ĵ�ǰ��ֱλ��

        for (int i = 0; i < registries.Length; i++)
        {
            GameObject tileButton = Instantiate(tileButtonPrefab, scrollViewContent.transform);

            // �ֶ�Ųλ�ã����������ƺ��ڹ��ַ���������
            RectTransform buttonRectTransform = tileButton.GetComponent<RectTransform>();
            buttonRectTransform.localPosition = new Vector3(80 + 140 * i, 0, 0);


            //buttonRectTransform.anchoredPosition = new Vector2(buttonRectTransform.anchoredPosition.x, 80 - currentButtonY);

            // ��ȡ��ť�ϵ�Text���������
            Text buttonText = tileButton.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                buttonText.text = registries[i].tileName;
            }

           

            // ��ȡ��ť�ϵ�TileOnGUI���
            TileOnGUI tileOnGUIComponent = tileButton.GetComponent<TileOnGUI>();
            if (tileOnGUIComponent)
            {
                tileOnGUIComponent.SetId(i);
                
                Image img = tileButton.GetComponentInChildren<Image>();
                img.sprite = tileOnGUIComponent.GetTile().imgOnGui;

                Button button = tileButton.GetComponentInChildren<Button>();
                if (button)
                {
                    button.onClick.AddListener(() =>
                    {
                        //UIManager.Instance.OnPutClick();
                        UIManager.Instance.OnObstacleClick(tileOnGUIComponent.GetTile());
                    });
                }
            }

            // ��ť��ļ��
            currentButtonY += buttonRectTransform.rect.height + 35f;

        }
        tileButtonPrefab.SetActive(false);
    }



}
