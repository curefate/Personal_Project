using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextPusher : MonoBehaviour
{
    public float intervalTime;

    public bool isPushing { get; private set; }
    private TextMeshProUGUI textMeshPro;

    private void Awake()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
    }

    public void PushText(string text)
    {
        textMeshPro.text = "";
        if (isPushing)
            StopAllCoroutines();
        isPushing = false;
        StartCoroutine(PushText1by1(text));
    }

    IEnumerator PushText1by1(string text)
    {
        isPushing = true;
        foreach (char c in text)
        {
            textMeshPro.text += c;
            yield return new WaitForSeconds(intervalTime);
        }
        isPushing = false;
    }

    public void DestroyFromRoot()
    {
        StopAllCoroutines();
        Destroy(transform.root.gameObject);
    }
}
