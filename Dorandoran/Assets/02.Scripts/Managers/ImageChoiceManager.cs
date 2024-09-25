using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageChoiceManager : MonoBehaviour
{
    public static ImageChoiceManager instance;

    [Serializable]
    public struct ImageData
    {
        public string name;
        public Texture2D image;
    }

    [Serializable]
    public struct Map
    {
        public string name;
        public Texture2D image;
    }

    #region �̹��� ����
    [Header("�̹��� ����")]
    //[SerializeField]
    public ImageData[] BookData;
    public ImageData[] MapData;
    #endregion


    #region ���� â
    [Header("���� â")]
    public Dropdown bookChoice;
    public Dropdown mapChoice;
    #endregion

    #region �̹��� ����
    [Header("�̹��� ���� â")]
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public Texture2D GetbookChoiceImage()
    {
        return BookData[bookChoice.value].image;
    }

    public Texture2D GetmapChoiceImage()
    {
        return MapData[mapChoice.value].image;
    }

    public void SetbookChoiceImage()
    {
        bookImage.texture = GetbookChoiceImage();
    }

    public void SetmapChoiceImage()
    {
        mapImage.texture = GetmapChoiceImage();
    }



}
