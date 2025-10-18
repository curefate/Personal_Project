using UnityEngine;

public class HorizontalUIArranger : MonoBehaviour
{
    private float totalWidth;

    void Start()
    {
        RectTransform rt = GetComponent<RectTransform>();
        totalWidth = rt.rect.width;
    }

    void Update()
    {
        ArrangeChildren();
    }

    void ArrangeChildren()
    {
        int childCount = transform.childCount;
        if (childCount == 0) return;

        float spacing = totalWidth / (childCount + 1);
        float startX = -totalWidth / 2f + spacing;
        for (int i = 0; i < childCount; i++)
        {
            RectTransform child = transform.GetChild(i) as RectTransform;
            if (child != null)
            {
                child.anchoredPosition = Vector2.Lerp(child.anchoredPosition, new Vector2(startX + i * spacing, child.anchoredPosition.y), Time.deltaTime * 10f);
            }
        }
    }
}