namespace UIs
{
    using LitMotion;
    using System;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;
    using FNet;
    using static Player.Player_Controle;

    public class UI_Comment_System : MonoBehaviour
    {
        [Header("◆メインオプション")]
        [SerializeField] RectTransform rectTrans;
        [SerializeField] Fusion_Chat FChat;
        [SerializeField, Tooltip("サイズ拡大(縦)")]
        int sizeChange;
        [SerializeField, Tooltip("移動時間")]
        float moveTime;
        [SerializeField]
        TMP_InputField inputField;
        [SerializeField, Tooltip("開閉オブジェクト")]
        GameObject openRect;

        // Logのオプション
        [SerializeField, Tooltip("スクロールビュー")]
        ScrollRect scrollRect;
        float savedScrollY = 0f;

        bool nowAnime = false;
        public bool isOpen = false;
        Vector2 defaultSize;

        void Start()
        {
            defaultSize = rectTrans.sizeDelta;
            openRect.SetActive(isOpen);
        }

        void Update()
        {
            if (PCont.PI.actions["Chat"].triggered)
            {
                Chats();
            }
        }
        public void ButtonChat()
        {
            Chats();
        }
        void Chats()
        {
            if (nowAnime) return;
            if (inputField.text != "")
            {
                //文字送信
                FChat.SendChat();
                EventSystem.current.SetSelectedGameObject(inputField.gameObject);
                inputField.ActivateInputField();
            }
            else
            {
                nowAnime = true;
                isOpen = !isOpen;
                if (!isOpen)
                {
                    savedScrollY = scrollRect.verticalNormalizedPosition;
                }
                OpenSystem();
            }
        }
        // コメントの開閉
        void OpenSystem()
        {
            // 開く
            if (isOpen)
            {
                openRect.SetActive(isOpen);

                LMotion.Create(defaultSize.y, defaultSize.y + sizeChange, moveTime)
                    .WithOnComplete(() => nowAnime = false)
                    .Bind(y => rectTrans.sizeDelta = new Vector2(defaultSize.x, y));

                EventSystem.current.SetSelectedGameObject(inputField.gameObject);
                inputField.ActivateInputField();

            }
            // 閉じる
            else
            {
                LMotion.Create(defaultSize.y + sizeChange, defaultSize.y, moveTime)
                    .WithOnComplete(() =>
                    {
                        rectTrans.sizeDelta = new Vector2(defaultSize.x, defaultSize.y);
                        openRect.SetActive(isOpen);

                        Canvas.ForceUpdateCanvases();
                        scrollRect.verticalNormalizedPosition = 0f;
                        nowAnime = false;
                    })
                    .Bind(y =>
                    {
                        rectTrans.sizeDelta = new Vector2(defaultSize.x, y);

                        // スクロールを下に固定
                        scrollRect.verticalNormalizedPosition = 0f;
                    });
            }
        }

    }
}

