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

    CameraFollow cameraFollow;
    Transform cam;
    CharacterController cc;
    Animator myAnim;
    PhotonView pv;
    Vector3 myPos;
    Quaternion myRot;
    PhotonVoice.Recorder recorder;

    float mx = 0;
    int groundLayer;

    bool isReady;
    bool preSitting;
    GameManager gameManager;

    bool isGround;

    Vector3 velocity;
    float jumpHeight;
    Chair chair;
    bool Cu;


    void Start()
    {
        cam = Camera.main.transform;
        cc = GetComponent<CharacterController>();
        myAnim = GetComponentInChildren<Animator>();
        pv = GetComponent<PhotonView>();
        recorder = GetComponentInChildren<PhotonVoice.Recorder>();
        isReady = false;
        preSitting = false;

        jumpHeight = 0.8f;

        gameManager = GameObject.Find("GameManager").GetComponentInChildren<GameManager>();

        groundLayer = LayerMask.NameToLayer("Ground");

        cameraFollow = Camera.main.GetComponentInChildren<CameraFollow>();
        //playerID = pv.ViewID;
    }

    void Update()
    {

        ///// 소유권을 가진 캐릭터가 앉아있다면 회전 막기
        if (pv.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.P)) // 값이 바뀌지 않았을 때
            {
                if (chair != null)
                {
                    if (isReady == false)
                    {
                        if (chair.Sitting(DataManager.instance.nickName))
                        {
                            isReady = true;
                            Sitting();
                            myAnim.SetBool("Sitting", isReady);
                            gameManager.OnStaticCamera();
                            return; // 바로 종료
                        }
                    }
                    else
                    {
                        isReady = false;
                        chair.Eixt();
                        myAnim.SetBool("Sitting", isReady);
                        gameManager.OnPlayerCamera();
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                Cursor.visible = !Cursor.visible;

                if (Cursor.visible)
                {
                    Cursor.lockState = CursorLockMode.None;
                    myAnim.SetFloat("Horizontal", 0);
                    myAnim.SetFloat("Vertical", 0);
                }
                else
                {
                    Cursor.lockState = CursorLockMode.Locked;
                }
            }
        }
        else
        {
            myAnim.SetBool("Sitting", isReady);
            if (isReady) // 앉아있을때
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
                transform.rotation = myRot;
            }
        }

        if (!isReady)
        {
            Move();
            Rotate();
        }
    }

    bool CheckIsGround()
    {
        return Physics.Raycast(transform.position, Vector3.down, 0.3f, groundLayer);
    }
    void Move()
    {
        // 소유권을 가진 캐릭터라면
        if (pv.IsMine)
        {
            // 현재 카메라가 바라보는 방향으로 이동
            // CharacterController Move 함수 사용
            if (!Cursor.visible && !isReady)
            {
                float h = Input.GetAxis("Horizontal");
                float v = Input.GetAxis("Vertical");
                Vector3 dir = new Vector3(h, 0, v);
                dir.y = 0.0f;
                dir = transform.TransformDirection(dir);

                if (dir.magnitude > 0.0f) // 이동하지 않으면
                {
                    isReady = false;
                    myAnim.SetBool("Sitting", isReady);
                }
                else if (dir.magnitude >= 1)
                {
                    dir.Normalize();
                }

                if (isGround && velocity.y < 0.0f)
                    velocity.y = 0.0f;

                Jump();
                velocity.y += -10.0f * Time.deltaTime;

                cc.Move(dir * moveSpeed * Time.deltaTime);
                cc.Move(velocity * Time.deltaTime);
                isGround = cc.isGrounded;
                //transform.forward = dir;

                if (myAnim != null)
                {
                    myAnim.SetFloat("Horizontal", h);
                    myAnim.SetFloat("Vertical", v);
                    //Debug.Log("Animation: HorizontalValue: " + h + "Vertical: " + v + "\n");
                }

            }

        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, myPos, Time.deltaTime * trackingSpeed);
        }
    }

    public void Jump()
    {
        if(Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2.0f * -20.0f);
        }
    }

    void Sitting()
    {
        // p 누르기 + 근처 의자와 충돌 + 앉는 애니메이션 실행
        if (pv.IsMine && chair != null)
        {
            transform.position = chair.transform.position;
            transform.rotation = chair.transform.rotation;
            //for (int i = 0; i < 4; i++)
            //{
            //    if (gameObject.tag == "Chair0" + i)
            //    {
            //        Quaternion rotationQuaternion = Quaternion.Euler(gameManager.GetPlayerRotation(i));
            //        Vector3 playerPosition = gameManager.GetPlayerPosition(i);
            //        transform.position = new Vector3(playerPosition.x, playerPosition.y/* + 1.0f*/, playerPosition.z);
            //        transform.eulerAngles = new Vector3(0, 0, 0);
            //        myRot = rotationQuaternion;
            //        transform.rotation = myRot;

            //        mx = 0;
            //        sitting = true;
            //        preSitting = false;
            //        myAnim.SetBool("Sitting", sitting);
            //        gameManager.OnStaticCamera();
            //        return;
            //    }
            //}

        }
    }

    void Rotate()
    {
        if (pv.IsMine)
        {
            if (!Cursor.visible && !isReady)
            {
                // 사용자의 마우스 좌우 드래그 입력을 받는다.
                mx += Input.GetAxis("Mouse X") * rotSpeed * Time.deltaTime;

                // 입력받은 방향에 따라 플레이어를 좌우로 회전한다.
                transform.eulerAngles = new Vector3(0, mx, 0);
            }
        }
        else
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, myRot, Time.deltaTime * trackingSpeed);
        }
    }

    public bool GetSitting()
    {
        return isReady;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // 만일, 데이터를 서버에 전송(PhotonView.IsMine == true)하는 상태라면...
        if (stream.IsWriting)
        {
            // iterable 데이터를 보낸다.
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(isReady);
        }
        // 그렇지 않고, 만일 데이터를 서버로부터 읽어야하는 상태라면...
        else if (stream.IsReading)
        {
            myPos = (Vector3)stream.ReceiveNext();
            myRot = (Quaternion)stream.ReceiveNext();
            isReady = (bool)stream.ReceiveNext();
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Chair") && pv.IsMine) // 만약 Chair라는 레이어의 마스크를 가지고 있다면
        {

            chair = other.GetComponent<Chair>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Chair") && pv.IsMine) // 만약 Chair라는 레이어의 마스크를 가지고 있다면
        {

            chair = null;
        }
    }
}

// 