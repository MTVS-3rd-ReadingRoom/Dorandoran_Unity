using Photon.Voice.Unity.Demos;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG;
using DG.Tweening;
using System;
using Photon.Pun;

public class StageUIManager : MonoBehaviourPun
{
    public static StageUIManager instance;
    private Stack<Action> inactiveStack = new Stack<Action>();

    public string[] nickName = new string[] { "", "", "", "" };
    private string[] indexString = new string[]
        {
            "토론 시작",
            "1차 내부토의 시작!", $"찬성측\n입론을 시작해주세요", $"반대측\n입론을 시작해주세요",
            "2차 내부토의 시작!", $"반대측\n반론을 시작해주세요", $"찬성측\n반론을 시작해주세요",
            "3차 내부토의 시작!", $"찬성측\n반론을 시작해주세요", $"반대측\n반론을 시작해주세요",
            "최종 내부토의 시작!", $"반대측\n주장 정리 및 결론", $"찬성측\n주창 정리 및 결론"
        };

    [Header("메인 UI")]
    public TMP_Text text_Topic;
    public RectTransform[] infoPanels;
    public Button[] buttons_PanelTags;
    public Button[] buttons_Inactive;
    public Button button_Option;
    public Button button_Quit;
    public GameObject panel_Option;
    public GameObject panel_Quit;
    public GameObject panel_End;
    public GameObject image_MicInactive;
    public RectTransform panel_CurrentOrder;
    private bool activeIndex = false;
    private bool activeInfo = false;

    [Header("Topic")]
    public TMP_Text[] text_Topicinfos;

    [Header("정보")]
    public Panel_IndexUser infoIndexUserUI_Prefab;
    public RectTransform infoContent;
    public List<Panel_IndexUser> infoIndexUsers = new List<Panel_IndexUser>();
    public RectTransform infoIndexLast;


    [Header("Option")]
    public TMP_Dropdown dropdown_MicList;
    public Slider[] slider_Sound;

    Sequence uiSequence;

    public Button[] buttons_LeaveRoom;

    PhotonView pv;

    public RectTransform[] panel_IndexUI;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    
    private void Start()
    {
        pv = GetComponent<PhotonView>();
        InitUI();
        SetTopic();
    }
    public bool test = false;
    private void Update()
    {
        if (test)
        {

            test = false;
            uiSequence = Move_PanelCurrentIndex();
        }
    }

    private void InitUI()
    {
        buttons_PanelTags[0].onClick.AddListener(() => { OnClick_InfoButton(); });
        buttons_PanelTags[1].onClick.AddListener(() => { OnClick_IndexButton(); });
        buttons_PanelTags[2].onClick.AddListener(() => { OnClick_TopicButton(); });
        button_Option.onClick.AddListener(() => { ActiveSelectMicrophoneUI(); panel_Option.transform.SetAsLastSibling(); panel_Option.SetActive(true); inactiveStack.Push(() => { panel_Option.SetActive(false); }); });
        button_Quit.onClick.AddListener(() => { panel_Quit.transform.SetAsLastSibling(); panel_Quit.SetActive(true); inactiveStack.Push(() => { panel_Quit.SetActive(false); }); });
        dropdown_MicList.onValueChanged.AddListener(delegate { SelectMicrophone(); });
        dropdown_MicList.GetComponentInChildren<Button>().onClick.AddListener(() => ActiveSelectMicrophoneUI());

        for (int i = 0; i < buttons_Inactive.Length; i++)
        {
            buttons_Inactive[i].onClick.AddListener(() => { InactiveUI(); });
        }
        slider_Sound[0].value = SoundManager.instance.bgmVolume;
        slider_Sound[1].value = SoundManager.instance.sfxVolume;
        //slider_Sound[2].value = ���̽� ���� ���� ����;
        
        slider_Sound[0].onValueChanged.AddListener((value) => { SoundManager.instance.ChangeBGMVolum(value); });
        slider_Sound[1].onValueChanged.AddListener((value) => { SoundManager.instance.ChangeBGMVolum(value); });
        //slider_Sound[2].onValueChanged.AddListener((value) => { ���̽� ���� ���� �Լ� �߰� ); });

        for (int i = 0; i < buttons_LeaveRoom.Length; i++)
        {
            buttons_LeaveRoom[i].onClick.AddListener(()=> { NetworkManager.instance.LeaveRoom(); });
        }
    }


    private void InactiveUI()
    {
        if(inactiveStack.Count > 0)
        {
            Action action = inactiveStack.Pop();
            action.Invoke();
        }
    }


