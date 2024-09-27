using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BookHistory : MonoBehaviour
{
    public Button button;

    public Image image_Book;
    public string bookInfo;

    public TMP_Text text_BookName;
    public TMP_Text text_BookAuthor;
    public TMP_Text text_BookTopic;
    public TMP_Text text_BookData;

    public History history;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => { LobbyUIManager.instance.ShowMyBook(bookInfo, history.bookAuthor, history.category, history.topic, history.summary, $"{history.createdAtDate} - {history.createdAtTime}"); });
    }

    public void SetHistory(History history)
    {
        this.history = history;
        foreach (var item in ImageChoiceManager.instance.BookData)
        {
            if(history.bookName == item.name)
            {
                image_Book.sprite = item.image;
                bookInfo = item.info;
            }
        }
        text_BookName.text = history.bookName;
        text_BookAuthor.text = history.bookAuthor;
        text_BookTopic.text = history.topic;
        text_BookData.text = $"{history.createdAtDate} - {history.createdAtTime}";
    }
}
