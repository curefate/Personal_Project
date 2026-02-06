using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public BattleBase Target;
    public int MaxHealth;
    public RectTransform RedBar;
    public RectTransform GreenBar;

    private float _originWidth;

    void Start()
    {
        _originWidth = GreenBar.sizeDelta.x;
    }

    void Update()
    {
        if (Target == null)
        {
            Destroy(gameObject);
            return;
        }

        var health = Target.Health;
        var ratio = (float)health / MaxHealth;
        GreenBar.sizeDelta = Vector2.Lerp(GreenBar.sizeDelta, new Vector2(_originWidth * ratio, GreenBar.sizeDelta.y), Time.deltaTime * 10);
    }
}
