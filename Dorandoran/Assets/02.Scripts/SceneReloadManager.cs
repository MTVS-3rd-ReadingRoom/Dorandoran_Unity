using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneReloadManager : MonoBehaviour
{
    public static SceneReloadManager instance;

    bool Reload;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        Reload = false;
    }

    // Update is called once per frame
    void Update()
    {
        CheckReload();
    }

    public void NextScene()
    {
        StartCoroutine(CheckReloadWait());
    }

    public void CheckReload()
    {
        if(Reload)
        {
            LobbyUIManager.instance.ShowSelectChannelPanel();
            DataManager.instance.SetBookList(DataManager.instance.bookList);
            
            Reload = false;
        }
    }

    IEnumerator CheckReloadWait()
    {
        yield return new WaitForSeconds(0.01f);

        Reload = true;

    }
}
