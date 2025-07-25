using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;
using UnityEngine.Rendering;
using Photon.Pun;

public enum MapType
{
    City, Sea, Desert
}
public class RoomListItem : MonoBehaviour
{
    // 방 이름
    [SerializeField] private TextMeshProUGUI roomNameText;
    // 입장 인원
    [SerializeField] private TextMeshProUGUI playerContText;

    [SerializeField] private TextMeshProUGUI mapText;
    // 버튼을 통한 방 참가 
    [SerializeField] private Button joinButton;

    private string roomName;
    public void Init(RoomInfo info)
    {
        roomName = info.Name;
        roomNameText.text = $"Room Name : {roomName}";
        playerContText.text = $"{info.PlayerCount} / {info.MaxPlayers}";
        mapText.text = $"Map : {(MapType)info.CustomProperties["Map"]}";

        joinButton.onClick.AddListener(JoinRoom);
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(roomName);
        joinButton.onClick.RemoveListener(JoinRoom);   // = joinButton.interactable = false; 중복 입력 방지를 위함

    }
}
