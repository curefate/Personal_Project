using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HpUI : MonoBehaviour
{
    public TextMeshProUGUI HpText;
    private Color _damageColor = Color.red;
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
            _hpBar.color = _damageColor;
        }
        else
        {
            _hpBar.color = Color.Lerp(_hpBar.color, _originalColor, Time.deltaTime * 5f);
        }

        if (!_player.IsDead)
        {
            HpText.text = $"HP: {_player.Health}/{_player.MaxHp}";
        }
        else
        {
            HpText.text = $"Respawn in: {_player.RemainRespawnTime}s";
        }
        
        _lastHp = _player.Health;
    }
}
