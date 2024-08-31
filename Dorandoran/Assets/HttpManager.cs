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
    private string url;

    public void GetLogin(string id, string pw)
    {
        HttpInfo httpInfo = new HttpInfo();
        
        Login_Json login_Json = new Login_Json(id, pw);
        httpInfo.body = JsonUtility.ToJson(login_Json);
        httpInfo.contentType = "application/json";
        httpInfo.onComplete = (DownloadHandler downloadHandler) =>
        {
            // �ٿ�ε� �� �����͸� Texture2D �� ��ȯ.
            UserInfo result = JsonUtility.FromJson<UserInfo>(downloadHandler.text);
        };

        // ����̽� Ȯ�ο� �ڵ�
        foreach (var device in Microphone.devices)
        {
            Debug.Log("Name: " + device);
        }

    }

    public void Get(string id, string pw)
    {
        HttpInfo httpInfo = new HttpInfo();

        Login_Json login_Json = new Login_Json(id, pw);
        httpInfo.body = JsonUtility.ToJson(login_Json);
        httpInfo.contentType = "application/json";
        httpInfo.onComplete = (DownloadHandler downloadHandler) =>
        {
            // �ٿ�ε� �� �����͸� Texture2D �� ��ȯ.
            UserInfo result = JsonUtility.FromJson<UserInfo>(downloadHandler.text);
        };

        // ����̽� Ȯ�ο� �ڵ�
        foreach (var device in Microphone.devices)
        {
            Debug.Log("Name: " + device);
        }

    }

    public IEnumerator Post(HttpInfo info)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Post(url, info.body, info.contentType))
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
