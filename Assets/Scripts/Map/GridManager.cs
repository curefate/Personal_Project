using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    private readonly Dictionary<Vector3Int, GridBrick> _grid = new();


    public bool TryMatchPlacement(
    Vector3Int seed,
    TowerSizeType sizeType,
    out List<Vector3Int> matchedCoordinators,
    bool preferHorizontal = true
)
    {
        matchedCoordinators = null;

        if (!_grid.ContainsKey(seed) || !_grid[seed].IfEmpty)
            return false;

        // 单格子直接返回
        if (sizeType == TowerSizeType.Single)
        {
            matchedCoordinators = new List<Vector3Int> { seed };
            return true;
        }

        // Double 类型匹配：仅横向或纵向
        if (sizeType == TowerSizeType.Double)
        {
            Vector3Int[] directions = preferHorizontal
                ? new[] { Vector3Int.right, Vector3Int.left }
                : new[] { Vector3Int.forward, Vector3Int.back };

            foreach (var dir in directions)
            {
                Vector3Int second = seed + dir;
                if (_grid.ContainsKey(second) && _grid[second].IfEmpty)
                {
                    matchedCoordinators = new List<Vector3Int> { seed, second };
                    return true;
                }
            }

            return false;
        }

        // Quadruple 类型匹配：尝试四个方向的 2x2 区域
        Vector3Int[,] quadruplePatterns = {
                    // Pattern 1: coord at top-left
                    { new Vector3Int(0, 0, 0), new Vector3Int(1, 0, 0), new Vector3Int(1, 0, -1), new Vector3Int(0, 0, -1) },
                    // Pattern 2: coord at top-right  
                    { new Vector3Int(-1, 0, 0), new Vector3Int(0, 0, 0), new Vector3Int(0, 0, -1), new Vector3Int(-1, 0, -1) },
                    // Pattern 3: coord at bottom-right
                    { new Vector3Int(-1, 0, 1), new Vector3Int(0, 0, 1), new Vector3Int(0, 0, 0), new Vector3Int(-1, 0, 0) },
                    // Pattern 4: coord at bottom-left
                    { new Vector3Int(0, 0, 1), new Vector3Int(1, 0, 1), new Vector3Int(1, 0, 0), new Vector3Int(0, 0, 0) }
        };
        for (int patternIndex = 0; patternIndex < quadruplePatterns.GetLength(0); patternIndex++)
        {
            List<Vector3Int> patternCoords = new();
            foreach (var offset in quadruplePatterns)
            {
                Vector3Int checkCoord = seed + offset;
                if (_grid.ContainsKey(checkCoord) && _grid[checkCoord].IfEmpty)
                {
                    patternCoords.Add(checkCoord);
                }
                else
                {
                    break;
                }
            }
            if (patternCoords.Count == 4)
            {
                matchedCoordinators = patternCoords;
                return true;
            }
        }

        return false;
    }


    public List<Vector3Int> CheckEmpty(Vector3Int coord, TowerSizeType towerSizeType, out bool isValid, bool isfromTop = true)
    {
        if (!_grid.ContainsKey(coord) || !_grid[coord].IfEmpty)
        {
            isValid = false;
            return null;
        }

        List<Vector3Int> requiredCoords = new() { coord };

        switch (towerSizeType)
        {
            //=====================================================
            case TowerSizeType.Single:
                isValid = true;
                return requiredCoords;

            //=====================================================
            case TowerSizeType.Double:
                Vector3Int[] doubleOffsets = isfromTop ?
                    new Vector3Int[] {
                        new(0, 0, 1),   // Top
                        new(1, 0, 0),   // Right
                        new(0, 0, -1),  // Bottom
                        new(-1, 0, 0)   // Left
                    } :
                    new Vector3Int[] {
                        new(1, 0, 0),   // Right
                        new(0, 0, -1),  // Bottom
                        new(-1, 0, 0),  // Left
                        new(0, 0, 1)    // Top
                    };

                foreach (var offset in doubleOffsets)
                {
                    Vector3Int adjacentCoord = coord + offset;
                    if (_grid.ContainsKey(adjacentCoord) && _grid[adjacentCoord].IfEmpty)
                    {
                        requiredCoords.Add(adjacentCoord);
                        isValid = true;
                        return requiredCoords;
                    }
                }
                isValid = false;
                return null;

            //=====================================================
            // Quadruple tower needs a 2x2 grid
            case TowerSizeType.Quadruple:
                Vector3Int[,] quadruplePatterns = {
                    // Pattern 1: coord at top-left
                    { new Vector3Int(0, 0, 0), new Vector3Int(1, 0, 0), new Vector3Int(1, 0, -1), new Vector3Int(0, 0, -1) },
                    // Pattern 2: coord at top-right  
                    { new Vector3Int(-1, 0, 0), new Vector3Int(0, 0, 0), new Vector3Int(0, 0, -1), new Vector3Int(-1, 0, -1) },
                    // Pattern 3: coord at bottom-right
                    { new Vector3Int(-1, 0, 1), new Vector3Int(0, 0, 1), new Vector3Int(0, 0, 0), new Vector3Int(-1, 0, 0) },
                    // Pattern 4: coord at bottom-left
                    { new Vector3Int(0, 0, 1), new Vector3Int(1, 0, 1), new Vector3Int(1, 0, 0), new Vector3Int(0, 0, 0) }
                };

                for (int patternIndex = 0; patternIndex < quadruplePatterns.GetLength(0); patternIndex++)
                {
                    List<Vector3Int> patternCoords = new();
                    foreach (var offset in quadruplePatterns)
                    {
                        Vector3Int checkCoord = coord + offset;
                        if (_grid.ContainsKey(checkCoord) && _grid[checkCoord].IfEmpty)
                        {
                            patternCoords.Add(checkCoord);
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (patternCoords.Count == 4)
                    {
                        isValid = true;
                        return patternCoords;
                    }
                }
                isValid = false;
                return null;

            default:
                throw new System.ArgumentOutOfRangeException(nameof(towerSizeType), towerSizeType, null);
        }
    }

    public void RegisterBrick(GridBrick brick)
    {
        if (!brick.IsGrid) return;
        if (_grid.ContainsKey(brick.Coordinate))
        {
            Debug.LogWarning($"Grid position {brick.Coordinate} is already occupied.\nWorld Position: {brick.transform.position}");
            return;
        }
        _grid[brick.Coordinate] = brick;
    }

    public GridBrick GetBrickAt(Vector3Int coord)
    {
        return _grid.TryGetValue(coord, out var brick) ? brick : null;
    }
}
