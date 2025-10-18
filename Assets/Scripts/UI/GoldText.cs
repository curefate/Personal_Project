using System;
using TMPro;
using UnityEngine;

public class GoldText : MonoBehaviour
{
    public TextMeshProUGUI GoldAmountText;
    public int FontSize = 36;
    private PlayerController playerController;

    void Start()
    {
        playerController = FindFirstObjectByType<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("PlayerController not found in the scene.");
        }
    }

    void Update()
    {
        GoldAmountText.text = "Gold: " + playerController.Gold.ToString();
        GoldAmountText.fontSize = Mathf.Lerp(GoldAmountText.fontSize, FontSize, Time.deltaTime * 5f);
    }
}
