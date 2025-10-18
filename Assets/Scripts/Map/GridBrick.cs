using UnityEngine;
using System.Collections.Generic;

public class GridBrick : MonoBehaviour
{
    public static int GridSize = 2;
    public bool IsGrid = true;
    public GameObject TowerPrefab = null;

    public Vector3Int Coordinate { get; private set; }
    public bool IfEmpty => TowerPrefab == null;

    private GridManager _gridManager;

    void Start()
    {
        if (IsGrid)
        {
            Vector3 pos = transform.position;
            Coordinate = new Vector3Int(Mathf.RoundToInt(pos.x / GridSize), Mathf.RoundToInt(pos.y / GridSize), Mathf.RoundToInt(pos.z / GridSize));
            _gridManager = FindFirstObjectByType<GridManager>();
            if (_gridManager != null)
            {
                _gridManager.RegisterBrick(this);
            }
            else
            {
                Debug.LogError("GridManager instance not found in the scene.");
            }
        }
    }

    void Update()
    {

    }
}