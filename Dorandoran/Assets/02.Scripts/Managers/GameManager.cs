using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using Photon.Pun.Demo.PunBasics;

public class GameManager : MonoBehaviourPun
{
    public TMP_Text text_playerList;

    static GameManager gameManager = null;

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
    void Start()
    {
        //if(gameManager != this)
        //        return;
        StartCoroutine(SpawnPlayer());

        // OnPhotonSerializeView ���� ������ ���� �� �� �����ϱ�(per seconds)
        PhotonNetwork.SerializationRate = 30;
        // ��κ��� ������ ���� �� �� �����ϱ�(per seconds)
        PhotonNetwork.SendRate = 30;

    }

    IEnumerator SpawnPlayer()
    {
        // �뿡 ������ �Ϸ�� ������ ��ٸ���.
        yield return new WaitUntil(() => { return PhotonNetwork.InRoom; });

        Vector2 randomPos = Random.insideUnitCircle * 5.0f;
        Vector3 initPosition = new Vector3(randomPos.x, 1.0f, randomPos.y);

        GameObject player = PhotonNetwork.Instantiate("Player", initPosition, Quaternion.identity);

        Debug.Log("���� �÷��̾� ����");
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
        Debug.Log("���� �Ŵ��� ����");
    }
}
