using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Text;
using System;
using UnityEngine.Networking;
using UnityEditor.PackageManager.Requests;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Net;
using UnityEngine.Video;

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

    // Body 데이터
    public string body = "";

    // contentType
    public string contentType = "";

    // 통신 성공 후 호출되는 함수 담을 변수
    public Action<UnityEngine.Networking.UnityWebRequest> onComplete;
}

public class HttpManager : MonoBehaviour
{
    public static HttpManager instance;
    public string key { get; private set; } = "Authorization";
    public string value { get; private set; }


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


    public void PostSignUp_FormData(string name, string userId, string password, string nickName)
    {
        HttpInfo info = new HttpInfo();
        // 서버 URL 설정
        info.url = "http://192.168.0.58:8080/signup";
        info.onComplete = (UnityWebRequest webRequest) =>
        {
            print(webRequest.downloadHandler.text);
        };

        // data 를 MultipartForm 으로 셋팅
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("name", name));
        formData.Add(new MultipartFormDataSection("userId", userId));
        formData.Add(new MultipartFormDataSection("password", password));
        formData.Add(new MultipartFormDataSection("nickName", nickName));

        StartCoroutine(UploadFileByFormData(info, formData));
    }

    public void PostLogIn_FormData(string userId, string password)
    {
        HttpInfo info = new HttpInfo();
        // 서버 URL 설정
        info.url = "http://192.168.0.58:8080/login";
        info.onComplete = (UnityWebRequest webRequest) =>
        {
            value = webRequest.GetResponseHeaders()[key];
        };

        // data 를 MultipartForm 으로 셋팅
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("userId", userId));
        formData.Add(new MultipartFormDataSection("password", password));

        StartCoroutine(UploadFileByFormData(info, formData));
    }


    //public void Get(string id, string pw)
    //{
    //    HttpInfo httpInfo = new HttpInfo();

    //    Login_Json login_Json = new Login_Json(id, pw);
    //    httpInfo.url = "http://192.168.0.6:8080/api/voice/upload";
    //    httpInfo.body = JsonUtility.ToJson(login_Json);
    //    httpInfo.contentType = "application/json";
    //    httpInfo.onComplete = (UnityWebRequest webRequest) =>
    //    {
    //        // 다운로드 된 데이터를 Texture2D 로 변환.
    //        UserInfo result = JsonUtility.FromJson<UserInfo>(webRequest.downloadHandler.text);
    //    };
    //}

    //public void PostVoiceClip(AudioClip_Json audioClip_Json)
    //{
    //    HttpInfo httpInfo = new HttpInfo();
    //    httpInfo.url = "http://192.168.0.6:8080/api/voice/upload";
    //    httpInfo.body = JsonUtility.ToJson(audioClip_Json);
    //    httpInfo.contentType = "multipart/form-data";
    //    httpInfo.onComplete = (DownloadHandler downloadHandler) =>
    //    {
    //        print("test");
    //    };
    //    StartCoroutine(Post(httpInfo));
    //}


    public void PostTheme(string chat_room_id)
    {
        print("StartPostTopic");
        HttpInfo info = new HttpInfo();
        // 서버 URL 설정
        info.url = "http://125.132.216.190:11225/api/topic_suggest";
        info.onComplete = (UnityWebRequest webRequest) =>
        {
            AudioClip audioClip = DownloadHandlerAudioClip.GetContent(webRequest); //test
            DataManager.instance.PlayAudio(audioClip);
        };

        // data 를 MultipartForm 으로 셋팅
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("chat_room_id ", chat_room_id));

        StartCoroutine(UploadFileByFormData(info, formData));

    }

    public void PostVoiceClip_FormData(string user_id, string chat_room_id,  byte[] file)
    {
        HttpInfo info = new HttpInfo();
        // 서버 URL 설정
        info.url = "http://192.168.0.58:8080/api/send-data";
        info.onComplete = (UnityWebRequest webRequest) =>
        {
            print(webRequest.downloadHandler.text);
        };

        // data 를 MultipartForm 으로 셋팅
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("user_id", user_id));
        formData.Add(new MultipartFormDataSection("chat_room_id", chat_room_id));
        formData.Add(new MultipartFormFileSection("file", file, "voice"+user_id+".wav", "audio/wav"));

        StartCoroutine(UploadFileByFormData(info, formData));
    }

    public void PostVoiceClip_Octet(byte[] bins)
    {
        // 서버 URL 설정
        string url = "http://192.168.0.58:8080/api/send-data";

        // UnityWebRequest 생성
        UnityWebRequest request = new UnityWebRequest(url, "POST");

        // 업로드할 데이터 설정
        UploadHandlerRaw uploadHandler = new UploadHandlerRaw(bins);
        request.uploadHandler = uploadHandler;

        // 헤더 설정 (Content-Type: application/octet-stream)
        request.SetRequestHeader("Content-Type", "application/octet-stream");

        // 요청 보내기
        UnityWebRequestAsyncOperation asyncOp = request.SendWebRequest();

    }


    public IEnumerator Post(HttpInfo info)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Post(info.url, info.body, info.contentType))
        {
            // 서버에 요청 보내기
            yield return webRequest.SendWebRequest();

            // 서버에게 응답이 왔다.
            DoneRequest(webRequest, info);
        }
    }

    public IEnumerator UploadFileByFormData(HttpInfo info, List<IMultipartFormSection> formData)
    {

        using (UnityWebRequest webRequest = UnityWebRequest.Post(info.url, formData))
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
                info.onComplete(webRequest);
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
