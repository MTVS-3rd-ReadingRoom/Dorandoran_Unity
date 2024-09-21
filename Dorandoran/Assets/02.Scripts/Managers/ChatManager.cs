using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Realtime;
using UnityEngine.EventSystems;
using System;
using System.Data;
using UnityEngine.UIElements;

enum PlayerChat
{
    Agree,
    Disgree,
    PlayerChatEnd
}

public class ChatManager : MonoBehaviourPun, IOnEventCallback
{
    public ScrollRect scrollChatRect;
    public TMP_Text text_chatContent;
    public TMP_InputField input_chat;

    UnityEngine.UI.Image img_charbackground;
    const byte chattingEvent = 1;
    public Photon.Voice.Unity.Recorder recorder;

    PhotonView pv;

    PlayerChat playerChat;
    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    void Start()
    {
        pv = GetComponent<PhotonView>();
        playerChat = PlayerChat.Agree;
        input_chat.text = "";
        text_chatContent.text = "";

        // 인풋 필드 Submit 이벤트에 SendMyMessage 바인딩
        input_chat.onSubmit.AddListener(SendMyMessage);

        // 좌측 하단으로 Content 피봇 변경
        scrollChatRect.content.pivot = Vector2.zero;

        img_charbackground = scrollChatRect.transform.GetComponent<UnityEngine.UI.Image>();
        img_charbackground.color = new Color32(255, 255, 255, 10);
    }

    void SendMyMessage(string msg)
    {
        if (input_chat.text.Length > 0)
        {
            string currentTime = DateTime.Now.ToString("HH:mm:ss");

            // 이벤트에 보낼 내용
            object[] sendContent = new object[] { playerChat, PhotonNetwork.NickName, msg, currentTime };

            // 송신 옵션
            RaiseEventOptions eventOptions = new RaiseEventOptions();

            eventOptions.Receivers = ReceiverGroup.All;
            // eventOptions.CachingOption = EventCaching.DoNotCache;

            // 레이즈 이벤트로 값 동기화
            // 이벤트 송신 시작
            PhotonNetwork.RaiseEvent(1, sendContent, eventOptions, SendOptions.SendUnreliable);

            print("Send!!");
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    // 같은 방에 다른 사용자로부터 이벤트가 왔을 경우 실행되는 함수
    public void OnEvent(EventData photonEvent)
    {
        print("Recieve");

        // 받은 이벤트가 채팅 이벤트라면
        if (photonEvent.Code == chattingEvent)
        {
            // 받은 내용을 "닉네임 : 채팅 내용" 형식으로 스크롤뷰의 텍스트에 전달
            object[] receiveObjects = (object[])photonEvent.CustomData;
            string recieveMessage = $"\n[{receiveObjects[3].ToString()}]{receiveObjects[1].ToString()} : {receiveObjects[2].ToString()}";

            if (!receiveObjects[0].Equals((int)playerChat))
                return;
            text_chatContent.text += recieveMessage;
            input_chat.text = "";
        }
        img_charbackground.color = new Color32(255, 255, 255, 50);
        StopAllCoroutines();
        StartCoroutine(AlphaReturn(2.0f));
    }

    IEnumerator AlphaReturn(float time)
    {
        yield return new WaitForSeconds(time);
        img_charbackground.color = new Color32(255, 255, 255, 10);
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.RemoveCallbackTarget(this);
    }

    // Update is called once per frame
    void Update()
    {
        // tap 키 - 인풋필드를 활성화
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            EventSystem.current.SetSelectedGameObject(input_chat.gameObject);
            input_chat.OnPointerClick(new PointerEventData(EventSystem.current));
            UnityEngine.Cursor.lockState = CursorLockMode.Confined;
            UnityEngine.Cursor.visible = true;
        }

        if(Input.GetKeyDown(KeyCode.C) && pv.IsMine) // Changed
        {
            switch(playerChat)
            {
                case PlayerChat.Agree:
                    playerChat = PlayerChat.Disgree;
                    break;
                case PlayerChat.Disgree:
                    playerChat = PlayerChat.Agree;
                    break;
            }
        }
    }

}
