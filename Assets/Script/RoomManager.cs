using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomManager : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button leaveButton;

    [SerializeField] private GameObject playerPanelItemPrefabs;
    [SerializeField] private Transform playerPanelContent;

    private Dictionary<int, PlayerPanelItem> playerPanels = new Dictionary<int, PlayerPanelItem>();

    private void Start()
    {
        leaveButton.onClick.AddListener(LeaveRoom);
    }
    public void PlayerPanelSpawn(Player player)
    {
        // ���� �÷��̾ ���ο� �÷��̾� ���� �� ȣ��
        GameObject obj = Instantiate(playerPanelItemPrefabs);
        obj.transform.SetParent(playerPanelContent);
        PlayerPanelItem item = obj.GetComponent<PlayerPanelItem>();
        item.Init(player);
        playerPanels.Add(player.ActorNumber, item);
    }

    public void PlayerPanelSpawn()
    {
        if(!PhotonNetwork.IsMasterClient)
        {
            startButton.interactable = false;
        }

        
        // ���� ���� ���������� ȣ��
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            GameObject obj = Instantiate(playerPanelItemPrefabs);
            obj.transform.SetParent(playerPanelContent);
            PlayerPanelItem item = obj.GetComponent<PlayerPanelItem>();
            item.Init(player);
            playerPanels.Add(player.ActorNumber, item);
        }
    }

    public void PlayerPanelDestroy(Player player)
    {
        if(playerPanels.TryGetValue(player.ActorNumber, out PlayerPanelItem panel))
        {
            Destroy(panel.gameObject);
            playerPanels.Remove(player.ActorNumber);
        }
        else
        {
            Debug.LogError("�г��� �������� ����");
        }
    }

    public void LeaveRoom()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            Destroy(playerPanels[player.ActorNumber].gameObject);
        }

        playerPanels.Clear();

        PhotonNetwork.LeaveRoom();
    }
}
