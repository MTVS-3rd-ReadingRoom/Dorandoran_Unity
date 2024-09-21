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

        // ��ǲ �ʵ� Submit �̺�Ʈ�� SendMyMessage ���ε�
        input_chat.onSubmit.AddListener(SendMyMessage);

        // ���� �ϴ����� Content �Ǻ� ����
        scrollChatRect.content.pivot = Vector2.zero;

        img_charbackground = scrollChatRect.transform.GetComponent<UnityEngine.UI.Image>();
        img_charbackground.color = new Color32(255, 255, 255, 10);
    }

    void SendMyMessage(string msg)
    {
        if (input_chat.text.Length > 0)
        {
            string currentTime = DateTime.Now.ToString("HH:mm:ss");

            // �̺�Ʈ�� ���� ����
            object[] sendContent = new object[] { playerChat, PhotonNetwork.NickName, msg, currentTime };

            // �۽� �ɼ�
            RaiseEventOptions eventOptions = new RaiseEventOptions();

            eventOptions.Receivers = ReceiverGroup.All;
            // eventOptions.CachingOption = EventCaching.DoNotCache;

            // ������ �̺�Ʈ�� �� ����ȭ
            // �̺�Ʈ �۽� ����
            PhotonNetwork.RaiseEvent(1, sendContent, eventOptions, SendOptions.SendUnreliable);

            print("Send!!");
            EventSystem.current.SetSelectedGameObject(null);
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
        // tap Ű - ��ǲ�ʵ带 Ȱ��ȭ
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
