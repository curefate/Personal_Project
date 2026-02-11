using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Tower))]
public class TowerBluePrint : MonoBehaviour
{
    public int Cost;
    public GameObject RealTowerPrefab;
    public GameObject ModelChild;

    public Vector3 TargetScale;
    public Vector3 InitialOffset;
    public Material HighlightMaterial;

    private Vector3 _originScale;
    private Vector3 _hoverScale;
    private Vector3 _realScale;
    private GridBrick _gridBrick;
    private TowerSizeType _sizeType;
    private bool _isHeld;
    private Material _originalMaterial;
    private Renderer[] _renderers;
    private HapticImpulsePlayer _hapticPlayer;

    private GridManager _gridManager;
    private TowerManager _towerManager;
    private GoldManager _goldManager;
    private ParticleSpawner _particleSpawner;
    private AudioPlayer _audioPlayer;

    private bool _isHaptic;

    private void Awake()
    {
        _originalMaterial = GetComponentInChildren<Renderer>().material;
        _gridManager = FindFirstObjectByType<GridManager>();
        _towerManager = FindFirstObjectByType<TowerManager>();
        _originScale = transform.localScale;
        _hoverScale = _originScale * 1.2f;
        _sizeType = GetComponent<Tower>().SizeType;
        _renderers = GetComponentsInChildren<Renderer>();
        _realScale = 1 / _originScale.x * TargetScale;
        _particleSpawner = FindFirstObjectByType<ParticleSpawner>();
        _goldManager = FindFirstObjectByType<GoldManager>();
        GetComponent<XRGrabInteractable>().selectEntered.AddListener(OnGrab);
        _audioPlayer = GetComponent<AudioPlayer>();
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        _hapticPlayer = args.interactorObject.transform.parent.GetComponent<HapticImpulsePlayer>();
    }

    void Update()
    {
        if (!_isHeld) return;

        ModelChild.transform.localScale = _realScale;

        var ray = new Ray(transform.position, Vector3.down);
        Debug.DrawRay(ray.origin, ray.direction * 4f, Color.red);
        if (Physics.Raycast(ray, out RaycastHit hit, 4f, LayerMask.GetMask("Ground")))
        {
            _gridBrick = hit.collider.GetComponent<GridBrick>();
        }
        else
        {
            _gridBrick = null;
        }

        bool ifTurn = _sizeType == TowerSizeType.Double ? transform.rotation.eulerAngles.y <= 90 : true;
        if (_gridBrick != null
        && _gridManager.TryMatchPlacement(_gridBrick.Coordinate, _sizeType, out List<Vector3Int> matchCoords, ifTurn)
        && matchCoords != null)
        {
            Vector3 centerPos = _gridBrick.transform.position;
            foreach (var coord in matchCoords)
            {
                centerPos = Vector3.Lerp(centerPos, _gridManager.GetBrickAt(coord).transform.position, 1f / matchCoords.Count);
            }
            ModelChild.transform.position = centerPos + InitialOffset;
            foreach (var mat in _renderers)
            {
                mat.material = HighlightMaterial;
            }
            if (!_isHaptic)
            {
                _hapticPlayer.SendHapticImpulse(0.5f, 0.1f);
                _isHaptic = true;
            }
        }
        else
        {
            ModelChild.transform.position = transform.position;
            foreach (var mat in _renderers)
            {
                mat.material = _originalMaterial;
            }
            _isHaptic = false;
        }
    }

    public void OnHover()
    {
        if (_isHeld) return;
        transform.localScale = _hoverScale;
        foreach (var mat in _renderers)
        {
            mat.material = HighlightMaterial;
        }
    }

    public void ExitHover()
    {
        if (_isHeld) return;
        transform.localScale = _originScale;
        foreach (var mat in _renderers)
        {
            mat.material = _originalMaterial;
        }
    }

    public void OnSelect()
    {
        _isHeld = true;
        transform.parent = null;
        foreach (var mat in _renderers)
        {
            mat.material = _originalMaterial;
        }
    }

    public void ExitSelect()
    {
        Destroy(gameObject);
    }

    public void Onactivate()
    {
        if (_gridBrick == null) return;

        _hapticPlayer.SendHapticImpulse(0.5f, 0.2f);
        if (_gridBrick != null
        && _gridManager.TryMatchPlacement(_gridBrick.Coordinate, _sizeType, out List<Vector3Int> matchCoords, true)
        && matchCoords != null)
        {
            if (_goldManager.Gold < Cost)
            {
                _audioPlayer.PlayOneShotFromAsset("error");
                return;
            }

            Vector3 centerPos = _gridBrick.transform.position;
            foreach (var coord in matchCoords)
            {
                centerPos = Vector3.Lerp(centerPos, _gridManager.GetBrickAt(coord).transform.position, 1f / matchCoords.Count);
            }
            var tower = Instantiate(RealTowerPrefab, centerPos + InitialOffset, Quaternion.identity);
            _goldManager.Gold -= Cost;
            _particleSpawner.SpawnParticle("smoke", centerPos + InitialOffset);
            _audioPlayer.PlayOneShotFromAsset("build");
            foreach (var coord in matchCoords)
            {
                _gridManager.GetBrickAt(coord).TowerPrefab = tower;
            }
            _towerManager.TowerList.Add(tower);
        }
    }
}
