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
        // 기존 플레이어가 새로운 플레이어 입장 시 호출
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

        
        // 내가 새로 입장했을때 호출
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
            Debug.LogError("패널이 존재하지 않음");
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

        if(mapIndex == -1) // 조건식을 < 0 으로 했을때는 뭔가 오류가 난다.
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

        if(mapIndex == mapSprites.Length) // 조건식을 > mapSprites.Length 로 했을때 뭔가 오류가 난다. 왜?
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
