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
    // Body 데이터
    public string body = "";

    // contentType
    public string contentType = "";

    // 통신 성공 후 호출되는 함수 담을 변수
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
            // 다운로드 된 데이터를 Texture2D 로 변환.
            UserInfo result = JsonUtility.FromJson<UserInfo>(downloadHandler.text);
        };

        // 디바이스 확인용 코드
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
            // 다운로드 된 데이터를 Texture2D 로 변환.
            UserInfo result = JsonUtility.FromJson<UserInfo>(downloadHandler.text);
        };

        // 디바이스 확인용 코드
        foreach (var device in Microphone.devices)
        {
            Debug.Log("Name: " + device);
        }

    }

    public IEnumerator Post(HttpInfo info)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Post(url, info.body, info.contentType))
        {
            // 서버에 요청 보내기
            yield return webRequest.SendWebRequest();

            // 서버에게 응답이 왔다.
            DoneRequest(webRequest, info);
        }
    }

    void DoneRequest(UnityWebRequest webRequest, HttpInfo info)
    {
        // 만약에 결과가 정상이라면
        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            // 응답 온 데이터를 요청한 클래스로 보내자.
            if (info.onComplete != null)
            {
                info.onComplete(webRequest.downloadHandler);
            }
        }
        // 그렇지 않다면 (Error 라면)
        else
        {
            // Error 의 이유를 출력
            Debug.LogError("Net Error : " + webRequest.error);
        }
    }
}
