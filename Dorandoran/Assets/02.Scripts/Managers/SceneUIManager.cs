using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Voice.Unity;
using Photon.Voice.PUN;

public class SceneUIManager : MonoBehaviourPunCallbacks
{
    public Photon.Voice.Unity.Recorder recorder;

    public TextMeshProUGUI orderText;
    public static SceneUIManager instance;

    public TextMeshProUGUI timeText;
    private double startTime;
    public double timeDuration = 120.0f;
    bool isRunning = false;

    int minute = 0;
    int second = 0;

    float maxTime;

    int Order;
    int playerIndex = 0;
    int playerN = 0;

    bool bStartTime = false;

    public TextMeshProUGUI logText;

    #region Panel
    [Header("순서UI")]
    public GameObject panel_Order;
    public GameObject panel_Timer;
    public Button button_Next;
    #endregion

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // 각 클라이언트마다
    // isMine만 해서 맞춰주자.
    // time과 speakerOrder만 맞춰주면 된다.

    // Start is called before the first frame update
    void Start()
    {
        Order = 0;
        playerIndex = 0;
        InitUI();
    }

    [PunRPC]
    void StartTimer()
    {
        startTime = PhotonNetwork.Time;
    }

    [PunRPC]
    public void ResetTimer()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // 서버 시간 기준으로 다시 설정
            photonView.RPC("StartTimer", RpcTarget.AllBuffered);
            // 모든 클라이언트에게 다시 새롭게 타이머 시작 시간 전송
        }
    }

    // 외부에서 리셋 호출
    public void TriggerReset()
    {
        // 마스터 클라이언트에서 ResetTimer 호출
        photonView.RPC("ResetTimer", RpcTarget.MasterClient);
    }

    public void InitUI()
    {
        button_Next.onClick.AddListener(NextOrderMessage);
    }

    public void UpdateTime()
    {


    }

    // Update is called once per frame
    void Update()
    {
        logText.text = Order + "번째 발표 순서입니다.\n + 현재 ActorNumber: " + PhotonNetwork.LocalPlayer.ActorNumber;
        if (isRunning && startTime > 0)
        {
            double elapsed = PhotonNetwork.Time - startTime;
            double remainingTime = timeDuration - elapsed;

            timeText.text = (int)(remainingTime / 60) + "분 " + (int)(remainingTime % 60) + "초 / 2분 제한시간";

            // 시간이 지남 or (키 누름 && 현재 차례인 플레이어가 눌렀을 경우) 0, 사회자, 2, 3 ~ 플레이어 수
            if (remainingTime < 0)
            {
                NextOrderPlayer();
            }
        }
    }

    void NextOrderMessage()
    {
        // 버튼을 누르면
        int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        Debug.Log("playerSpeak: " + Order + "ActorNumber: " + actorNumber);
        if (actorNumber == Order || Order <= 0)
            NextOrderPlayer();
    }

    public void NextOrderPlayer()
    {
        TriggerReset();
        RPCNextOrder();
        AllMuteTransmit();
        RPCSetTransmit();
    }

    // 버튼 동기화 - 버튼을 눌렀을때 isMine인 사용자에게 모두 해당 함수를 실행하도록 구현
    [PunRPC] // all로 뿌리고 isMine으로 1개만 실행되도록 구현
    public void NextOrder()
    {
        ++Order;
        Order = (Order % (playerN + 1)); // 0 ~ 2
        if (!isRunning) // 현재 실행 중이 아니면 실행 중으로 변경
        {
            isRunning = true;
            Order = 0;
        }
        // 여기서 모든 클라이언트 컴퓨터에게 해당 함수를 호출해줘야한다.
        playerN = PhotonNetwork.CurrentRoom.Players.Count;
        if (0 == Order) // 1번일 경우 사회자
        {
            panel_Timer.SetActive(false);
            orderText.text = "AI 사회자 시간입니다.";
        }
        else
        {
            panel_Timer.SetActive(true);
            orderText.text = (Order).ToString() + "번째 발표자 발언 시간입니다.";
        }

        Debug.Log(Order + "현재 발표 순서");
        Debug.Log(playerN + "명 존재");
    }

    public void RPCNextOrder()
    {
        photonView.RPC("NextOrder", RpcTarget.All);
    }

    [PunRPC]
    public void Toggle(bool toggle)
    {
        recorder.TransmitEnabled = toggle;
    }

    public void AllMuteTransmit()
    {
        playerN = PhotonNetwork.CurrentRoom.Players.Count;

        for (int i = 1; i < playerN + 1; i++)
        {
            photonView.RPC("Toggle", PhotonNetwork.CurrentRoom.GetPlayer(i), false);
        }
    }

    public void RPCSetTransmit()
    {
        photonView.RPC("Toggle", PhotonNetwork.CurrentRoom.GetPlayer(Order), true);
    }
}
