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

public class GameManager : MonoBehaviourPun
{
    public TMP_Text text_playerList;

    static GameManager gameManager = null;

    struct TransformData
    {
        public Vector3 pos;
        public Vector3 rot;
    }

    [SerializeField]
    public Vector3[] PlayerPositions;

    [SerializeField]
    public Vector3[] PlayerRotations;

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

    public void PlayerDataSetting()
    {
    }
    void Start()
    {
        StartCoroutine(SpawnPlayer());

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

        Vector3 initPosition = PlayerPositions[playerCount - 1];
        Quaternion rotationQuaternion = Quaternion.Euler(PlayerRotations[playerCount - 1]);

        GameObject player = PhotonNetwork.Instantiate("Player", initPosition, rotationQuaternion);

        Debug.Log("현재 플레이어 생성");
    }

    void Update()
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

    private void OnDestroy()
    {
        Debug.Log("게임 매니저 삭제");
    }
}
