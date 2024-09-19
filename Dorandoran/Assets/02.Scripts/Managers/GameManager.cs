using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using Photon.Pun.Demo.PunBasics;
using NumSystem = System.Numerics;
using UnityEngine.UIElements;
using Photon.Voice;
using ExitGames.Client.Photon.StructWrapping;

public class GameManager : MonoBehaviourPun
{
    public TMP_Text text_playerList;

    static GameManager gameManager = null;

    PhotonView pv;

    struct TransformData
    {
        public Vector3 pos;
        public Vector3 rot;
    }

    Camera mainCamera;
    Camera playerCamera;

    [SerializeField]
    public Vector3[] PlayerPositions;

    [SerializeField]
    public Vector3[] PlayerRotations;

    List<int> playerId;

    int playerSpeak = 0;


    bool sittingCheck;
    private void Awake()
    {
        if (null == gameManager)
        {
            gameManager = this;

            // DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public Vector3 GetPlayerPosition(int idx)
    {
        return PlayerPositions[idx];
    }

    public Vector3 GetPlayerRotation(int idx)
    {
        return PlayerRotations[idx];
    }

    public void PlayerDataSetting()
    {
    }
    void Start()
    {
        mainCamera = GameObject.Find("discussionPosition/Main Camera").GetComponent<Camera>();
        playerCamera = GameObject.Find("discussionPosition/Player Camera").GetComponent<Camera>();

        mainCamera.enabled = false;
        sittingCheck = false;
        pv = GetComponent<PhotonView>();
        StartCoroutine(SpawnPlayer()); // 0
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(SpawnTopicSpeeker()); // 1
        }

        // OnPhotonSerializeView 에서 데이터 전송 빈도 수 설정하기(per seconds)
        PhotonNetwork.SerializationRate = 30;
        // 대부분의 데이터 전송 빈도 수 설정하기(per seconds)
        PhotonNetwork.SendRate = 30;

    }

    IEnumerator SpawnPlayer()
    {
        // 룸에 입장이 완료될 때까지 기다린다.
        yield return new WaitUntil(() => { return PhotonNetwork.InRoom; });

        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;

        // Vector3 initPosition = PlayerPositions[playerCount - 1];
        // Quaternion rotationQuaternion = Quaternion.Euler(PlayerRotations[playerCount - 1]);

        GameObject player = PhotonNetwork.Instantiate("Player", new Vector3(530.0f, 4.5f, 210.0f), Quaternion.identity);
        Debug.Log("현재 플레이어 생성");
    }


    IEnumerator SpawnTopicSpeeker()
    {
        // 룸에 입장이 완료될 때까지 기다린다.
        yield return new WaitUntil(() => { return PhotonNetwork.InRoom; });

        Vector3 initPosition = new Vector3(534.0f, 4.5f, 204.88f);
        Quaternion rotationQuaternion = Quaternion.Euler(new Vector3(0.0f, 266.157f, 0.0f));

        GameObject Moderator = PhotonNetwork.Instantiate("Moderator", initPosition, rotationQuaternion);
        Debug.Log("현재 사회자 생성");
    }
    void Update()
    {

    }

    public void OnStaticCamera()
    {
        mainCamera.enabled = true;
        playerCamera.enabled = false;
    }

    public void OnPlayerCamera()
    {
        playerCamera.enabled = true;
        mainCamera.enabled = false;
    }
    void PrintPlayerList()
    {
        Dictionary<int, Player> playerDict = PhotonNetwork.CurrentRoom.Players;

        List<string> playerNames = new List<string>();

        foreach (KeyValuePair<int, Player> player in playerDict)
        {
            playerNames.Add(player.Value.NickName);
        }
        playerNames.Sort();

        text_playerList.text = "";
        foreach (string name in playerNames)
        {
            text_playerList.text += name + "\n";
        }
    }

    public bool CheckSittingPlayer()
    {
        sittingCheck = false;
        int maxPlayer = PhotonNetwork.CurrentRoom.PlayerCount;
        if (maxPlayer <= 0)
            return false;
        foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            foreach (PhotonView view in FindObjectsOfType<PhotonView>())
            {
                if(view.Owner != null)
                {
                    PlayerMove playerMove = view.GetComponentInChildren<PlayerMove>();
                    if(playerMove)
                    {
                        if (!playerMove.GetSitting())
                            return false;
                        else
                            sittingCheck = true;
                    }
                }
            }
        }
        return sittingCheck;
    }

    private void OnDestroy()
    {
        Debug.Log("게임 매니저 삭제");
    }
}
