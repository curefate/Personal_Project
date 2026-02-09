using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Tower))]
public class TowerBluePrint : MonoBehaviour
{
    public int Cost;
    public GameObject RealTowerPrefab;
    public GameObject ModelChild;

    public Vector3 TargetScale;
    public Vector3 InitialOffset;
    private Vector3 _originScale;
    private Vector3 _hoverScale;
    private GridBrick _gridBrick;
    private TowerSizeType _sizeType;
    private bool _isHeld;

    private GridManager _gridManager;
    private TowerManager _towerManager;

    private void Awake()
    {
        _gridManager = FindFirstObjectByType<GridManager>();
        _towerManager = FindFirstObjectByType<TowerManager>();
        _originScale = transform.localScale;
        _hoverScale = _originScale * 1.2f;
        _sizeType = GetComponent<Tower>().SizeType;
    }

    void Update()
    {
        if (!_isHeld) return;

        transform.localScale = TargetScale;

        var ray = new Ray(transform.position, Vector3.down);
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, LayerMask.GetMask("Ground")))
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
            ModelChild.transform.position = centerPos + Vector3.up * 1f + InitialOffset;
        }
        else
        {
            ModelChild.transform.position = transform.position;
        }
    }

    public void OnHover()
    {
        if (_isHeld) return;
        transform.localScale = _hoverScale;
    }

    public void ExitHover()
    {
        if (_isHeld) return;
        transform.localScale = _originScale;
    }

    public void OnSelect()
    {
        _isHeld = true;
        transform.parent = null;
        transform.localScale = TargetScale;
    }

    public void ExitSelect()
    {
        Destroy(gameObject);
    }

    public void Onactivate()
    {
        if (_gridBrick == null) return;

        if (_gridBrick != null
        && _gridManager.TryMatchPlacement(_gridBrick.Coordinate, _sizeType, out List<Vector3Int> matchCoords, true)
        && matchCoords != null)
        {
            Vector3 centerPos = _gridBrick.transform.position;
            foreach (var coord in matchCoords)
            {
                centerPos = Vector3.Lerp(centerPos, _gridManager.GetBrickAt(coord).transform.position, 1f / matchCoords.Count);
            }
            var tower = Instantiate(RealTowerPrefab, centerPos + InitialOffset, Quaternion.identity);
            foreach (var coord in matchCoords)
            {
                _gridManager.GetBrickAt(coord).TowerPrefab = tower;
            }
            _towerManager.TowerList.Add(tower);
            // TODO others, cost gold, etc.
            Debug.Log($"Tower placed at {centerPos}");
        }
    }
}
