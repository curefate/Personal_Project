using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    public Color DamageColor = Color.red;
    private Image _hpBar;
    private float _maxWidth;
    private PlayerController _player;
    private int _lastHp;
    private Color _originalColor;

    void Start()
    {
        _hpBar = GetComponent<Image>();
        _maxWidth = _hpBar.rectTransform.sizeDelta.x;
        _originalColor = _hpBar.color;
        _player = FindFirstObjectByType<PlayerController>();
        if (_player == null)
        {
            Debug.LogError("PlayerController not found in the scene.");
        }
        _lastHp = _player.Health;
    }

    void Update()
    {
        if (_player == null) return;

        // Update HP bar width
        float hpRatio = (float)_player.Health / _player.MaxHp;
        _hpBar.rectTransform.sizeDelta = Vector2.Lerp(_hpBar.rectTransform.sizeDelta, new Vector2(_maxWidth * hpRatio, _hpBar.rectTransform.sizeDelta.y), Time.deltaTime * 10f);

        if (_player.Health < _lastHp)
        {
            _hpBar.color = DamageColor;
        }
        else
        {
            _hpBar.color = Color.Lerp(_hpBar.color, _originalColor, Time.deltaTime * 5f);
        }

        _lastHp = _player.Health;
    }
}
