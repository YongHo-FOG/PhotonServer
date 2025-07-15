using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;
using Photon.Pun;

public class PlayerPanelItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nicknameText;
    [SerializeField] private TextMeshProUGUI readyText;
    [SerializeField] private Image hostImage;
    [SerializeField] private Image readyButtonImage;
    [SerializeField] private Button ReadyButton;

    private bool isReady;
    public void Init(Player player)
    {
        nicknameText.text = player.NickName;
        hostImage.enabled = player.IsMasterClient;
        ReadyButton.interactable = player.IsLocal;

        if(!player.IsLocal)
        {
            return;
        }

        isReady = false;

        Debug.Log("����");
        ReadyPropertyUpdate();

        ReadyButton.onClick.RemoveListener(ReadyButtonClick);
        ReadyButton.onClick.AddListener(ReadyButtonClick);
    }

    public void ReadyButtonClick()
    {
        isReady = !isReady;

        readyText.text = isReady ? "Ready" : "Click Ready";

        readyButtonImage.color = isReady ? Color.green : Color.gray;

        ReadyPropertyUpdate();
    }

    public void ReadyPropertyUpdate()
    {
        ExitGames.Client.Photon.Hashtable playerProperty = new ExitGames.Client.Photon.Hashtable();
        playerProperty["Ready"] = isReady;
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperty);
    }

    public void ReadyCheck(Player player)
    {
        if(player.CustomProperties.TryGetValue("Ready", out object value))
        {
            // if((bool) value)
            // {
            //
            // }
            // else
            // {
            //
            // }

            readyText.text = (bool)value ? "Ready" : "Click Ready";

            readyButtonImage.color = (bool)value ? Color.green : Color.gray;
        }
    }
}
