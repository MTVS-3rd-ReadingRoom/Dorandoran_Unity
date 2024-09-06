using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
// using UnityEngine.UIElements;

public class LoginUIController : MonoBehaviour
{
    public GameObject panel_login;
    public GameObject panel_makeRoom;
    public GameObject panel_choiceRoom;
    public GameObject panel_signUp;

    public Button btn_login;
    public TMP_InputField input_nickName;
    public static LoginUIController LoginUI;
    public TMP_Text text_logText;

    string log;


    private void Awake()
    {
        if (LoginUI == null)
        {
            LoginUI = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowMakeRoomPanel()
    {
        btn_login.interactable = false;
        panel_login.SetActive(false);
        panel_makeRoom.SetActive(true);
        panel_choiceRoom.SetActive(false);
        panel_signUp.SetActive(false);
    }

    public void ShowchoiceRoomPanel()
    {
        btn_login.interactable = false;
        panel_login.SetActive(false);
        panel_makeRoom.SetActive(false);
        panel_choiceRoom.SetActive(true);
        panel_signUp.SetActive(false);
    }

    public void ShowsignUpPanel()
    {
        btn_login.interactable = false;
        panel_login.SetActive(false);
        panel_makeRoom.SetActive(false);
        panel_choiceRoom.SetActive(false);
        panel_signUp.SetActive(true);
    }

    
    public void PrintLog(string message)
    {
        log += message + '\n';
        text_logText.text = log;
    }
}
