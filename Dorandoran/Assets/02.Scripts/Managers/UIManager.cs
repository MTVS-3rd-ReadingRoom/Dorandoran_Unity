
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public string microphoneName;
    public Dropdown dropdown_MicrophoneName;
    public Button button;
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
        button.onClick.AddListener(delegate { DataManager.instance.RecordMicrophone(); });
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
