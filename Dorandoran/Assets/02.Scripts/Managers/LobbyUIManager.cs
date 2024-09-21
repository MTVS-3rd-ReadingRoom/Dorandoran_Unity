using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUIManager : MonoBehaviour
{
    public static LobbyUIManager instance;

    private Stack<Action> inactiveStack = new Stack<Action>();
    public Button[] buttons_Inactive;

    #region Panel

    [Header("페이지")]
    public GameObject panel_login;
    public GameObject panel_signUp;
    public GameObject panel_SelectChannel;
    public GameObject panel_makeRoom;
    public GameObject panel_choiceRoom;
    public GameObject panel_PopUp;
    public GameObject panel_HttpLoad;
    public GameObject panel_SceneLoad;
    public GameObject panel_Option;
    public GameObject panel_Quit;

    #endregion

    #region 로그인
    [Header("로그인")]
    public TMP_InputField[] inputFields_LogIn;
    public Button button_LogIn;
    public Button button_StartSignUp;
    #endregion

    #region 회원가입
    [Header("회원가입")]
    public TMP_InputField[] inputFields_SignUp;
    public Button button_SignUp;
    public Button button_SignUpCancle;
    #endregion

    #region 채널 선택
    [Header("채널 선택")]
    public Button[] buttons_MakeRoomChannel;
    public Button[] buttons_ChoiceRoomChannel;
    #endregion

    #region 방 만들기
    [Header("방 만들기")]
    public Button button_CreateRoom;

    #endregion

    #region 팝업

    [Header("팝업")]
    public Button[] button_Exit;
    public TMP_Text text_Error;
    public RectTransform image_HttpLoading;
    private Coroutine roateLoading;

    #endregion

    #region 옵션
    [Header("옵션")]
    public Button button_Option;
    public Button button_Quit;
    public Button button_QuitGame;
    public TMP_Dropdown dropdown_MicList;
    public Slider[] slider_Sound;
    #endregion

    public Coroutine getTopic;

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

    void Start()
    {
        InitUI();
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        if (instance == this)
            instance = null;
    }

    public void InitUI()
    {
        button_LogIn.onClick.AddListener(() =>
        {
            if (CheckLogIn())
            {
                print("LogIn_Success");
                HttpManager.instance.PostLogIn_FormData(inputFields_LogIn[0].text, inputFields_LogIn[1].text);
            }
            else
            {
                print("LogIn_False");
            }
        });
        button_StartSignUp.onClick.AddListener( () => { ShowSignUpPanel(); });
        button_SignUp.onClick.AddListener(() =>
        {
            if (CheckSignUp())
            {
                print("SignUp_Success");
                HttpManager.instance.PostSignUp_FormData(inputFields_SignUp[0].text, inputFields_SignUp[1].text, inputFields_SignUp[2].text, inputFields_SignUp[3].text);
            }
            else
            {
                print("SignUp_False");
            }
        });
        button_SignUpCancle.onClick.AddListener(() => { ShowLogInPanel(); });
        button_CreateRoom.onClick.AddListener(() => { CreateRoom(); });
        for (int i = 0; i < buttons_MakeRoomChannel.Length; i++)
        {
            buttons_MakeRoomChannel[i].onClick.AddListener(() => { ShowMakeRoomPanel(); });
        }
        for (int i = 0; i < buttons_ChoiceRoomChannel.Length; i++)
        {
            buttons_ChoiceRoomChannel[i].onClick.AddListener(() => { ShowChoiceRoomPanel(); });
        }
        for (int i = 0; i < button_Exit.Length; i++)
        {
            button_Exit[i].onClick.AddListener(() => { panel_PopUp.SetActive(false); });
        }
        for (int i = 0; i < buttons_Inactive.Length; i++)
        {
            buttons_Inactive[i].onClick.AddListener(() => { InactiveUI(); });
        }

        button_Option.onClick.AddListener(() => { ActiveSelectMicrophoneUI(); panel_Option.SetActive(true); inactiveStack.Push(() => { panel_Option.SetActive(false); }); });
        button_Quit.onClick.AddListener(() => { panel_Quit.SetActive(true); inactiveStack.Push(() => { panel_Quit.SetActive(false); }); });
        button_QuitGame.onClick.AddListener(() => { Application.Quit(); });

        dropdown_MicList.onValueChanged.AddListener(delegate { SelectMicrophone(); });
        dropdown_MicList.GetComponentInChildren<Button>().onClick.AddListener(() => ActiveSelectMicrophoneUI());

        slider_Sound[0].value = SoundManager.instance.bgmVolume;
        slider_Sound[1].value = SoundManager.instance.sfxVolume;
        //slider_Sound[2].value = 보이스 사운드 볼륨 적용;

        slider_Sound[0].onValueChanged.AddListener((value) => { SoundManager.instance.ChangeBGMVolum(value); });
        slider_Sound[1].onValueChanged.AddListener((value) => { SoundManager.instance.ChangeBGMVolum(value); });
        //slider_Sound[2].onValueChanged.AddListener((value) => { 보이스 사운드 조절 함수 추가 ); });

      
    }
    private void InactiveUI()
    {
        if (inactiveStack.Count > 0)
        {
            Action action = inactiveStack.Pop();
            action.Invoke();
        }
    }

    public bool CheckLogIn()
    {
        for (int i = 0; i < inputFields_LogIn.Length; i++)
        {
            if (string.IsNullOrEmpty(inputFields_LogIn[i].text))
            {
                if (i == 0)
                    ShowPopUp("아이디를 입력해 주세요.");
                else
                    ShowPopUp("비밀번호를 입력해 주세요.");
                return false;
            }
        }

        return true;
    }

    public bool CheckSignUp()
    {
        for (int i = 0; i < inputFields_SignUp.Length; i++)
        {
            if (string.IsNullOrEmpty(inputFields_SignUp[i].text))
            {
                if (i == 0)
                    ShowPopUp("이름을 입력해 주세요.");
                else if (i == 1)
                    ShowPopUp("아이디를 입력해 주세요.");
                else if (i == 2) 
                    ShowPopUp("비밀번호를 입력해 주세요.");
                else
                    ShowPopUp("닉네임을 입력해 주세요.");
                return false;
            }
        }

        return true;
    }

    public void StopRoateLoading()
    {
        if (roateLoading != null)
        {
            panel_HttpLoad.SetActive(false);
            StopCoroutine(roateLoading);
            roateLoading = null;
        }
    }

    private IEnumerator RoateLoading()
    {
        while (true)
        {
            image_HttpLoading.Rotate(-Vector3.forward * Time.deltaTime * 50);
            yield return new WaitForEndOfFrame();
        }
    }

    private void CreateRoom()
    {
        if (getTopic != null)
            return;

        DataManager.instance.topic.topic = null;
        DataManager.instance.topic.content = null;
        DataManager.instance.topicClip = null;
        NetworkManager.instance.CreateRoom();
        //HttpManager.instance.PostDedateRoom(NetworkManager.instance.GetBook(), DataManager.instance.photon_debater_room_no);
        //getTopic = StartCoroutine(Coroutine_GetTopic());
        panel_SceneLoad.SetActive(true);
    }

    public void StopGetTopic()
    {
        if(getTopic != null)
        {
            StopCoroutine(getTopic);
            getTopic = null;
        }

        panel_SceneLoad.SetActive(false);
    }

    private IEnumerator Coroutine_GetTopic()
    {
        //while(DataManager.instance.topic.topic == null || DataManager.instance.topicClip == null)
        //{
        //    yield return new WaitForSeconds(0.5f);
        //}

        while (DataManager.instance.topic.topic == null)
        {
            yield return new WaitForSeconds(0.5f);
        }

        NetworkManager.instance.CreateRoom();
        getTopic = null;
    }

    #region 판넬 엑티브
    public void ShowLogInPanel()
    {
        panel_login.SetActive(true);
        panel_makeRoom.SetActive(false);
        panel_choiceRoom.SetActive(false);
        panel_signUp.SetActive(false);
        panel_SelectChannel.SetActive(false);
        panel_PopUp.SetActive(false);
        panel_HttpLoad.SetActive(false);
        panel_SceneLoad.SetActive(false);
}

    public void ShowSelectChannelPanel()
    {
        panel_login.SetActive(true);
        panel_makeRoom.SetActive(false);
        panel_choiceRoom.SetActive(false);
        panel_signUp.SetActive(false);
        panel_SelectChannel.SetActive(true);
        panel_PopUp.SetActive(false);
        panel_HttpLoad.SetActive(false);
        panel_SceneLoad.SetActive(false);
    }

    public void ShowMakeRoomPanel()
    {
        panel_login.SetActive(false);
        panel_makeRoom.SetActive(true);
        panel_choiceRoom.SetActive(false);
        panel_signUp.SetActive(false);
        panel_SelectChannel.SetActive(false);
        panel_PopUp.SetActive(false);
        panel_HttpLoad.SetActive(false);
        panel_SceneLoad.SetActive(false);
    }

    public void ShowChoiceRoomPanel()
    {
        panel_login.SetActive(false);
        panel_makeRoom.SetActive(false);
        panel_choiceRoom.SetActive(true);
        panel_signUp.SetActive(false);
        panel_SelectChannel.SetActive(false);
        panel_PopUp.SetActive(false);
        panel_HttpLoad.SetActive(false);
        panel_SceneLoad.SetActive(false);
    }

    public void ShowSignUpPanel()
    {
        panel_login.SetActive(false);
        panel_makeRoom.SetActive(false);
        panel_choiceRoom.SetActive(false);
        panel_signUp.SetActive(true);
        panel_SelectChannel.SetActive(false);
        panel_PopUp.SetActive(false);
        panel_HttpLoad.SetActive(false);
        panel_SceneLoad.SetActive(false);
    }

    public void ShowLoading()
    {
        panel_login.SetActive(false);
        panel_makeRoom.SetActive(false);
        panel_choiceRoom.SetActive(false);
        panel_signUp.SetActive(false);
        panel_SelectChannel.SetActive(false);
        panel_PopUp.SetActive(false);
        panel_HttpLoad.SetActive(false);
        panel_SceneLoad.SetActive(true);
    }

    public void ShowPopUp(string error)
    {
        text_Error.text = error;
        panel_PopUp.SetActive(true);
    }

    public void ShowHttpLoadingImage()
    {
        if (roateLoading != null)
            return;

        image_HttpLoading.rotation = Quaternion.identity;
        panel_HttpLoad.SetActive(true);
        roateLoading = StartCoroutine(RoateLoading());
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
