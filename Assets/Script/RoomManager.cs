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
    [SerializeField] private Button mapRightButton;
    [SerializeField] private Button mapLeftButton;
    [SerializeField] private Image mapImage;
    [SerializeField] private Sprite[] mapSprites;
    [SerializeField] private GameObject playerPanelItemPrefabs;
    [SerializeField] private Transform playerPanelContent;

    public int mapIndex;
    private Dictionary<int, PlayerPanelItem> playerPanels = new Dictionary<int, PlayerPanelItem>();

    private void Start()
    {
        leaveButton.onClick.AddListener(LeaveRoom);
        mapLeftButton.onClick.AddListener(ClickLeftMapButton);
        mapRightButton.onClick.AddListener(ClickRightMapButton);
        mapIndex = 0;
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
            mapLeftButton.interactable = false;
            mapRightButton.interactable = false;
            MapChange();
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

    public void ClickLeftMapButton()
    {
        mapIndex--;

        if(mapIndex == -1) // ���ǽ��� < 0 ���� �������� ���� ������ ����.
        {
            mapIndex = mapSprites.Length - 1;
        }

        ExitGames.Client.Photon.Hashtable roomProperty = new ExitGames.Client.Photon.Hashtable();
        roomProperty["Map"] = mapIndex;
        PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperty);


        MapChange();
    }

    public void ClickRightMapButton()
    {
        mapIndex++;

        if(mapIndex == mapSprites.Length) // ���ǽ��� > mapSprites.Length �� ������ ���� ������ ����. ��?
        {
            mapIndex = 0;
        }

        ExitGames.Client.Photon.Hashtable roomProperty = new ExitGames.Client.Photon.Hashtable();
        roomProperty["Map"] = mapIndex;
        PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperty);

        MapChange();
    }

    public void MapChange()
    {
        mapIndex = (int)PhotonNetwork.CurrentRoom.CustomProperties["Map"];
        Debug.Log(mapIndex);
        mapImage.sprite = mapSprites[mapIndex];
    }
}
