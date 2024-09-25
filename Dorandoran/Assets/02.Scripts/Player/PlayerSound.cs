using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Voice.PUN;
using Photon.Voice;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerSound : MonoBehaviour, IPunObservable
{
    public PhotonView pv;
    public RawImage voiceIcon;
    PhotonVoiceView voiceView;
    bool isTalking = false;

    // Start is called before the first frame update
    void Start()
    {
        print(gameObject.name);
        pv = GetComponent<PhotonView>();
        voiceView = GetComponent<PhotonVoiceView>();
    }

    // Update is called once per frame
    void Update()
    {
        if(pv.IsMine) // 냬꺼만 레코딩 껐다 켰다.
        {
            // 레코딩 중인지 아닌지 체크 후 아이콘 활성화 
            voiceIcon.gameObject.SetActive(voiceView.IsRecording);
        }
        else
        {
            voiceIcon.gameObject.SetActive(isTalking);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //if(stream.IsWriting)
        //{
        //    stream.SendNext(voiceView.IsRecording);
        //}
        //else if(stream.IsReading)
        //{
        //    isTalking = (bool)stream.ReceiveNext();
        //}
    }
}
