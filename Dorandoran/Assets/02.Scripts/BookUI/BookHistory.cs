using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BookHistory : MonoBehaviour
{
    public Button button;

    public Sprite image_Book;
    public TMP_Text text_BookName;
    public TMP_Text text_BookAuthor;
    public TMP_Text text_BookTopic;
    public TMP_Text text_BookData;

    public History history;

    private void Start()
    {
        button = GetComponent<Button>();
    }

    public void SetHistory(History history)
    {
        this.history = history;
        foreach (var item in ImageChoiceManager.instance.BookData)
        {
            if(history.bookName == item.name)
            {
                image_Book = item.image;
            }
        }
        text_BookName.text = history.bookName;
        text_BookAuthor.text = history.bookAuthor;
        text_BookTopic.text = history.topic;
        text_BookData.text = $"{history.createdAtDate} - {history.createdAtTime}";
    }
}
