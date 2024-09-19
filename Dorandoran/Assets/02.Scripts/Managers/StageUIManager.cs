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

    [Header("扁夯 UI")]
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

    [Header("规 沥焊")]
    public TMP_Text[] text_Topicinfos;

    [Header("格瞒")]
    public Panel_IndexUser infoIndexUserUI_Prefab;
    public RectTransform infoContent;
    public List<Panel_IndexUser> infoIndexUsers = new List<Panel_IndexUser>();
    public RectTransform infoIndexLast;


    [Header("可记")]
    public TMP_Dropdown dropdown_MicList;
    public Slider[] slider;
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
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    panel_End.transform.SetAsLastSibling();
        //    panel_End.SetActive(true);
        //}
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
    }

    private void InactiveUI()
    {
        if(inactiveStack.Count > 0)
        {
            Action action = inactiveStack.Pop();
            action.Invoke();
        }
    }

    #region 皋牢拳搁

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


    #region 林力
    public void SetTopic()
    {
        StartCoroutine(Corroutine_SetTopic());
    }

    private IEnumerator Corroutine_SetTopic()
    {
        int count = 0;
        print(0);
        while (DataManager.instance.topic.topic == null)
        {
            yield return new WaitForSeconds(1);
            count++;
            if(count > 30)
            {
                print(1);
                break;
            }
        }

        text_Topicinfos[0].text = DataManager.instance.topic.topic;
        text_Topicinfos[1].text = DataManager.instance.topic.content;
        text_Topic.text = DataManager.instance.topic.topic;
    }

    #endregion


    #region 格瞒

    public void AddInfoIndex_UserInfo(string name)
    {
        Panel_IndexUser ui = Instantiate(infoIndexUserUI_Prefab, infoContent);
        infoIndexUsers.Add(ui);
        ui.transform.SetAsLastSibling();
        infoIndexLast.transform.SetAsLastSibling();
        ui.text_UserName.text = name;
    }


    #endregion


    #region 可记

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
