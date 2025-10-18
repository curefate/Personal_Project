using TMPro;
using UnityEngine;

public class TowerShopItem : MonoBehaviour
{
    public GameObject TowerPrefab;
    public TextMeshProUGUI GoldText;

    private Tower towerComponent;
    private PlayerController playerController;

    void Start()
    {
        towerComponent = TowerPrefab.GetComponent<Tower>();
        playerController = FindFirstObjectByType<PlayerController>();
    }

    void Update()
    {

    }

    public void OnClick()
    {
        if (playerController.Gold >= towerComponent.Cost)
        {
            playerController.SelectTower(Instantiate(TowerPrefab));
            playerController.CloseMenu();
        }
        else
        {
            GoldText.fontSize *= 1.5f;
        }
    }
}
