using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUIManager : MonoBehaviour
{
    public static LobbyUIManager instance;

    #region Panel
    [Header("페이지")]
    public GameObject panel_login;
    public GameObject panel_signUp;
    public GameObject panel_SelectChannel;
    public GameObject panel_makeRoom;
    public GameObject panel_choiceRoom;
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


    #region 회원가입
    [Header("채널 선택")]
    public Button[] buttons_MakeRoomChannel;
    public Button[] buttons_ChoiceRoomChannel;
    #endregion

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
        for (int i = 0; i < buttons_MakeRoomChannel.Length; i++)
        {
            buttons_MakeRoomChannel[i].onClick.AddListener(() => { ShowMakeRoomPanel(); });
        }
        for (int i = 0; i < buttons_ChoiceRoomChannel.Length; i++)
        {
            buttons_ChoiceRoomChannel[i].onClick.AddListener(() => { ShowChoiceRoomPanel(); });
        }
    }
    public bool CheckLogIn()
    {
        for (int i = 0; i < inputFields_LogIn.Length; i++)
        {
            if (string.IsNullOrEmpty(inputFields_LogIn[i].text))
            {
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
                return false;
            }
        }

        for (int i = 0; i < inputFields_SignUp.Length; i++)
        {
            if (inputFields_SignUp[i].text.Length < 2)
            {
                return false;
            }
        }

        return true;
    }
    public void ShowLogInPanel()
    {
        panel_login.SetActive(true);
        panel_makeRoom.SetActive(false);
        panel_choiceRoom.SetActive(false);
        panel_signUp.SetActive(false);
        panel_SelectChannel.SetActive(false);
    }

    public void ShowSelectChannelPanel()
    {
        print(2);
        panel_login.SetActive(true);
        panel_makeRoom.SetActive(false);
        panel_choiceRoom.SetActive(false);
        panel_signUp.SetActive(false);
        panel_SelectChannel.SetActive(true);
    }

    public void ShowMakeRoomPanel()
    {
        print(1);
        panel_login.SetActive(false);
        panel_makeRoom.SetActive(true);
        panel_choiceRoom.SetActive(false);
        panel_signUp.SetActive(false);
        panel_SelectChannel.SetActive(false);
    }

    public void ShowChoiceRoomPanel()
    {
        panel_login.SetActive(false);
        panel_makeRoom.SetActive(false);
        panel_choiceRoom.SetActive(true);
        panel_signUp.SetActive(false);
        panel_SelectChannel.SetActive(false);
    }

    public void ShowSignUpPanel()
    {
        panel_login.SetActive(false);
        panel_makeRoom.SetActive(false);
        panel_choiceRoom.SetActive(false);
        panel_signUp.SetActive(true);
        panel_SelectChannel.SetActive(false);
    }
}
