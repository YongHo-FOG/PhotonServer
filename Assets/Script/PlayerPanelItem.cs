using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;

public class PlayerPanelItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nicknameText;
    [SerializeField] private TextMeshProUGUI readyText;
    [SerializeField] private Image hostImage;
    [SerializeField] private Image readyButtonImage;
    [SerializeField] private Button ReadyButton;

    public void Init(Player player)
    {
        nicknameText.text = player.NickName;
        hostImage.enabled = player.IsMasterClient;
        ReadyButton.interactable = player.IsLocal;
    }
}
