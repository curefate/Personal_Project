using TMPro;
using UnityEngine;

public class EnergyText : MonoBehaviour
{
    public TextMeshProUGUI EnergyAmountText;
    public RectTransform EnergyBarFill;
    private EnergyManager _energyManager;

    private float _originalEnergyBarWidth;

    void Start()
    {
        _energyManager = FindFirstObjectByType<EnergyManager>();
        _originalEnergyBarWidth = EnergyBarFill.sizeDelta.x;
    }

    void Update()
    {
        EnergyBarFill.sizeDelta = Vector2.Lerp(EnergyBarFill.sizeDelta, new Vector2(_energyManager.EnergyPercentage / 100 * _originalEnergyBarWidth, EnergyBarFill.sizeDelta.y), Time.deltaTime * 10);
        EnergyAmountText.text = "Energy:" + _energyManager.EnergyPercentage.ToString() + "%";
    }
}
