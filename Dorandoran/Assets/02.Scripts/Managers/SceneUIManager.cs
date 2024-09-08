using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SceneUIManager : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI orderText;
    public static SceneUIManager instance;

    public TextMeshProUGUI timeText;
    private double startTime;
    public double timeDuration = 120.0f;
    bool isRunning = false;

    int minute = 0;
    int second = 0;

    float maxTime;

    int speakerOrder = 0;
    int playerN = 0;

    bool bStartTime = false;

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
        if (isRunning && startTime > 0)
        {
            double elapsed = PhotonNetwork.Time - startTime;
            double remainingTime = timeDuration - elapsed;

            timeText.text = (int)(remainingTime / 60) + "분 " + (int)(remainingTime % 60) + "초 / 2분 제한시간";

            if (remainingTime < 0)
            {
                NextOrderMessage(); // 다음 차례로
                TriggerReset();
            }
        }
    }

    void NextOrderMessage()
    {
        TriggerReset();
        RPCNextOrder();
    }

    // 버튼 동기화 - 버튼을 눌렀을때 isMine인 사용자에게 모두 해당 함수를 실행하도록 구현
    [PunRPC] // all로 뿌리고 isMine으로 1개만 실행되도록 구현
    public void NextOrder()
    {
        if(!isRunning) // 현재 실행 중이 아니면 실행 중으로 변경
        {
            isRunning = true;
        }

        // 여기서 모든 클라이언트 컴퓨터에게 해당 함수를 호출해줘야한다.
        playerN = PhotonNetwork.CurrentRoom.Players.Count;
        if (speakerOrder + 1 > playerN) // 발표자보다 크면
        {
            panel_Timer.SetActive(false);
            orderText.text = "AI 사회자 시간입니다.";
            speakerOrder = 0;

        }
        else
        {
            panel_Timer.SetActive(true);
            orderText.text = (speakerOrder + 1).ToString() + "번째 발표자 발언 시간입니다.";
            speakerOrder++; 
        }
    }

    public void RPCNextOrder()
    {
        if(photonView.IsMine)
        {
            photonView.RPC("NextOrder", RpcTarget.All);
        }
    }
}
