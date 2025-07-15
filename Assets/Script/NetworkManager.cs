using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
public class NetworkManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private TextMeshProUGUI stateText;
    [SerializeField] private GameObject nicknamePanel;
    [SerializeField] private TMP_InputField nicknameField;
    [SerializeField] private Button nicknameAdmitButton;

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        nicknameAdmitButton.onClick.AddListener(NicknameAdmit);

       // 연결 끊기
       // PhotonNetwork.Disconnect();
    }

   // public override void OnConnected()
   // { 
   //     base.OnConnected();
   //     Debug.Log("연결");
   // }
   // 순서상 디버그에서는 온커넥티드가 먼저 로드됨만 알아두기

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("마스터 연결");
        loadingPanel.SetActive(false); // 연결이 다 되면 로딩패널을 끔
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        // Debug.Log(cause.ToString()); 이유찾기
        PhotonNetwork.ConnectUsingSettings(); // 문제가있어서 연결이 해제되면 재연결 시도
    }

    public void NicknameAdmit()
    {
        if (string.IsNullOrWhiteSpace(nicknameField.text))
        {
            Debug.LogError("닉네임 입력값 없음");
            return;
        }
        PhotonNetwork.NickName = nicknameField.text;
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        nicknamePanel.SetActive(false);  // 닉네임 연결되서 로비에 접속되면 닉네임패널 활성화 끔
        Debug.Log("로비 참가");
    }
    private void Update()
    {
        stateText.text = $"Current State : {PhotonNetwork.NetworkClientState}";
    }
}
