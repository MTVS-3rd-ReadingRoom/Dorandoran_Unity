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

    List<int> playerId;

    int playerSpeak = 0;

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
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(SpawnTopicSpeeker());
        }

        // OnPhotonSerializeView ���� ������ ���� �� �� �����ϱ�(per seconds)
        PhotonNetwork.SerializationRate = 30;
        // ��κ��� ������ ���� �� �� �����ϱ�(per seconds)
        PhotonNetwork.SendRate = 30;

    }

    IEnumerator SpawnPlayer()
    {
        // �뿡 ������ �Ϸ�� ������ ��ٸ���.
        yield return new WaitUntil(() => { return PhotonNetwork.InRoom; });

        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;

        Vector3 initPosition = PlayerPositions[playerCount - 1];
        Quaternion rotationQuaternion = Quaternion.Euler(PlayerRotations[playerCount - 1]);

        GameObject player = PhotonNetwork.Instantiate("Player", initPosition, rotationQuaternion);
        Debug.Log("���� �÷��̾� ����");
    }


    IEnumerator SpawnTopicSpeeker()
    {
        // �뿡 ������ �Ϸ�� ������ ��ٸ���.
        yield return new WaitUntil(() => { return PhotonNetwork.InRoom; });

        Vector3 initPosition = new Vector3(534.0f, 4.5f, 204.88f);
        Quaternion rotationQuaternion = Quaternion.Euler(new Vector3(0.0f, 266.157f, 0.0f));

        GameObject Moderator = PhotonNetwork.Instantiate("Moderator", initPosition, rotationQuaternion);
        Debug.Log("���� ��ȸ�� ����");
    }
    void Update()
    {
        // PrintPlayerList();
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


    private void OnDestroy()
    {
        Debug.Log("���� �Ŵ��� ����");
    }
}
