using Photon.Pun;
using PhotonVoice = Photon.Voice.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using Photon.Voice.PUN;
using UnityEngine.UI;

public class PlayerMove : PlayerStateBase, IPunObservable
{
    public float trackingSpeed = 3;

    Transform cam;
    CharacterController cc;
    Animator myAnim;
    PhotonView pv;
    Vector3 myPos;
    Quaternion myRot;

    PhotonVoice.Recorder recorder;

    float mx = 0;

    void Start()
    {
        cam = Camera.main.transform;
        cc = GetComponent<CharacterController>();
        myAnim = GetComponentInChildren<Animator>();
        pv = GetComponent<PhotonView>();
        recorder = GetComponentInChildren<PhotonVoice.Recorder>();
        //playerID = pv.ViewID;
    }

    void Update()
    {
        Move();
        Rotate();
    }

    void Move()
    {
        // 만일, 내가 소유권을 가진 캐릭터라면...
        if (pv.IsMine)
        {
            // 현재 카메라가 바라보는 방향으로 이동을 하고 싶다.
            // 이동의 조작 방식은 W,A,S,D 키를 이용한다.
            // 캐릭터 컨트롤러 클래스의 Move 함수를 이용한다.
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            Vector3 dir = new Vector3(h, 0, v);
            dir.Normalize();
            dir = transform.TransformDirection(dir);
            cc.Move(dir * moveSpeed * Time.deltaTime);

            if (myAnim != null)
            {
                myAnim.SetFloat("Horizontal", h);
                myAnim.SetFloat("Vertical", v);
                Debug.Log("Animation: HorizontalValue: " + h + "Vertical: " + v + "\n");
            }
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, myPos, Time.deltaTime * trackingSpeed);
        }
    }

    void Rotate()
    {
        if (pv.IsMine)
        {
            // 사용자의 마우스 좌우 드래그 입력을 받는다.
            mx += Input.GetAxis("Mouse X") * rotSpeed * Time.deltaTime;

            // 입력받은 방향에 따라 플레이어를 좌우로 회전한다.
            transform.eulerAngles = new Vector3(0, mx, 0);
        }
        else
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, myRot, Time.deltaTime * trackingSpeed);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // 만일, 데이터를 서버에 전송(PhotonView.IsMine == true)하는 상태라면...
        if (stream.IsWriting)
        {
            // iterable 데이터를 보낸다.
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        // 그렇지 않고, 만일 데이터를 서버로부터 읽어야하는 상태라면...
        else if (stream.IsReading)
        {
            myPos = (Vector3)stream.ReceiveNext();
            myRot = (Quaternion)stream.ReceiveNext();
        }
    }

    //// RPC 함수
    [PunRPC]
    void UpdateSound(int SoundPlayerid)
    {
        if (SoundPlayerid == pv.ViewID) // 끌 플레이어 id
        {
            recorder.TransmitEnabled = true; // 말한 내용 전달
        }
        else // 킬 플레이어 id
        {
            recorder.TransmitEnabled = false; // 말한 내용 전달x
        }
    }

}

// 