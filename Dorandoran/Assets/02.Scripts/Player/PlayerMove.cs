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
    PlayerProsAndCons playerProAndCons;
    bool isGround;

    Vector3 velocity;
    Vector3 myPrevPos;
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

        playerProAndCons = GetComponentInChildren<PlayerProsAndCons>();
        //playerID = pv.ViewID;
    }

    void Update()
    {

        ///// �������� ���� ĳ���Ͱ� �ɾ��ִٸ� ȸ�� ����
        if (pv.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.P)) // ���� �ٲ��� �ʾ��� ��
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

                            if(chair.transform.parent.localPosition.z > 0.0f) // ����
                                playerProAndCons.SetCurPlayerProsAndConsData((int)PlayerProsAndCons.DebatePosition.Pro);
                            else // �ݴ�
                                playerProAndCons.SetCurPlayerProsAndConsData((int)PlayerProsAndCons.DebatePosition.Con);
                            return; // �ٷ� ����
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
            if (isReady) // �ɾ�������
            {
                if(chair)
                {
                    transform.position = chair.transform.position;
                }
                transform.eulerAngles = new Vector3(0, 0, 0);
                transform.rotation = myRot;
            }
        }

        Move();
        Rotate();
        //if (!isReady)
        //{

        //}
    }

    bool CheckIsGround()
    {
        return Physics.Raycast(transform.position, Vector3.down, 0.3f, groundLayer);
    }
    void Move()
    {
        // �������� ���� ĳ���Ͷ��
        if (pv.IsMine)
        {
            // ���� ī�޶� �ٶ󺸴� �������� �̵�
            // CharacterController Move �Լ� ���
            if (!Cursor.visible && !isReady)
            {
                float h = Input.GetAxis("Horizontal");
                float v = Input.GetAxis("Vertical");
                Vector3 dir = new Vector3(h, 0, v);
                dir.y = 0.0f;
                dir = transform.TransformDirection(dir);

                if (dir.magnitude > 0.0f) // �̵����� ������
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
            Vector3 targetPos = Vector3.Lerp(transform.position, myPos, Time.deltaTime * trackingSpeed);

            float dist = (targetPos - myPrevPos).magnitude;
            transform.position = dist > 0.01f ? targetPos : myPos;
            //Vector2 animPos = dist > 0.01f ? Vector2.one : Vector2.zero;

            Vector3 localDir = transform.InverseTransformDirection(targetPos - myPrevPos);

            float deltaX = localDir.x;
            float deltaZ = localDir.z;

            float newX = 0;
            float newZ = 0;
            if (Mathf.Abs(deltaX) > 0.01f)
            {
                newX = deltaX > 0 ? 1.0f : -1.0f;
            }

            if (Mathf.Abs(deltaZ) > 0.01f)
            {
                newZ = deltaZ > 0 ? 1.0f : -1.0f;
            }

            myPrevPos = transform.position;
            myAnim.SetFloat("Horizontal", newX);
            myAnim.SetFloat("Vertical", newZ);
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
        // p ������ + ��ó ���ڿ� �浹 + �ɴ� �ִϸ��̼� ����
        if (pv.IsMine && chair != null)
        {
            transform.position = chair.transform.position;
            transform.rotation = chair.transform.rotation;
        }
    }

    void Rotate()
    {
        if (pv.IsMine)
        {
            if (!Cursor.visible && !isReady)
            {
                // ������� ���콺 �¿� �巡�� �Է��� �޴´�.
                mx += Input.GetAxis("Mouse X") * rotSpeed * Time.deltaTime;

                // �Է¹��� ���⿡ ���� �÷��̾ �¿�� ȸ���Ѵ�.
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
        // ����, �����͸� ������ ����(PhotonView.IsMine == true)�ϴ� ���¶��...
        if (stream.IsWriting)
        {
            // iterable �����͸� ������.
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(isReady);
        }
        // �׷��� �ʰ�, ���� �����͸� �����κ��� �о���ϴ� ���¶��...
        else if (stream.IsReading)
        {
            myPos = (Vector3)stream.ReceiveNext();
            myRot = (Quaternion)stream.ReceiveNext();
            isReady = (bool)stream.ReceiveNext();
        }
    }

    //// RPC �Լ�
    [PunRPC]
    void UpdateSound(int SoundPlayerid)
    {
        if (SoundPlayerid == pv.ViewID) // �� �÷��̾� id
        {
            recorder.TransmitEnabled = true; // ���� ���� ����
        }
        else // ų �÷��̾� id
        {
            recorder.TransmitEnabled = false; // ���� ���� ����x
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Chair") && pv.IsMine) // ���� Chair��� ���̾��� ����ũ�� ������ �ִٸ�
        {

            chair = other.GetComponent<Chair>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Chair") && pv.IsMine) // ���� Chair��� ���̾��� ����ũ�� ������ �ִٸ�
        {

            chair = null;
        }
    }
}

// 