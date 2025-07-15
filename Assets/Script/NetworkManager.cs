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
        // ���� ����
        // PhotonNetwork.Disconnect();
    }

    // public override void OnConnected()
    // { 
    //     base.OnConnected();
    //     Debug.Log("����");
    // }
    // ������ ����׿����� ��Ŀ��Ƽ�尡 ���� �ε�ʸ� �˾Ƶα�

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("������ ����");
        loadingPanel.SetActive(false); // ������ �� �Ǹ� �ε��г��� ��
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        // Debug.Log(cause.ToString()); ����ã��
        PhotonNetwork.ConnectUsingSettings(); // �������־ ������ �����Ǹ� �翬�� �õ�
    }

    public void NicknameAdmit()
    {
        if (string.IsNullOrWhiteSpace(nicknameField.text))
        {
            Debug.LogError("�г��� �Է°� ����");
            return;
        }
        PhotonNetwork.NickName = nicknameField.text;
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        nicknamePanel.SetActive(false);  // �г��� ����Ǽ� �κ� ���ӵǸ� �г����г� Ȱ��ȭ ��
        Debug.Log("�κ� ����");
    }

    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(roomNameField.text))
        {
            Debug.LogError("�� �̸� �Է� ����");
            return;
        }

        roomNameAdmitButton.interactable = false; // �ߺ� �Է� ������ ����

        RoomOptions options = new RoomOptions { MaxPlayers = 8 }; // ���Ŀ� ui���� �� ������ ���� ������ ����Ѵٸ� �ִ��ο��� ������ ���� ����
        PhotonNetwork.CreateRoom(roomNameField.text, options);
        roomNameField.text = null;
    }

    public override void OnCreatedRoom() // ���� �۾����� ȣ��Ʈ�ν� �濡 �����ϴ� �������� ���ԵǾ��ִ�.
    {
        base.OnCreatedRoom();
        lobbyPanel.SetActive(false);
        roomNameAdmitButton.interactable = true; // �ٽ� ��ư Ȱ��ȭ
        Debug.Log("�� ���� �Ϸ�");
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        lobbyPanel.SetActive(false);
        Debug.Log("�� ���� �Ϸ�");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);

        // �ŰԺ����� roomList�� ��ȸ�Ѵ�
        foreach (RoomInfo info in roomList)
        {
            // ���� �����ɶ�.
            if (info.RemovedFromList)
            {
                // ��ųʸ��� ���� �ִ��� Ȯ��
                if(roomListItems.TryGetValue(info.Name,out GameObject obj))
                {
                    // ����Ʈ ����
                    Destroy(obj);
                    // ��ųʸ����� ����
                    roomListItems.Remove(info.Name);
                }
                continue;
            }

            // roomListItems �� �ش� ���� ��������
            if (roomListItems.ContainsKey(info.Name))
            {
                // �÷��̾� ���� ����Ǵ� ���.
                roomListItems[info.Name].GetComponent<RoomListItem>().Init(info);
            }
            else // �ش� ���� ������. �κ� ���� ���� �߰ų�, ���� ���� �����Ǿ�����
            {
                // �� ����Ʈ ������Ʈ�� ����
                GameObject roomListItem = Instantiate(roomListItemPrefabs);
                // ��ũ�Ѻ��� ����Ʈ�� �־��ִ� �۾�
                roomListItem.transform.SetParent(roomListContent);
                // �ʱ�ȭ
                roomListItem.GetComponent<RoomListItem>().Init(info);
                // ��ųʸ��� �ش� �� ������ �߰�.
                roomListItems.Add(info.Name, roomListItem);
            }
        }

    }
    private void Update()
    {
        stateText.text = $"Current State : {PhotonNetwork.NetworkClientState}";
    }
}
