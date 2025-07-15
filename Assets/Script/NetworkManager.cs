using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class NetworkManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private TextMeshProUGUI stateText;

    [Header("NickName field")]
    [SerializeField] private GameObject nicknamePanel;
    [SerializeField] private TMP_InputField nicknameField;
    [SerializeField] private Button nicknameAdmitButton;

    [Header("Lobby field")]
    [SerializeField] private TMP_InputField roomNameField;
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private Button roomNameAdmitButton;
    [SerializeField] private GameObject roomListItemPrefabs;
    [SerializeField] private Transform roomListContent;
    private Dictionary<string, GameObject> roomListItems = new Dictionary<string, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        nicknameAdmitButton.onClick.AddListener(NicknameAdmit);
        roomNameAdmitButton.onClick.AddListener(CreateRoom);
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

    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(roomNameField.text))
        {
            Debug.LogError("방 이름 입력 없음");
            return;
        }

        roomNameAdmitButton.interactable = false; // 중복 입력 방지를 위함

        RoomOptions options = new RoomOptions { MaxPlayers = 8 }; // 차후에 ui에서 이 값또한 받은 값으로 사용한다면 최대인원수 지정이 직접 가능
        PhotonNetwork.CreateRoom(roomNameField.text, options);
        roomNameField.text = null;
    }

    public override void OnCreatedRoom() // 생성 작업에는 호스트로써 방에 참여하는 과정까지 포함되어있다.
    {
        base.OnCreatedRoom();
        lobbyPanel.SetActive(false);
        roomNameAdmitButton.interactable = true; // 다시 버튼 활성화
        Debug.Log("방 생성 완료");
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        lobbyPanel.SetActive(false);
        Debug.Log("방 참가 완료");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);

        // 매게변수인 roomList를 순회한다
        foreach (RoomInfo info in roomList)
        {
            // 방이 삭제될때.
            if (info.RemovedFromList)
            {
                // 딕셔너리에 값이 있는지 확인
                if(roomListItems.TryGetValue(info.Name,out GameObject obj))
                {
                    // 리스트 삭제
                    Destroy(obj);
                    // 딕셔너리에서 삭제
                    roomListItems.Remove(info.Name);
                }
                continue;
            }

            // roomListItems 에 해당 방이 있을때에
            if (roomListItems.ContainsKey(info.Name))
            {
                // 플레이어 수가 변경되는 경우.
                roomListItems[info.Name].GetComponent<RoomListItem>().Init(info);
            }
            else // 해당 방이 없을때. 로비에 새로 입장 했거나, 방이 새로 생성되었을때
            {
                // 방 리스트 오브젝트를 생성
                GameObject roomListItem = Instantiate(roomListItemPrefabs);
                // 스크롤뷰의 뷰포트에 넣어주는 작업
                roomListItem.transform.SetParent(roomListContent);
                // 초기화
                roomListItem.GetComponent<RoomListItem>().Init(info);
                // 딕셔너리에 해당 방 정보를 추가.
                roomListItems.Add(info.Name, roomListItem);
            }
        }

    }
    private void Update()
    {
        stateText.text = $"Current State : {PhotonNetwork.NetworkClientState}";
    }
}
