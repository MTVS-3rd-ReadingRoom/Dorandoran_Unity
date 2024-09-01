using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Com.DoranDoran.ReadingRoom
{
    [RequireComponent(typeof(InputField))] // 요구되는 컴포넌트를 종속성으로 자동으로 추가
    public class PlayerNameInputField : MonoBehaviour
    {
        #region Private Constants

        const string playerNamePrefKey = "PlayerName";

        #endregion

        #region MonoBehaviour CallBacks

        // PlayerPrefs - key, Value의 쌍으로 묶여진 항목
        void Start()
        {
            string defaultName = string.Empty;
            InputField _inputField = this.GetComponent<InputField>();
            if(_inputField != null)
            {
                if (PlayerPrefs.HasKey(playerNamePrefKey))
                {

                    defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                    _inputField.text = defaultName;
                }
            }

            PhotonNetwork.NickName = defaultName;
        }

        #endregion

        #region Public Methods

        public void SetPlayerName(string value)
        {
            // #Important
            if (string.IsNullOrEmpty(value))
            {
                Debug.LogError("Player Name is null or empty");
                return;
            }

            // 네트워크상의 플레이어 이름 설정
            PhotonNetwork.NickName = value;

            PlayerPrefs.SetString(playerNamePrefKey, value);
        }

        #endregion
    }
}
