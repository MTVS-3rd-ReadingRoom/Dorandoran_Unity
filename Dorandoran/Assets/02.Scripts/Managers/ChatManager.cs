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
    enum AlignedText
    {
        Right,
        Left,
        AlignedTextEnd
    }

    public static ChatManager chatManager = null;

    public ScrollRect scrollChatRect;
    public TMP_Text text_chatContent;
    public TMP_InputField input_chat;

    UnityEngine.UI.Image img_charbackground;
    const byte chattingEvent = 1;
    public Photon.Voice.Unity.Recorder recorder;

    PhotonView pv;

    // ���� ��� ���� ���� - �ش� ��ũ��Ʈ ����
    PlayerProsAndCons.DebatePosition curDebatePosition;

    private void Awake()
    {
        if (null == chatManager)
        {
            chatManager = this;
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

        // ��ǲ �ʵ� Submit �̺�Ʈ�� SendMyMessage ���ε�
        input_chat.onSubmit.AddListener(SendMyMessage);

        // ���� �ϴ����� Content �Ǻ� ����
        scrollChatRect.content.pivot = Vector2.zero;

        img_charbackground = scrollChatRect.transform.GetComponent<UnityEngine.UI.Image>();
        img_charbackground.color = new Color32(255, 255, 255, 10);

        curDebatePosition = DebatePosition.DebatePositionEnd;
    }

    void SendMyMessage(string msg)
    {
        if (input_chat.text.Length > 0)
        {
            string currentTime = DateTime.Now.ToString("HH:mm:ss");

            // �̺�Ʈ�� ���� ����
            object[] sendContent = new object[] { curDebatePosition, PhotonNetwork.NickName, msg, currentTime };

            // �۽� �ɼ�
            RaiseEventOptions eventOptions = new RaiseEventOptions();


            eventOptions.Receivers = ReceiverGroup.Others;
            // eventOptions.CachingOption = EventCaching.DoNotCache;

            // ������ �̺�Ʈ�� �� ����ȭ
            // �̺�Ʈ �۽� ����
            PhotonNetwork.RaiseEvent(1, sendContent, eventOptions, SendOptions.SendUnreliable);

            print("Send!!");
            EventSystem.current.SetSelectedGameObject(null);

            string recieveMessage = $"\n[{sendContent[3].ToString()}]{sendContent[1].ToString()} : {sendContent[2].ToString()}";
            AddAlignedText(recieveMessage, AlignedText.Right);
        }
    }

    // ���� �濡 �ٸ� ����ڷκ��� �̺�Ʈ�� ���� ��� ����Ǵ� �Լ�
    public void OnEvent(EventData photonEvent)
    {
        print("Recieve");

        // ���� �̺�Ʈ�� ä�� �̺�Ʈ���
        if (photonEvent.Code == chattingEvent)
        {
            // ���� ������ "�г��� : ä�� ����" �������� ��ũ�Ѻ��� �ؽ�Ʈ�� ����
            object[] receiveObjects = (object[])photonEvent.CustomData;
            string recieveMessage = $"\n[{receiveObjects[3].ToString()}]{receiveObjects[1].ToString()} : {receiveObjects[2].ToString()}";

            if (!receiveObjects[0].Equals((int)curDebatePosition))
                return;
            AddAlignedText(recieveMessage, AlignedText.Left);
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
        // tap Ű - ��ǲ�ʵ带 Ȱ��ȭ
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            EventSystem.current.SetSelectedGameObject(input_chat.gameObject);
            input_chat.OnPointerClick(new PointerEventData(EventSystem.current));
            UnityEngine.Cursor.lockState = CursorLockMode.Confined;
            UnityEngine.Cursor.visible = true;
        }
    }

    // ���� �÷��̾��� ���� ���� �ֱ�
    public void SetCurChatProsAndConsData(DebatePosition debatePosition)
    {
        if (curDebatePosition == debatePosition)
            return;
        curDebatePosition = debatePosition;
    }

    void AddAlignedText(string text, AlignedText alignedText)
    {
        switch(alignedText)
        {
            case AlignedText.Right:
                text_chatContent.text += $"<align=right>{text}</align>\n";
                break;
            case AlignedText.Left:
                text_chatContent.text += $"<align=left>{text}</align>\n";
                break;
        }
        
    }
}
