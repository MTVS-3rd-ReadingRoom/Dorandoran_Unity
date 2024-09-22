using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
// using static UnityEngine.Rendering.DebugUI;

public class NoteWrite : MonoBehaviour
{
    public TMP_InputField inputData;

    public Button button_Note;
    public GameObject Note_Panel;

    bool NoteEnable = false;
    // Start is called before the first frame update
    void Start()
    {
        Note_Panel.SetActive(NoteEnable);
        button_Note.onClick.AddListener(NotePopup);
        inputData.lineType = TMP_InputField.LineType.MultiLineNewline;
        inputData.caretPosition = inputData.text.Length;
        inputData.ActivateInputField();
    }



    void NotePopup()
    {
        NoteEnable = !NoteEnable;
        Note_Panel.SetActive(NoteEnable);
    }

    private void OnEnable()
    {
        inputData.caretPosition = inputData.text.Length;
        inputData.ActivateInputField();
    }

    // Update is called once per frame
    void Update()
    {
        EnterNote();
    }

    public void EnterNote()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            inputData.text += "\n";
            inputData.caretPosition = inputData.text.Length;
            inputData.ActivateInputField();
        }
    }
}
