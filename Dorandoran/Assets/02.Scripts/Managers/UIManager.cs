
using System.Collections;
using System.Collections.Generic;
using System.Linq;
// using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public string microphoneName;
    public Dropdown dropdown_MicrophoneName;
    public Button button_Signup;
    public Button button_Login;
    public Button button_Record;
    public Button button_GetTopic;
    public Text theme_UI;

    private void Awake()
    {
        InitUI();
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
        button_Login.onClick.AddListener(delegate { HttpManager.instance.PostLogIn_FormData("test", "test"); });
        button_GetTopic.onClick.AddListener(delegate { HttpManager.instance.PostTheme("test_room"); });
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
}
