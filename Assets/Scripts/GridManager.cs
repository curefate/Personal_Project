using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    private readonly Dictionary<Vector3Int, GridBrick> _grid = new();

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
            Debug.LogWarning($"Grid position {brick.Coordinate} is already occupied.");
            return;
        }
        _grid[brick.Coordinate] = brick;
    }

    public GridBrick GetBrickAt(Vector3Int coord)
    {
        return _grid.TryGetValue(coord, out var brick) ? brick : null;
    }
}
