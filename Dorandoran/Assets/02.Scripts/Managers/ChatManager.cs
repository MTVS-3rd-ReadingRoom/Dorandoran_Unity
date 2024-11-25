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
using static PlayerProsAndCons;
using ExitGames.Client.Photon.StructWrapping;
using System.Xml;

public class ChatManager : MonoBehaviourPun, IOnEventCallback
{
    enum OwnerText
    {
        Mine,
        Other,
        OwnerTextEnd
    }


    public GameObject ChattingPanel;
    public UnityEngine.UI.Button chat_Send;
    public UnityEngine.UI.Button chat_Button;

    public static ChatManager chatManager = null;

    public ScrollRect scrollChatRect;
    public TMP_Text text_chatContent;
    public TMP_InputField input_chat;

    UnityEngine.UI.Image img_charbackground;
    const byte chattingEvent = 1;
    public Photon.Voice.Unity.Recorder recorder;

    PhotonView pv;

    // 현재 토론 찬반 정보 - 해당 스크립트 기준
    PlayerProsAndCons.DebatePosition curDebatePosition;

    bool bOnChattingPanel;

    private void Awake()
    {
        if (null == chatManager)
        {
            chatManager = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        } 
    }

    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    void Start()
    {
        pv = GetComponent<PhotonView>();
        input_chat.text = "";
        text_chatContent.text = "";

        bOnChattingPanel = false;

        // 인풋 필드 Submit 이벤트에 SendMyMessage 바인딩
        input_chat.onSubmit.AddListener(SendMyMessage);


        chat_Button.onClick.AddListener(OnChattingPanel);
        // 좌측 하단으로 Content 피봇 변경
        scrollChatRect.content.pivot = Vector2.zero;

        img_charbackground = scrollChatRect.transform.GetComponent<UnityEngine.UI.Image>();
        img_charbackground.color = new Color32(255, 255, 255, 200);

        curDebatePosition = DebatePosition.DebatePositionEnd;

        ChattingPanel.SetActive(false);

    }

    void SendMyMessage(string msg)
    {
        if (input_chat.text.Length > 0)
        {
            string currentTime = DateTime.Now.ToString("HH:mm:ss");

            // 이벤트에 보낼 내용
            object[] sendContent = new object[] { curDebatePosition, PhotonNetwork.NickName, msg, currentTime };

            // 송신 옵션
            RaiseEventOptions eventOptions = new RaiseEventOptions();


            eventOptions.Receivers = ReceiverGroup.Others;
            // eventOptions.CachingOption = EventCaching.DoNotCache;

            // 레이즈 이벤트로 값 동기화
            // 이벤트 송신 시작
            PhotonNetwork.RaiseEvent(1, sendContent, eventOptions, SendOptions.SendUnreliable);

            print("Send!!");
            EventSystem.current.SetSelectedGameObject(null);

            string recieveMessage = $"\n[{sendContent[3].ToString()}]{sendContent[1].ToString()} : {sendContent[2].ToString()}";
            AddAlignedText(recieveMessage, OwnerText.Mine);

            input_chat.text = "";
        }
    }

    // 같은 방에 다른 사용자로부터 이벤트가 왔을 경우 실행되는 함수
    public void OnEvent(EventData photonEvent)
    {

        // 받은 이벤트가 채팅 이벤트라면
        if (photonEvent.Code == chattingEvent)
        {
            // 받은 내용을 "닉네임 : 채팅 내용" 형식으로 스크롤뷰의 텍스트에 전달
            object[] receiveObjects = (object[])photonEvent.CustomData;
            string recieveMessage = $"\n[{receiveObjects[3].ToString()}]{receiveObjects[1].ToString()} : {receiveObjects[2].ToString()}";

            // 같은 팀이 아닐 경우
            if (!receiveObjects[0].Equals((int)curDebatePosition))
                return;
            AddAlignedText(recieveMessage, OwnerText.Other);

            StopAllCoroutines();
            StartCoroutine(AlphaReturn(2.0f));
        }

    }

    IEnumerator AlphaReturn(float time)
    {
        yield return new WaitForSeconds(time);
        img_charbackground.color = new Color32(255, 255, 255, 200);
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.RemoveCallbackTarget(this);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnChattingPanel()
    {
        bOnChattingPanel = !bOnChattingPanel;
        ChattingPanel.SetActive(bOnChattingPanel);
        if (bOnChattingPanel)
        {
            EventSystem.current.SetSelectedGameObject(input_chat.gameObject);
            input_chat.OnPointerClick(new PointerEventData(EventSystem.current));
            UnityEngine.Cursor.lockState = CursorLockMode.Confined;
            UnityEngine.Cursor.visible = true;
        }
    }

    // 현재 플레이어의 찬반 정보 넣기
    public void SetCurChatProsAndConsData(DebatePosition debatePosition)
    {
        if (curDebatePosition == debatePosition)
            return;
        curDebatePosition = debatePosition;
    }

    void AddAlignedText(string text, OwnerText ownerText)
    {
        switch(ownerText)
        {
            case OwnerText.Mine:
                text_chatContent.text += $"<align=right>{text}</align>\n";
                break;
            case OwnerText.Other:
                text_chatContent.text += $"<align=left>{text}</align>\n";
                break;
        }
    }
}
