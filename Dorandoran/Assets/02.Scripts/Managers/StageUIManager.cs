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

public class StageUIManager : MonoBehaviour
{
    public static StageUIManager instance;
    private Stack<Action> inactiveStack = new Stack<Action>();

    [Header("기본 UI")]
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
    private bool activeIndex = false;
    private bool activeInfo = false;

    [Header("방 정보")]
    public TMP_Text[] text_Topicinfos;

    [Header("목차")]
    public Panel_IndexUser infoIndexUserUI_Prefab;
    public RectTransform infoContent;
    public List<Panel_IndexUser> infoIndexUsers = new List<Panel_IndexUser>();
    public RectTransform infoIndexLast;


    [Header("옵션")]
    public TMP_Dropdown dropdown_MicList;
    public Slider[] slider_Sound;
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
        InitUI();
        SetTopic();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            panel_End.transform.SetAsLastSibling();
            panel_End.SetActive(!panel_End.activeSelf);
        }
    }
    private void InitUI()
    {
        buttons_PanelTags[0].onClick.AddListener(() => { OnClick_InfoButton(); });
        buttons_PanelTags[1].onClick.AddListener(() => { OnClick_IndexButton(); });
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
        //slider_Sound[2].value = 보이스 사운드 볼륨 적용;
        
        slider_Sound[0].onValueChanged.AddListener((value) => { SoundManager.instance.ChangeBGMVolum(value); });
        slider_Sound[1].onValueChanged.AddListener((value) => { SoundManager.instance.ChangeBGMVolum(value); });
        //slider_Sound[2].onValueChanged.AddListener((value) => { 보이스 사운드 조절 함수 추가 ); });
    }

    private void InactiveUI()
    {
        if(inactiveStack.Count > 0)
        {
            Action action = inactiveStack.Pop();
            action.Invoke();
        }
    }

    #region 메인화면

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

    public void SetActiveMicUI(bool value)
    {
        image_MicInactive.SetActive(value);
    }

    #endregion


    #region 주제

    public void SetTopic()
    {
        text_Topicinfos[0].text = DataManager.instance.topic.topic;
        text_Topicinfos[1].text = DataManager.instance.topic.content;
        text_Topic.text = DataManager.instance.topic.topic;
    }

    #endregion


    #region 목차

    public void AddInfoIndex_UserInfo(string name)
    {
        Panel_IndexUser ui = Instantiate(infoIndexUserUI_Prefab, infoContent);
        infoIndexUsers.Add(ui);
        ui.transform.SetAsLastSibling();
        infoIndexLast.transform.SetAsLastSibling();
        ui.text_UserName.text = name;
    }


    #endregion


    #region 옵션

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
