using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Voice.Unity;
using Photon.Voice.PUN;
using Photon.Pun.Demo.PunBasics;
using DG.Tweening.Core.Easing;


public class SceneUIManager : MonoBehaviourPunCallbacks
{
    enum CharacterTurn
    {
        CharacterHostTurn,
        CharacterPlayerTurn,
        CharacterDebateTurn,
        CharacterTurnEnd
    }
    public Photon.Voice.Unity.Recorder recorder;

    public Chair announcer_Chair;
    public Chair[] propositionSide_Chair;
    public Chair[] oppositionSide_Chair;
    public List<Chair> propositionSide = new List<Chair>();
    public List<Chair> oppositionSide = new List<Chair>();

    public static SceneUIManager instance;

    public TextMeshProUGUI timeText;
    private double startTime;
    public double timeDuration = 120.0f;
    bool isRunning = false;

    int minute = 0;
    int second = 0;

    float maxTime;

    //int PlayN;
    int timelineIndex = 0;
    float[] times;
    public Chair[] speakerIdList;

    int playerIndex = 0;

    bool bStartTime = false;

    public TextMeshProUGUI logText;

    public TextMeshProUGUI recorder_InterestText;
    CharacterTurn m_eCurCharacterTurn;

    GameManager gameManager;

    bool checkSittingPlayer;
    public TMP_Text[] tmpProsAndConsList;

    SceneNetworkManager sceneNetworkManager;
    #region Panel
    [Header("순서UI")]
    public GameObject panel_Order;
    public GameObject panel_Timer;
    public Button button_Next;
    #endregion

    // 클릭했을 때마다 증가
    void NextTurn() // 다음에 턴
    {
        CinemachineManager.instance.AddInstructions();
        if (timelineIndex < times.Length) // 총 플레이어 숫자만큼 증가했다면
        {
            print(timelineIndex);
            // orderText.text = StageUIManager.instance.PrintCurrentIndex(timelineIndex);
            if (timelineIndex == 1 || timelineIndex == 4 || timelineIndex == 7 || timelineIndex == 10)
            {
                m_eCurCharacterTurn = CharacterTurn.CharacterDebateTurn;
                DebatePlayer();
                CinemachineManager.instance.AddInstructions();
            }
            else
            {
                m_eCurCharacterTurn = CharacterTurn.CharacterPlayerTurn;
                AllMuteTransmit();
                if (speakerIdList[timelineIndex] != announcer_Chair && speakerIdList[timelineIndex] != null)
                {
                    RPCSetTransmit();
                }
                if (speakerIdList[timelineIndex] != null)
                {
                    CinemachineManager.instance.AddInstructions(speakerIdList[timelineIndex].virtualCameraIndex);
                }
                else
                {
                    CinemachineManager.instance.AddInstructions();
                }
                SetSameSpeakGroup();
            }
            timeDuration = times[timelineIndex];
            print(timelineIndex);
            timelineIndex++;
        }
    }
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
        m_eCurCharacterTurn = CharacterTurn.CharacterHostTurn;
        timelineIndex = 0;
        playerIndex = 0;
        checkSittingPlayer = false;

        sceneNetworkManager = GameObject.Find("SceneNetworkManager").GetComponentInChildren<SceneNetworkManager>();
        gameManager = GameObject.Find("GameManager").GetComponentInChildren<GameManager>();
        InitUI();

        times = new float[]
            {
                60, 
                180, 120, 120,
                180, 120, 120,
                180, 120, 120,
                180, 120, 120,
            };
        speakerIdList = new Chair[]
            {
                announcer_Chair,
                null, null, null,
                null, null, null,
                null, null, null,
                null, null, null,
            };
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

