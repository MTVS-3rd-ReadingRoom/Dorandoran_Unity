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

    // public TextMeshProUGUI orderText;
    public static SceneUIManager instance;

    public TextMeshProUGUI timeText;
    public RectTransform timeBar;
    private double startTime;
    public double timeDuration = 120.0f;
    public bool isRunning = false;

    int minute = 0;
    int second = 0;

    float maxTime;

    public int timelineIndex = 0;
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

    bool end = false;
    #region Panel
    [Header("����UI")]
    public GameObject panel_Order;
    public GameObject panel_Timer;
    public Button button_Next;
    public Button button_Start;
    public Button button_TurnOver;
    #endregion

    bool isStart = false;
    public List<AudioClip> announcerVoices = new List<AudioClip>();

    private List<bool> turnOver;

    private int turnOverCount = 0;
    private bool isTurnOver = false;
    private Coroutine nextOrder;

    float timePercent = 0;

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
    // �� Ŭ���̾�Ʈ����
    // isMine�� �ؼ� ��������.
    // time�� speakerOrder�� �����ָ� �ȴ�.

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
                10, 120, 120,
                10, 120, 120,
                10, 120, 120,
                10, 120, 120
            };
        speakerIdList = new Chair[]
            {
                announcer_Chair,
                null, null, null,
                null, null, null,
                null, null, null,
                null, null, null
            };

        button_Next.gameObject.SetActive(false);
        button_TurnOver.gameObject.SetActive(false);
        if (PhotonNetwork.IsMasterClient)
        {
            button_Start.gameObject.SetActive(true);
        }
        else
        {
            button_Start.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // �ɾ��ִ� �÷��̾� üũ
        CheckSittingPlayer();
        // �÷��̾� ���� ������ üũ
        UpdatePlayerProsAndConsList();

        recorder_InterestText.text = "Recorder Interest Group: " + recorder.InterestGroup;

        // �ð�üũ
        if (isRunning && startTime > 0)
        {
            double elapsed = PhotonNetwork.Time - startTime;
            double remainingTime = timeDuration - elapsed;

            timeText.text = $"{(int)(remainingTime / 60)}�� {(int)(remainingTime % 60)}�� /{(int)(timeDuration / 60)}�� {(int)(timeDuration % 60)}��";
            timePercent = (float)(remainingTime / timeDuration);
            if (timePercent > 0)
            {
                timeBar.sizeDelta = new Vector2(400 * timePercent, timeBar.sizeDelta.y);
            }
            else
            {
                timeBar.sizeDelta = new Vector2(0, timeBar.sizeDelta.y);
            }
            timeBar.GetComponent<Image>().color = new Color(1, timePercent, timePercent);
            timeText.color = new Color(1, timePercent, timePercent);

            // �ð��� �����ٸ�
            if (remainingTime < 0)
            {
                startTime = 0;
                if (PhotonNetwork.IsMasterClient)
                {
                    RPC_NextTurnTrigger();
                }
            }
        }
    }
    public void InitUI()
    {
        button_Start.onClick.AddListener(StartDebate);
        button_Next.onClick.AddListener(RPC_NextTurnTrigger);
        button_TurnOver.onClick.AddListener(()=> { Event_TurnOver_Discussion(!isTurnOver); });
    }

    public void StartDebate()
    {
        print(checkSittingPlayer + " , " + isStart);
        if (checkSittingPlayer && !isStart)
        {
            button_Start.gameObject.SetActive(false);
            RPC_NextTurnTrigger();
        }
    }


    public void RPC_NextTurnTrigger()
    {
        photonView.RPC("NextTunrTrigger", RpcTarget.All);
    }

    [PunRPC]
    public void NextTunrTrigger()
    {
        switch (m_eCurCharacterTurn)
        {
            case CharacterTurn.CharacterHostTurn:
                HostTurn();
                break;
            default:
                StartCoroutine(Coroutine_NextOrder());
                break;
        }
    }

    public void HostTurn()
    {
        isRunning = true;
        panel_Timer.SetActive(false);
        // orderText.text = "AI ��ȸ�� �ð��Դϴ�.";
        m_eCurCharacterTurn = CharacterTurn.CharacterPlayerTurn;
        InitPlayerData();

    }

    private void InitPlayerData()
    {
        StartCoroutine(Coroutine_InitPlayerData());
    }

    IEnumerator Coroutine_InitPlayerData()
    {
        print("Coroutine_InitPlayerData");
        isStart = true;
        gameManager.StartCamera();
        yield return new WaitForSeconds(2.5f);

        #region ���� ����
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
        StageUIManager.instance.StartSetting(speakerIdList[2].playerName, speakerIdList[6].playerName, speakerIdList[3].playerName, speakerIdList[5].playerName);
        #endregion

        // �� �ø��� ��ȣ ���� �� �鿣�忡�� ����
        DataManager.instance.serial_Room = (int)PhotonNetwork.CurrentRoom.CustomProperties["SERIAL_ROOMNUM"];
        if (!PhotonNetwork.IsMasterClient)
        {
            HttpManager.instance.PostDedateRoom_User(DataManager.instance.serial_Room);
        }


        NextTunrTrigger();

    }

    IEnumerator Coroutine_NextOrder()
    {
        button_Next.gameObject.SetActive(false);
        button_Start.gameObject.SetActive(false);
        button_TurnOver.gameObject.SetActive(false);
        startTime = 0;
        if (timelineIndex != 0)
        {
            CinemachineManager.instance.AddInstructions();
            yield return new WaitForSeconds(3);
            CinemachineManager.instance.AddInstructions(1);
            yield return new WaitForSeconds(3);
            ModeratorSound.instance.SpeakPlayer(announcerVoices[timelineIndex + 2]);
            yield return new WaitForSeconds(5);
        }
        NextTurn();
    }

    public void NextTurn() // ������ ��
    {
        print("NextTurn : " + timelineIndex);
        // �ð� �� UI ����

        // �ó׸ӽ� ����ġ
        CinemachineManager.instance.AddInstructions();

        // ������ �� ������ �ʾҴٸ�
        if (timelineIndex < times.Length)
        {
            panel_Timer.SetActive(true);
            timeDuration = times[timelineIndex];
            TimeReset();
            // ȣ��Ʈ�� ������ 0���϶� �����̼� ���
            if (timelineIndex == 0 && PhotonNetwork.IsMasterClient)
            {
                StartCoroutine(StartVoice());
            }

            //  ������ �����ϴ����� �ִٸ� ���� �� ����Ʈ
            int temp = timelineIndex - 1;
            if (temp >= 0)
            {
                if (speakerIdList[temp] != announcer_Chair && speakerIdList[temp] != null)
                {
                    if (PhotonNetwork.LocalPlayer.ActorNumber == speakerIdList[temp].id)
                    {
                        DataManager.instance.StopRecord(DataManager.instance.serial_Room.ToString());
                    }
                }
            }

            // ���� ȸ�ǽð��� �� 
            if (timelineIndex == 1 || timelineIndex == 4 || timelineIndex == 7 || timelineIndex == 10)
            {
                m_eCurCharacterTurn = CharacterTurn.CharacterDebateTurn;
                CinemachineManager.instance.AddInstructions();
                button_Next.gameObject.SetActive(false);
                isTurnOver = false;
                button_TurnOver.GetComponent<Image>().color = new Color(1, 1, 1);
                button_TurnOver.gameObject.SetActive(true);
                DebatePlayer();
                StageUIManager.instance.SetActiveMicUI(true);
            }
            else
            {
                button_TurnOver.gameObject.SetActive(false);
                m_eCurCharacterTurn = CharacterTurn.CharacterPlayerTurn;
                AllMuteTransmit();
                if (speakerIdList[timelineIndex] != announcer_Chair && speakerIdList[timelineIndex] != null)
                {
                    // �߾��ڰ� ������ ���̽� ����
                    SetPlayerGroup_All();
                    RPCSetTransmit();

                    print($"{PhotonNetwork.LocalPlayer.ActorNumber} : {speakerIdList[timelineIndex].id}");
                    // �÷��̾ �߾����϶�
                    if (PhotonNetwork.LocalPlayer.ActorNumber == speakerIdList[timelineIndex].id)
                    {
                        DataManager.instance.RecordMicrophone(DataManager.instance.serial_Room.ToString());
                        button_Next.gameObject.SetActive(true);
                        StageUIManager.instance.SetActiveMicUI(true);
                    }
                    // �÷��̾ û���϶�
                    else
                    {
                        button_Next.gameObject.SetActive(false);
                        StageUIManager.instance.SetActiveMicUI(false);
                    }
                }

                // �߾��ڿ� ���� �ó׸ӽ� ����
                if (speakerIdList[timelineIndex] != null)
                {
                    CinemachineManager.instance.AddInstructions(speakerIdList[timelineIndex].virtualCameraIndex);
                }
                else
                {
                    CinemachineManager.instance.AddInstructions();
                }

            }

            // ���� UI Ȱ��ȭ
            StageUIManager.instance.PrintCurrentIndex(timelineIndex);
            timelineIndex++;
        }
        else if (!end)
        {
            print("End");
            panel_Timer.SetActive(false);
            end = true;
            if (PhotonNetwork.IsMasterClient)
            {
                HttpManager.instance.PostSummary(DataManager.instance.serial_Room.ToString());
            }
            SetPlayerGroup_All();
            StageUIManager.instance.panel_End.SetActive(true);
        }
    }


    IEnumerator StartVoice()
    {
        announcerVoices[1] = DataManager.instance.topicClip;
        ModeratorSound.instance.SpeakPlayer(announcerVoices[0]);
        yield return new WaitForSeconds(announcerVoices[0].length);
        ModeratorSound.instance.SpeakPlayer(announcerVoices[1]);
        yield return new WaitForSeconds(announcerVoices[1].length);
        ModeratorSound.instance.SpeakPlayer(announcerVoices[2]);
        yield return new WaitForSeconds(announcerVoices[2].length);
        RPC_NextTurnTrigger();
    }


    // �ܺο��� ���� ȣ��
    public void TimeReset()
    {
        startTime = PhotonNetwork.Time;
    }

    public void CheckSittingPlayer()
    {
        if (!isStart)
        {
            if (gameManager.CheckSittingPlayer())
            {
                gameManager.SetPlayerProsAndConsText();
                checkSittingPlayer = true;
            }
            else
            {
                checkSittingPlayer = false;
            }
        }
    }

    private void Event_TurnOver_Discussion(bool turnOver)
    {
        photonView.RPC("CheckTurnOver_Discussion", RpcTarget.MasterClient, turnOver);
        isTurnOver = turnOver;

        if (isTurnOver)
        {
            button_TurnOver.GetComponent<Image>().color = new Color(0.3f, 0.3f, 0.3f);
        }
        else
        {
            button_TurnOver.GetComponent<Image>().color = new Color(1, 1, 1);
        }
    }

    [PunRPC]
    public void CheckTurnOver_Discussion(bool turnOver)
    {
        if (turnOver)
        {
            turnOverCount++;
        }
        else
        {
            turnOverCount--;
        }

        if (turnOverCount >= PhotonNetwork.PlayerList.Length)
        {
            turnOverCount = 0;
            RPC_NextTurnTrigger();
        }
    }
 

    #region ���̽� �¿���

    [PunRPC]
    public void Toggle(bool toggle)
    {
        recorder.TransmitEnabled = toggle;
    }

    [PunRPC]
    public void SetSpeakGroup(byte groupID)
    {
        recorder.InterestGroup = groupID;
        
    }
    public void DebatePlayer()
    {
        AllSpeakTransmit();
        SetPlyaerSpeak_Group();
    }

    public void SetPlayerGroup_All()
    {
        for (int i = 1; i <= PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {
            RPCSetSpeakGroup(i, 0);
        }
    }

    public void SetPlyaerSpeak_Group()
    {
        for (int i = 0; i < oppositionSide.Count; i++)
        {
            print("oppositionSide : " + oppositionSide[i].name);
            RPCSetSpeakGroup(oppositionSide[i].id, 1);
        }
        for (int i = 0; i < propositionSide.Count; i++)
        {
            print("propositionSide : " + propositionSide[i].name);
            RPCSetSpeakGroup(propositionSide[i].id, 2);
        }
    }

    public void RPCSetSpeakGroup(int playerActor, byte GroupID)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("SetSpeakGroup", PhotonNetwork.CurrentRoom.GetPlayer(playerActor), GroupID);
        }
    }

    public void AllMuteTransmit()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 1; i < PhotonNetwork.CurrentRoom.Players.Count + 1; i++)
            {
                photonView.RPC("Toggle", PhotonNetwork.CurrentRoom.GetPlayer(i), false);
            }
        }
    }

    public void AllSpeakTransmit()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 1; i < PhotonNetwork.CurrentRoom.Players.Count + 1; i++)
            {
                photonView.RPC("Toggle", PhotonNetwork.CurrentRoom.GetPlayer(i), true);
            }
        }
    }

    public void RPCSetTransmit()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            print(speakerIdList[timelineIndex].id);
            photonView.RPC("Toggle", PhotonNetwork.CurrentRoom.GetPlayer(speakerIdList[timelineIndex].id), true);
        }
    }

    #endregion

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
                        tmpProsAndConsList[t].text = "���� ������:\n ����";
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
                        tmpProsAndConsList[t].text = "���� ������:\n �ݴ�";
                }
            }
        }
    }
}
