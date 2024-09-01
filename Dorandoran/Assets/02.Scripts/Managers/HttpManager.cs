using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Text;
using System;
using UnityEngine.Networking;

[System.Serializable]
public struct Login_Json
{
    string id;
    string pw;

    public Login_Json(string id, string pw)
    {
        this.id = id;
        this.pw = pw;
    }
}
public struct UserInfo
{

}

public class HttpInfo
{
    public string url = "";

    // Body ������
    public string body = "";

    // contentType
    public string contentType = "";

    // ��� ���� �� ȣ��Ǵ� �Լ� ���� ����
    public Action<DownloadHandler> onComplete;
}

public class HttpManager : MonoBehaviour
{
    public static HttpManager instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void GetTheme()
    {
        HttpInfo httpInfo = new HttpInfo();

        httpInfo.url = "http://192.168.0.6:8080/api/voice/upload";
        //httpInfo.body = JsonUtility.ToJson(login_Json);
        httpInfo.contentType = "application/json";
        httpInfo.onComplete = (DownloadHandler downloadHandler) =>
        {
            DataManager.instance.theme = downloadHandler.text;
        };

    }

    public void GetLogin(string id, string pw)
    {
        HttpInfo httpInfo = new HttpInfo();
        
        Login_Json login_Json = new Login_Json(id, pw);
        httpInfo.url = "http://192.168.0.6:8080/api/voice/upload";
        httpInfo.body = JsonUtility.ToJson(login_Json);
        httpInfo.contentType = "application/json";
        httpInfo.onComplete = (DownloadHandler downloadHandler) =>
        {
            // �ٿ�ε� �� �����͸� Texture2D �� ��ȯ.
            UserInfo result = JsonUtility.FromJson<UserInfo>(downloadHandler.text);
        };

    }

    public void Get(string id, string pw)
    {
        HttpInfo httpInfo = new HttpInfo();

        Login_Json login_Json = new Login_Json(id, pw);
        httpInfo.url = "http://192.168.0.6:8080/api/voice/upload";
        httpInfo.body = JsonUtility.ToJson(login_Json);
        httpInfo.contentType = "application/json";
        httpInfo.onComplete = (DownloadHandler downloadHandler) =>
        {
            // �ٿ�ε� �� �����͸� Texture2D �� ��ȯ.
            UserInfo result = JsonUtility.FromJson<UserInfo>(downloadHandler.text);
        };
    }

    public void PostVoiceClip(AudioClip_Json audioClip_Json)
    {
        HttpInfo httpInfo = new HttpInfo();
        httpInfo.url = "http://192.168.0.6:8080/api/voice/upload";
        httpInfo.body = JsonUtility.ToJson(audioClip_Json);
        httpInfo.contentType = "multipart/form-data";
        httpInfo.onComplete = (DownloadHandler downloadHandler) =>
        {
            print("test");
        };
        StartCoroutine(Post(httpInfo));
    }


    public IEnumerator Post(HttpInfo info)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Post(info.url, info.body, info.contentType))
        {
            // ������ ��û ������
            yield return webRequest.SendWebRequest();

            // �������� ������ �Դ�.
            DoneRequest(webRequest, info);
        }
    }

    void DoneRequest(UnityWebRequest webRequest, HttpInfo info)
    {
        // ���࿡ ����� �����̶��
        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            // ���� �� �����͸� ��û�� Ŭ������ ������.
            if (info.onComplete != null)
            {
                info.onComplete(webRequest.downloadHandler);
            }
        }
        // �׷��� �ʴٸ� (Error ���)
        else
        {
            // Error �� ������ ���
            Debug.LogError("Net Error : " + webRequest.error);
        }
    }
}