    public void CheckSittingPlayer()
    {
        if (!checkSittingPlayer)
        {
            if(gameManager.CheckSittingPlayer())
            {
                NextOrderMessage();
                gameManager.SetPlayerProsAndConsText();
                checkSittingPlayer = true;
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        // 앉아있는 플레이어 체크
        CheckSittingPlayer();
        // 플레이어 찬반 데이터 체크
        UpdatePlayerProsAndConsList();

        recorder_InterestText.text = "Recorder Interest Group: " + recorder.InterestGroup;

        //logText.text = PlayN + "번째 발표 순서입니다.\n + 현재 ActorNumber: " + PhotonNetwork.LocalPlayer.ActorNumber + "\n"
        //    + "레코더의 그룹: " + recorder.InterestGroup;
        if (isRunning && startTime > 0)
        {
            double elapsed = PhotonNetwork.Time - startTime;
            double remainingTime = timeDuration - elapsed;

            timeText.text = (int)(remainingTime / 60) + "분 " + (int)(remainingTime % 60) + $"초 / {timeDuration}초 제한시간";

            // 시간이 지남 or (키 누름 && 현재 차례인 플레이어가 눌렀을 경우) 0, 사회자, 2, 3 ~ 플레이어 수
            if (remainingTime < 0)
            {
                NextOrderRPC();
            }
        }
    }

    void NextOrderMessage()
    {
        switch(m_eCurCharacterTurn)
        {
            case CharacterTurn.CharacterHostTurn:
                HostTurnRPC();
                TriggerReset();
                break;
            case CharacterTurn.CharacterPlayerTurn:
                NextOrderRPC();
                break;
            case CharacterTurn.CharacterDebateTurn:
                NextOrderRPC();
                break;
        }
    }

    public void DiscussPlayer()
    {
        TriggerReset();
    }

    public void NextOrderRPC()
    {
        TriggerReset();
        RPCNextOrder();
    }

    public void DebatePlayer()
    {
        AllSpeakTransmit();
        DivideTwoSpeakGroup();
    }
    
    IEnumerator Coroutine_InitPlayerData()
    {
        gameManager.StartCamera();
        yield return new WaitForSeconds(2.5f);
        for (int i = 0; i < propositionSide_Chair.Length; i++)
        {
            if (propositionSide_Chair[i].sitting)
            {
                propositionSide.Add(propositionSide_Chair[i]);
            }
        }
        if (propositionSide.Count > 1)
        {
            speakerIdList[2] = propositionSide[0];
            speakerIdList[6] = propositionSide[1];
            speakerIdList[8] = propositionSide[0];
            speakerIdList[12] = propositionSide[1];
        }
        else if (propositionSide.Count == 1)
        {
            speakerIdList[2] = propositionSide[0];
            speakerIdList[6] = propositionSide[0];
            speakerIdList[8] = propositionSide[0];
            speakerIdList[12] = propositionSide[0];
        }

        for (int i = 0; i < oppositionSide_Chair.Length; i++)
        {
            if (oppositionSide_Chair[i].sitting)
            {
                oppositionSide.Add(oppositionSide_Chair[i]);
            }
        }
        if (oppositionSide.Count > 1)
        {
            speakerIdList[3] = oppositionSide[0];
            speakerIdList[5] = oppositionSide[1];
            speakerIdList[9] = oppositionSide[0];
            speakerIdList[11] = oppositionSide[1];
        }
        else if (oppositionSide.Count == 1)
        {
            speakerIdList[3] = oppositionSide[0];
            speakerIdList[5] = oppositionSide[0];
            speakerIdList[9] = oppositionSide[0];
            speakerIdList[11] = oppositionSide[0];
        }
        ModeratorSound.instance.SpeakPlayer(DataManager.instance.topicClip);
    }
    private void InitPlayerData()
    {
        StartCoroutine(Coroutine_InitPlayerData());
    }

    public void HostTurnRPC()
    {
        photonView.RPC("HostTurn", RpcTarget.All);
    }


    [PunRPC] // all로 뿌리고 isMine으로 1개만 실행되도록 구현
    public void HostTurn()
    {
        if (!isRunning) // 현재 실행 중이 아니면 실행 중으로 변경
            isRunning = true;
        panel_Timer.SetActive(false);
        // orderText.text = "AI 사회자 시간입니다.";
        m_eCurCharacterTurn = CharacterTurn.CharacterPlayerTurn;
        InitPlayerData();

    }

    // 버튼 동기화 - 버튼을 눌렀을때 isMine인 사용자에게 모두 해당 함수를 실행하도록 구현
    [PunRPC] // all로 뿌리고 isMine으로 1개만 실행되도록 구현
    public void NextOrder()
    {
        NextTurn();
        panel_Timer.SetActive(true);

        //Debug.Log(PlayN + "현재 발표 순서");
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

    [PunRPC]
    public void SpeakGroup(byte groupID)
    {
        recorder.InterestGroup = groupID;            
    }

    public void SetSameSpeakGroup()
    {
        for (int i = 1; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {
            RPCSetSpeakGroup(i, 0);
        }
    }

    public void DivideTwoSpeakGroup()
    {
        // 1 ~ 2 - 2명
        // 1 ~ 4 - 4명
        int PlayerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        for (int i = 1; i < PlayerCount / 2 + 1; i++)
        {
            RPCSetSpeakGroup(i, 1);
            RPCSetSpeakGroup((i + PlayerCount / 2), 2);
        }
    }

    public void RPCSetSpeakGroup(int playerActor, byte GroupID)
    {
        photonView.RPC("SpeakGroup", PhotonNetwork.CurrentRoom.GetPlayer(playerActor), GroupID);
    }

    public void AllMuteTransmit()
    {
        for (int i = 1; i < PhotonNetwork.CurrentRoom.Players.Count + 1; i++)
        {
            photonView.RPC("Toggle", PhotonNetwork.CurrentRoom.GetPlayer(i), false);
        }
    }

    public void AllSpeakTransmit()
    {
        for (int i = 1; i < PhotonNetwork.CurrentRoom.Players.Count + 1; i++)
        {
            photonView.RPC("Toggle", PhotonNetwork.CurrentRoom.GetPlayer(i), true);
        }
    }

    public void RPCSetTransmit()
    {
        photonView.RPC("Toggle", PhotonNetwork.CurrentRoom.GetPlayer(speakerIdList[timelineIndex].id), true);
    }

    public void UpdatePlayerProsAndConsList()
    {
        int[] actorList = sceneNetworkManager.actorList;
        for (int i = 0; i < propositionSide_Chair.Length; i++)
        {
            if (propositionSide_Chair[i].sitting)
            {
                for(int t = 0; t < 4; t++)
                {
                    if (propositionSide_Chair[i].id == actorList[t])
                        tmpProsAndConsList[t].text = "찬반 데이터:\n 찬성";
                }
                 
            }
        }

        for (int i = 0; i < oppositionSide_Chair.Length; i++)
        {
            if (oppositionSide_Chair[i].sitting)
            {
                for (int t = 0; t < 4; t++)
                {
                    if (oppositionSide_Chair[i].id == actorList[t])
                        tmpProsAndConsList[t].text = "찬반 데이터:\n 반대";
                }
            }
        }
    }
}