    public void StartSetting(string team1_NickName1, string team1_NickName2, string team2_NickName1, string team2_NickName2)
    {
        nickName = new string[] { team1_NickName1, team1_NickName2, team2_NickName1, team2_NickName2 };
        indexString = new string[]
        {
            "토론 시작",
            "1차 내부토의 시작!", $"찬성측({nickName[0]})\n입론을 시작해주세요", $"반대측({nickName[2]})\n입론을 시작해주세요", 
            "2차 내부토의 시작!", $"반대측({nickName[3]})\n반론을 시작해주세요", $"찬성측({nickName[1]})\n반론을 시작해주세요",
            "3차 내부토의 시작!", $"찬성측({nickName[0]})\n반론을 시작해주세요", $"반대측({nickName[2]})\n반론을 시작해주세요", 
            "최종 내부토의 시작!", $"반대측({nickName[3]})\n주장 정리 및 결론", $"찬성측({nickName[1]})\n주창 정리 및 결론"
        };
        panel_IndexUI[2].GetComponentInChildren<TMP_Text>().text += $"\n + ({nickName[0]})";
        panel_IndexUI[3].GetComponentInChildren<TMP_Text>().text += $"\n + ({nickName[2]})";
        panel_IndexUI[5].GetComponentInChildren<TMP_Text>().text += $"\n + ({nickName[3]})";
        panel_IndexUI[6].GetComponentInChildren<TMP_Text>().text += $"\n + ({nickName[1]})";
        panel_IndexUI[8].GetComponentInChildren<TMP_Text>().text += $"\n + ({nickName[0]})";
        panel_IndexUI[9].GetComponentInChildren<TMP_Text>().text += $"\n + ({nickName[2]})";
        panel_IndexUI[11].GetComponentInChildren<TMP_Text>().text += $"\n + ({nickName[3]})";
        panel_IndexUI[12].GetComponentInChildren<TMP_Text>().text += $"\n + ({nickName[1]})";
    }


    public string PrintCurrentIndex(int index)
    {
        if(index < indexString.Length)
        {
            string text = $"{indexString[index]}";
            panel_CurrentOrder.GetComponentInChildren<TMP_Text>().text = text;
            if (uiSequence != null)
                uiSequence.Kill();
            uiSequence = Move_PanelCurrentIndex();
            for (int i = 0; i < panel_IndexUI.Length; i++)
            {
                panel_IndexUI[i].GetComponent<Image>().color = Color.white;
            }
            panel_IndexUI[index].GetComponent<Image>().color = Color.green;
            return text;
        }
        return null;
    }

    Sequence Move_PanelCurrentIndex()
    {
        return DOTween.Sequence()
        .OnStart(() =>
        {
            panel_CurrentOrder.anchoredPosition3D = new Vector3(-1600, 0, 0);
        })
        .Append(panel_CurrentOrder.DOAnchorPos3DX(0, 0.5f).SetEase(Ease.OutSine))
        .Append(panel_CurrentOrder.DOAnchorPos3DX(1600, 0.5f).SetEase(Ease.InSine).SetDelay(1));
    }

    #region ����ȭ��

    private void OnClick_InfoButton()
    {
        infoPanels[0].transform.SetAsLastSibling();
        if (activeInfo)
        {
            activeInfo = false;
            infoPanels[0].DOAnchorPos3DX(-1300, 0.25f).SetEase(Ease.InSine);
        }
        else
        {
            activeInfo = true;
            infoPanels[0].DOAnchorPos3DX(0, 0.25f).SetEase(Ease.InSine);
        }
    }

    private void OnClick_IndexButton()
    {
        infoPanels[1].transform.SetAsLastSibling();
        if (activeIndex)
        {
            activeIndex = false;
            infoPanels[1].DOAnchorPos3DX(-400, 0.25f).SetEase(Ease.InSine);
        }
        else
        {
            activeIndex = true;
            infoPanels[1].DOAnchorPos3DX(0, 0.25f).SetEase(Ease.InSine);
        }
    }

    private void OnClick_TopicButton()
    {
        infoPanels[2].transform.SetAsLastSibling();
        if (activeIndex)
        {
            activeIndex = false;
            infoPanels[2].DOAnchorPos3DX(-1000, 0.25f).SetEase(Ease.InSine);
        }
        else
        {
            activeIndex = true;
            infoPanels[2].DOAnchorPos3DX(0, 0.25f).SetEase(Ease.InSine);
        }
    }

    public void SetActiveMicUI(bool value)
    {
        image_MicInactive.SetActive(value);
    }

    #endregion


    #region ����

    [PunRPC]
    public void SendTopic(string text1, string text2, string text3)
    {
        text_Topicinfos[0].text = text1;
        text_Topicinfos[1].text = text2;
        text_Topic.text = text3;
    }

    public void SetTopic()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            text_Topicinfos[0].text = DataManager.instance.topic.topic;
            text_Topicinfos[1].text = DataManager.instance.topic.opposition;
            //text_Topicinfos[2].text = DataManager.instance.topic.proposition;
            text_Topic.text = DataManager.instance.topic.topic;
            pv.RPC("SendTopic", RpcTarget.AllBuffered, text_Topicinfos[0].text, text_Topicinfos[1].text, text_Topic.text);
        }
    }
    public void SetTopicRPC()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            pv.RPC("SetTopic", RpcTarget.MasterClient);
            // 모든 클라이언트에게 전송
        }
    }

    #endregion


    #region ����

    public void AddInfoIndex_UserInfo(string name)
    {
        Panel_IndexUser ui = Instantiate(infoIndexUserUI_Prefab, infoContent);
        infoIndexUsers.Add(ui);
        ui.transform.SetAsLastSibling();
        infoIndexLast.transform.SetAsLastSibling();
        ui.text_UserName.text = name;
    }


    #endregion


    #region �ɼ�

    private void ActiveSelectMicrophoneUI()
    {
        dropdown_MicList.ClearOptions();
        dropdown_MicList.AddOptions(Microphone.devices.ToList());
        dropdown_MicList.value = DataManager.instance.microphoneIndex;
    }

    private void SelectMicrophone()
    {
        DataManager.instance.microphoneIndex = dropdown_MicList.value;
    }

    #endregion
}
