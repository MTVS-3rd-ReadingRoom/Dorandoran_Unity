
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;

// using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public string microphoneName;
    public Dropdown dropdown_MicrophoneName;
    public Button button_Signup;
    public Button button_Login;
    public Button button_Record;
    public Button button_GetTopic;
    public Button button_DedateRoom;
    public Button button_DedateRoom_user;
    public Button button_Summary;
    public Button button_BookList;
    public Text theme_UI;
    public TMP_Text[] text_Topics;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            InitUI();
        }
        else
        {
            Destroy(instance);
        }

    }

    private void Start()
    {
        ActiveSelectMicrophoneUI();
    }

    private void InitUI()
    {
        dropdown_MicrophoneName.onValueChanged.AddListener(delegate { SelectMicrophone(); });
        button_Record.onClick.AddListener(delegate { DataManager.instance.RecordMicrophone(); });
        button_Signup.onClick.AddListener(delegate { HttpManager.instance.PostSignUp_FormData("test", "test", "test", "test"); });
        button_Login.onClick.AddListener(delegate { HttpManager.instance.PostLogIn_FormData("1", "1"); });
        button_DedateRoom.onClick.AddListener(delegate { HttpManager.instance.PostDedateRoom("1", "test_room"); });
        button_DedateRoom_user.onClick.AddListener(delegate { HttpManager.instance.PostDedateRoom_User(1); });
        button_GetTopic.onClick.AddListener(delegate { GetTopic(); });
        button_Summary.onClick.AddListener(delegate { HttpManager.instance.PostSummary("test_room"); });
        button_BookList.onClick.AddListener(delegate { HttpManager.instance.PostBookList(); });
    }

    private void ActiveSelectMicrophoneUI()
    {
        microphoneName = Microphone.devices[DataManager.instance.microphoneIndex];
        int index = Microphone.devices.Length;
        dropdown_MicrophoneName.ClearOptions();
        dropdown_MicrophoneName.AddOptions(Microphone.devices.ToList());
        dropdown_MicrophoneName.value = DataManager.instance.microphoneIndex;
    }

    public void SelectMicrophone()
    {
        DataManager.instance.microphoneIndex = dropdown_MicrophoneName.value;
    }

    public void SetTheme(string theme)
    {
        theme_UI.text = theme;
    }

    public void GetTopic()
    {
        HttpManager.instance.PostTopic_Voice("test_room"); 
        HttpManager.instance.PostTopic_Text("test_room");
    }

    public void SetTopic(Topic topic)
    {
        text_Topics[0].text = topic.topic;
        text_Topics[1].text = topic.content;
    }
}
