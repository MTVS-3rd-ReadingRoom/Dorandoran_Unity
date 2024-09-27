using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Realtime;
using static ImageChoiceManager;

public class RoomPanel : MonoBehaviour
{
    public TMP_Text[] roomTexts = new TMP_Text[5];
    public Button btn_join;
    public RawImage bookImage;

    public void SetRoomInfo(RoomInfo room)
    {
        roomTexts[0].text = room.Name;
        roomTexts[1].text = $"({room.PlayerCount}/{room.MaxPlayers})";
        print(room.CustomProperties["MASTER_NAME"] == null);
        string masterName = room.CustomProperties["MASTER_NAME"].ToString();
        roomTexts[2].text = masterName;
        print(room.CustomProperties["BOOK_NAME"] == null);
        string imageData = (string)room.CustomProperties["BOOK_NAME"];
        roomTexts[3].text = imageData;
        SetBookImage(imageData);
        roomTexts[4].text = GetBookAuthor(imageData);

    }

    public void SetBookImage(string imageName)
    {
        foreach (ImageData imagedata in ImageChoiceManager.instance.BookData)
        {
            if (imagedata.name == imageName)
                bookImage.texture = imagedata.image.texture;
        }
    }

    public string GetBookAuthor(string imageName)
    {
        foreach (BookUI bookUI in DataManager.instance.bookList)
        {
            if (bookUI.name == imageName)
                return bookUI.author;
        }
        return "";
    }

}
