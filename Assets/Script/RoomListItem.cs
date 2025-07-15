using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;
using UnityEngine.Rendering;
using Photon.Pun;
public class RoomListItem : MonoBehaviour
{
    // �� �̸�
    [SerializeField] private TextMeshProUGUI roomNameText;
    // ���� �ο�
    [SerializeField] private TextMeshProUGUI playerContText;
    // ��ư�� ���� �� ���� 
    [SerializeField] private Button joinButton;

    private string roomName;
    public void Init(RoomInfo info)
    {
        roomName = info.Name;
        roomNameText.text = $"Room Name : {roomName}";
        playerContText.text = $"{info.PlayerCount} / {info.MaxPlayers}";

        joinButton.onClick.AddListener(JoinRoom);
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(roomName);
        joinButton.onClick.RemoveListener(JoinRoom);   // = joinButton.interactable = false; �ߺ� �Է� ������ ����

    }
}
