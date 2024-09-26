using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ImageChoiceManager;

public class ImageChoiceManager : MonoBehaviour
{
    public static ImageChoiceManager instance;

    [Serializable]
    public struct ImageData
    {
        public string name;
        public Sprite image;
    }

    #region 이미지 정보
    [Header("이미지 정보")]
    public List<ImageData> BookData;
    public List<ImageData> MapData;
    #endregion


    #region 선택 창
    [Header("선택 창")]
    public Dropdown bookChoice;
    public Dropdown mapChoice;
    #endregion

    #region 이미지 렌더
    [Header("이미지 렌더 창")]
    public RawImage bookImage;
    public RawImage mapImage;
    #endregion

    void Awake()
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

    // Start is called before the first frame update
    void Start()
    {
        bookChoice.onValueChanged.AddListener(delegate { SetbookChoiceImage(); });
        mapChoice.onValueChanged.AddListener(delegate { SetmapChoiceImage(); });

        SetbookChoiceImage();
        SetmapChoiceImage();
    }

    public void CheckBookName()
    {

        
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void SettingImageData()
    {
        SetbookChoiceImage();
        SetmapChoiceImage();
    }

    public Sprite GetbookChoiceImage()
    {
        foreach (ImageData imagedata in BookData)
        {
            //// 선택한 이름과 책과 같으면
            if (bookChoice.options[bookChoice.value].text == imagedata.name)
            {
                return imagedata.image;
            }
        }

        return null;
    }

    public Sprite GetmapChoiceImage()
    {
        return MapData[mapChoice.value].image;
    }

    public void SetbookChoiceImage()
    {
        bookImage.texture = GetbookChoiceImage().texture;
    }

    public void SetmapChoiceImage()
    {
        mapImage.texture = GetmapChoiceImage().texture;
    }



}
