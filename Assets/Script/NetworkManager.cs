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
    private void Update()
    {
        stateText.text = $"Current State : {PhotonNetwork.NetworkClientState}";
    }
}
