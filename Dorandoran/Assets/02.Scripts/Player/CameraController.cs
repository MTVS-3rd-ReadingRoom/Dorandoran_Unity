using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CameraController : MonoBehaviour
{
    public Transform[] basePositions;
    public Vector3 nearOffset;
    public Vector3 farOffset;
    public float distance = 5.0f;
    public float rotSpeed = 300;

    CameraFollow followCam;
    float scrollValue = 1;
    Vector3 minLocalPos;
    Vector3 maxLocalPos;
    float my;

    void Start()
    {
        // nearPos의 위치를 플레이어의 중앙에서 nearOffset만큼 떨어지도록 조정한다.
        minLocalPos = nearOffset;

        // farPos의 위치를 nearPos의 위치로부터 farOffset만큼 떨어지도록 조정한다.
        Vector3 camDir = farOffset.normalized * distance;
        maxLocalPos = minLocalPos + camDir;

        // 만일, 이 캐릭터가 캐릭터의 소유권자라면...
        // 메인 카메라의 CameraFollow 컴포넌트에서 follow Target과 LookAt Target을 모두 farPos로 지정한다.
        if (GetComponent<PhotonView>().IsMine && Camera.main.transform.GetComponent<CameraFollow>())
        {
            followCam = Camera.main.transform.GetComponent<CameraFollow>();
            followCam.SetCameraFollowTarget(basePositions[1]);
            followCam.SetCameraLookAtTarget(basePositions[1]);
        }
        basePositions[0].localPosition = minLocalPos;
    }

    void Update()
    {
        
        basePositions[1].localPosition = SetCameraPosition();
        basePositions[1].localEulerAngles = SetCameraRotation();

    }

    Vector3 SetCameraPosition()
    {
        // 마우스 휠을 움직이면 farPos의 위치가 nearPos의 위치로부터 플레이어의 뒷 편으로 최대 5미터까지만 멀어지거나 가까워지도록 한다.
        scrollValue = Mathf.Clamp01(scrollValue - Input.GetAxis("Mouse ScrollWheel"));
        Vector3 newCampos = Vector3.Lerp(minLocalPos, maxLocalPos, scrollValue);

        // 단, nearPos와 farPos 사이에 장애물이 있다면 장애물 앞쪽으로 farPos를 당긴다(Player 레이어어는 장애물이 아님).
        Vector3 rayDirection = transform.TransformDirection(farOffset.normalized);
        Ray ray = new Ray(transform.position + minLocalPos, rayDirection);
        RaycastHit hitInfo;
        float rayDist = Mathf.Lerp(0, distance, scrollValue);
        if (Physics.Raycast(ray, out hitInfo, rayDist, ~(1 << 6)))
        {
            Vector3 calcPos = hitInfo.point + farOffset * -0.01f;
            newCampos = calcPos - transform.position;
            newCampos = Quaternion.Inverse(transform.rotation) * newCampos;
        }
        return newCampos;
    }

    Vector3 SetCameraRotation()
    {
        my -= Input.GetAxis("Mouse Y") * rotSpeed * Time.deltaTime;
        my = Mathf.Clamp(my, -30, 30);
        return new Vector3(my, 0, 0);
    }
}
