using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
using System.Reflection;
using Newtonsoft.Json;
using JetBrains.Annotations;

[System.Serializable]
public struct Error
{
    public string error;

    public Error(string error)
    {
        this.error = error;
    }
}

[System.Serializable]
public struct RoomNum
{
    public int roomId;

    public RoomNum(int id)
    {
        this.roomId = id;
    }
}

[System.Serializable]
public struct Topic
{
    public string topic;
    public string content;

    public Topic(string topic, string content)
    {
        this.topic = topic;
        this.content = content;
    }
}

[System.Serializable]
public struct BookUI
{
    public int no;
    public string isbn;
    public string name;
    public string author;
    public string category;
}

[System.Serializable]
public struct BookList
{
    public List<BookUI> books; 
}


[System.Serializable]
public struct History
{
    public int debateRoomNo;
    public string bookName;
    public string bookAuthor;
    public string category;
    public string topic;
    public string summary;
    public string createdAtDate;
    public string createdAtTime;
}

[System.Serializable]
public struct HistoryList
{
    public List<History> histories;
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
    public const string key = "Authorization";
    public string value;

    public const string url = "http://www.dorandoran.life:11225";

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PostBookList();
    }

    public void PostSignUp_FormData(string name, string userId, string password, string email)
    {
        print($"Start : {MethodInfo.GetCurrentMethod()}");
        HttpInfo info = new HttpInfo();
        // 서버 URL 설정
        info.url = url + "/signup";
        info.onComplete = (UnityWebRequest webRequest) =>
        {
            print($"Success : {MethodInfo.GetCurrentMethod()}");
            print(webRequest.downloadHandler.text);
            LobbyUIManager.instance.ShowLogInPanel();
        };

        // data 를 MultipartForm 으로 셋팅
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("name", name));
        formData.Add(new MultipartFormDataSection("userId", userId));
        formData.Add(new MultipartFormDataSection("password", password));
        formData.Add(new MultipartFormDataSection("email", email));

        StartCoroutine(UploadFileByFormData(info, formData, MethodInfo.GetCurrentMethod().Name));
    }


    public void PostLogIn_FormData(string userId, string password)
    {
        print($"Start : {MethodInfo.GetCurrentMethod()}");
        HttpInfo info = new HttpInfo();
        // 서버 URL 설정
        info.url = url + "/login";
        info.onComplete = (UnityWebRequest webRequest) =>
        {
            print($"Success : {MethodInfo.GetCurrentMethod()}");
            string v = webRequest.GetResponseHeaders()[key].Split(" ")[1];
            print(v);
            value = v;
            DataManager.instance.nickName = userId;
            NetworkManager.instance.StartLogin();
            if(LobbyUIManager.instance != null)
            {
                LobbyUIManager.instance.ShowSelectChannelPanel();
            }
        };

        // data 를 MultipartForm 으로 셋팅
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("userId", userId));
        formData.Add(new MultipartFormDataSection("password", password));

        StartCoroutine(UploadFileByFormData(info, formData, MethodInfo.GetCurrentMethod().Name));
    }


    // 1. 토론방을 생성하면 host는 백엔드로 토론방 생성 요청(POST /api/debate-room)을 보낸다. 백엔드는 호스트에게 토론방 식별값(number)을 반환하고 host는 토론방 식별값을 저장한다
    public void PostDedateRoom(string book_name, string photon_debater_room_no)
    {
        print($"Start : {MethodInfo.GetCurrentMethod()}");
        HttpInfo info = new HttpInfo();
        // 서버 URL 설정
        info.url = url + "/api/debate-room";
        info.onComplete = (UnityWebRequest webRequest) =>
        {
            RoomNum roomNum = JsonUtility.FromJson<RoomNum>(webRequest.downloadHandler.text);
            DataManager.instance.serial_Room = roomNum.roomId;

            // 시연 용
            Topic temp = new Topic();
            temp.topic = "백설공주는 예쁘면 허락 없이 다른 사람의 물건을 사용하는 것이 괜찮다는 메시지를 전달하는가?";
            temp.content = "찬성 측에서는 백설공주가 숲 속에서 길을 잃고 생존을 위해 음식과 잠자리를 찾은 것은 불가피한 선택이라고 주장할 수 있습니다. ||| 반대 측에서는 다른 사람의 집에 무단으로 들어가고 그들의 물건을 사용한 백설공주의 행동이 잘못되었다고 주장할 수 있습니다.";
            DataManager.instance.SetTopic_Text(temp);
            // 시연 용

            //PostTopic_Text(roomNum.roomId.ToString());
            print($"Success : {MethodInfo.GetCurrentMethod()} - {webRequest.downloadHandler.text}");
        };

        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("bookName", book_name));
        formData.Add(new MultipartFormDataSection("photonDebateRoomNo", photon_debater_room_no));

        StartCoroutine(UploadFileByFormData(info, formData, MethodInfo.GetCurrentMethod().Name));
    }

    // 2. (프론트)토론이 시작되면 host는 모든 사용자에게 백엔드에서 받은 토론방 식별값을 보낸다
    // 3. 모든 사용자는 자신의 토큰과 토론방 식별값을 백엔드에 참석 요청(POST /api/debate-room-user)을 보낸다
    public void PostDedateRoom_User(int debateroom_no) // --------------------------- Swagger 토큰 번호가 없음 ------------------------------------
    {
        print($"Start : {MethodInfo.GetCurrentMethod()}");
        HttpInfo info = new HttpInfo();
        // 서버 URL 설정
        info.url = url + "/api/debate-room-user";
        info.onComplete = (UnityWebRequest webRequest) =>
        {
            print($"Success : {MethodInfo.GetCurrentMethod()}");
        };


        print($"{key} : {value}");

        WWWForm wwwForm = new WWWForm();
        wwwForm.AddField("debateRoomNo", debateroom_no);
        //List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        //formData.Add(new MultipartFormDataSection("debateroom_no", debateroom_no));

        //List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        //formData.Add(new MultipartFormDataSection("debateroom_no", debateroom_no));
        StartCoroutine(UploadFileByWWWFormData(info, wwwForm, MethodInfo.GetCurrentMethod().Name));
    }


    public void PostTopic_Voice(string text)
    {
        print($"Start : {MethodInfo.GetCurrentMethod()}");
        HttpInfo info = new HttpInfo();
        // 서버 URL 설정
        info.url = url + "/api/tts/basic";

        //UnityWebRequest webRequest = new UnityWebRequest();
        //webRequest.downloadHandler = new DownloadHandlerAudioClip(info.url, AudioType.WAV);

        info.onComplete = (webRequest) =>
        {
            // save
            DownloadHandlerBuffer buffer = (DownloadHandlerBuffer)(webRequest.downloadHandler);
            byte[] bins = buffer.data;

            AudioClip a = WavUtility.ToAudioClip(bins);
            DataManager.instance.SetTopic_Voice(a);

        };

        // data 를 MultipartForm 으로 셋팅
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("text ", text));

        StartCoroutine(UploadFileByFormData(info, formData, MethodInfo.GetCurrentMethod().Name));

    }

    public void PostTopic_Text(string chat_room_id)
    {
        print($"Start : {MethodInfo.GetCurrentMethod()}");
        HttpInfo info = new HttpInfo();
        // 서버 URL 설정
        info.url = url + "/api/topic_suggest-text";

        UnityWebRequest webRequest = new UnityWebRequest();
        webRequest.downloadHandler = new DownloadHandlerAudioClip(info.url, AudioType.WAV);

        info.onComplete = (webRequest) =>
        {
            string topic_text = webRequest.downloadHandler.text;
            print(topic_text);
            Topic topic = JsonUtility.FromJson<Topic>(topic_text);
            DataManager.instance.SetTopic_Text(topic);
            print($"Success : {MethodInfo.GetCurrentMethod()}");
        };

        // data 를 MultipartForm 으로 셋팅
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("chat_room_id ", chat_room_id));

        StartCoroutine(UploadFileByFormData(info, formData, MethodInfo.GetCurrentMethod().Name));

    }

    public void PostSummary(string chat_room_id)
    {
        print($"Start : {MethodInfo.GetCurrentMethod()}");
        HttpInfo info = new HttpInfo();
        // 서버 URL 설정
        info.url = url + "/api/summary";
        info.onComplete = (UnityWebRequest webRequest) =>
        {
            print($"Success : {MethodInfo.GetCurrentMethod()}");
            print(webRequest.downloadHandler.text);
        };
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("chat_room_id ", chat_room_id));

        StartCoroutine(UploadFileByFormData(info, formData, MethodInfo.GetCurrentMethod().Name));
    }
   


    public void PostVoiceClip_FormData(string user_id, string chat_room_id,  byte[] file)
    {
        HttpInfo info = new HttpInfo();
        // 서버 URL 설정
        info.url = url + "/api/voice";
        info.onComplete = (UnityWebRequest webRequest) =>
        {
            print($"Success : {MethodInfo.GetCurrentMethod()}");
            print(webRequest.downloadHandler.text);
        };

        // data 를 MultipartForm 으로 셋팅
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("user_id", user_id));
        formData.Add(new MultipartFormDataSection("chat_room_id", chat_room_id));
        formData.Add(new MultipartFormFileSection("file", file, "voice"+user_id+".wav", "audio/wav"));

        StartCoroutine(UploadFileByFormData(info, formData, MethodInfo.GetCurrentMethod().Name));
    }
    public void PostBookList()
    {
        HttpInfo info = new HttpInfo();
        // 서버 URL 설정
        info.url = url + "/api/book";
        info.onComplete = (UnityWebRequest webRequest) =>
        {
            print($"Success : {MethodInfo.GetCurrentMethod()}");
            DataManager.instance.SetBookList(JsonConvert.DeserializeObject<List<BookUI>>(webRequest.downloadHandler.text));
        };

        StartCoroutine(Get(info, MethodInfo.GetCurrentMethod().Name));
    }

    public void GetHistory()
    {
        HttpInfo info = new HttpInfo();
        info.url = url + "/api/summary";
        info.onComplete = (UnityWebRequest webRequest) =>
        {
            print($"Success : {MethodInfo.GetCurrentMethod()}");
            List<History> histories = JsonConvert.DeserializeObject<List<History>>(webRequest.downloadHandler.text);
            DataManager.instance.SetHistoryList(histories);
            if(LobbyUIManager.instance != null)
            {
                LobbyUIManager.instance.ResetHistoryUI();
                for (int i = 0; i < histories.Count; i++)
                {
                    LobbyUIManager.instance.AddHistoryUI(histories[i]);
                }
            }
        };

        StartCoroutine(Get_Authorization(info, MethodInfo.GetCurrentMethod().Name));
    }

    //public UnityWebRequest test()
    //{
    //    UnityWebRequest webREquest = new UnityWebRequest;
    //    webREquest.SetRequestHeader(key, value);
    //    return webREquest;
    //}


    public IEnumerator Post(HttpInfo info, string method)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Post(info.url, info.body, info.contentType))
        {
            if (LobbyUIManager.instance != null)
            {
                LobbyUIManager.instance.ShowHttpLoadingImage();
            }
            // 서버에 요청 보내기
            yield return webRequest.SendWebRequest();

            // 서버에게 응답이 왔다.
            DoneRequest(webRequest, info, method);
        }
    }

    public IEnumerator Get(HttpInfo info, string method)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(info.url))
        {
            if (LobbyUIManager.instance != null)
            {
                LobbyUIManager.instance.ShowHttpLoadingImage();
            }
            // 서버에 요청 보내기
            yield return webRequest.SendWebRequest();

            // 서버에게 응답이 왔다.
            DoneRequest(webRequest, info, method);
        }
    }


    public IEnumerator Get_Authorization(HttpInfo info, string method)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(info.url))
        {
            if (LobbyUIManager.instance != null)
            {
                LobbyUIManager.instance.ShowHttpLoadingImage();
            }
            webRequest.SetRequestHeader(key, value);
            // 서버에 요청 보내기
            yield return webRequest.SendWebRequest();

            // 서버에게 응답이 왔다.
            DoneRequest(webRequest, info, method);
        }
    }


    public IEnumerator UploadFileByFormData(HttpInfo info, List<IMultipartFormSection> formData, string method)
    {

        using (UnityWebRequest webRequest = UnityWebRequest.Post(info.url, formData))
        {
            if (LobbyUIManager.instance != null)
            {
                LobbyUIManager.instance.ShowHttpLoadingImage();
            }
            // 서버에 요청 보내기
            yield return webRequest.SendWebRequest();

            // 서버에게 응답이 왔다.
            DoneRequest(webRequest, info, method);
        }
    }

    public IEnumerator UploadFileByFormData_AddHeaderKey(HttpInfo info, List<IMultipartFormSection> formData, string method)
    {

        using (UnityWebRequest webRequest = UnityWebRequest.Post(info.url, formData))
        {
            if (LobbyUIManager.instance != null)
            {
                LobbyUIManager.instance.ShowHttpLoadingImage();
            }
            webRequest.SetRequestHeader(key, value);
            // 서버에 요청 보내기
            yield return webRequest.SendWebRequest();

            // 서버에게 응답이 왔다.
            DoneRequest(webRequest, info, method);
        }
    }

    public IEnumerator UploadFileByWWWFormData( HttpInfo info, WWWForm wwwForm, string method)
    {

        using (UnityWebRequest webRequest = UnityWebRequest.Post(info.url, wwwForm))
        {
            webRequest.SetRequestHeader(key, value);

            if (LobbyUIManager.instance != null)
            {
                LobbyUIManager.instance.ShowHttpLoadingImage();
            }
            // 서버에 요청 보내기
            yield return webRequest.SendWebRequest();

            // 서버에게 응답이 왔다.
            DoneRequest(webRequest, info, method);
        }
    }


    void DoneRequest(UnityWebRequest webRequest, HttpInfo info, string method)
    {
        // 만약에 결과가 정상이라면
        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            if (LobbyUIManager.instance != null)
            {
                LobbyUIManager.instance.StopRoateLoading();
            }
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
            Debug.LogError($"Net Error : {webRequest.error} ({method})");

            if(LobbyUIManager.instance != null)
            {
                LobbyUIManager.instance.StopRoateLoading();
                LobbyUIManager.instance.ShowPopUp(JsonUtility.FromJson<Error>(webRequest.downloadHandler.text).error);

                LobbyUIManager.instance.StopGetTopic();
            }
        }
    }

    #region 안씀
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

    //IEnumerator GetAudioFile(string path, string fileName)
    //{
    //    //yield return new WaitForSeconds(10);
    //    string fullPath = Path.Combine(path, fileName);
    //    UnityWebRequest req = UnityWebRequestMultimedia.GetAudioClip(fullPath, AudioType.WAV);

    //    yield return req.SendWebRequest();

    //    if(req.result == UnityWebRequest.Result.Success)
    //    {
    //        AudioClip myClip = DownloadHandlerAudioClip.GetContent(req);
    //        DataManager.instance.PlayAudio(myClip);
    //    }
    //}


    //IEnumerator PostThemeCoroutine(string url, List<IMultipartFormSection> data)
    //{
    //    UnityWebRequest req = UnityWebRequest.Post(url, data);

    //    //string sendString = "TestString";
    //    //byte[] sendBins = Encoding.UTF8.GetBytes(sendString);

    //    //req.uploadHandler = new UploadHandlerRaw(sendBins);
    //    req.downloadHandler = new DownloadHandlerAudioClip(url, AudioType.WAV);
    //    yield return req.SendWebRequest();

    //    if (req.result == UnityWebRequest.Result.Success)
    //    {
    //        DownloadHandlerAudioClip downHandler = (DownloadHandlerAudioClip)(req.downloadHandler);
    //        AudioClip clip = downHandler.audioClip;
    //    }
    //    else
    //    {
    //        Debug.LogError($"{req.responseCode} - {req.error}");
    //    }
    //}

    //public void PostVoiceClip_Octet(byte[] bins)
    //{
    //    // 서버 URL 설정
    //    string url = "http://192.168.0.58:8080/api/send-data";

    //    // UnityWebRequest 생성
    //    UnityWebRequest request = new UnityWebRequest(url, "POST");

    //    // 업로드할 데이터 설정
    //    UploadHandlerRaw uploadHandler = new UploadHandlerRaw(bins);
    //    request.uploadHandler = uploadHandler;

    //    // 헤더 설정 (Content-Type: application/octet-stream)
    //    request.SetRequestHeader("Content-Type", "application/octet-stream");

    //    // 요청 보내기
    //    UnityWebRequestAsyncOperation asyncOp = request.SendWebRequest();

    //}
    #endregion
}
