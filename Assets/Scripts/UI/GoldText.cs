using System;
using TMPro;
using UnityEngine;

public class GoldText : MonoBehaviour
{
    public TextMeshProUGUI GoldAmountText;
    private GoldManager _goldManager;

    void Start()
    {
        _goldManager = FindFirstObjectByType<GoldManager>();
    }

    void Update()
    {
        GoldAmountText.text = "<color=yellow>x " + _goldManager.Gold.ToString();
    }
}
