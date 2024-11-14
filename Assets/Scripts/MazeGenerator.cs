using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField]
    private MazeCell _mazeCellPrefab;

    [SerializeField]
    private int _mazeWidth;

    [SerializeField]
    private int _mazeDepth;

    [SerializeField]
    private int _sizeMultiplier;

    [SerializeField]
    private int _mobCount;

    [SerializeField]
    private GameObject _mobPrefab;

    [SerializeField]
    private GameObject _player;

    [SerializeField]
    private GameObject _stalker;

    private MazeCell[,] _mazeGrid;

    [SerializeField]
    private int _lightDisappearChance;

    private int _isExitIndex;
    private List<List<int>> _mobSpawnPoints = new List<List<int>>();
    private List<int> _userSpawnPoint = new List<int>();

    void Start()
    {
        _mazeGrid = new MazeCell[_mazeWidth, _mazeDepth];

        _isExitIndex = Random.Range(0, _mazeWidth - 1);

        for (int x = 0; x < _mazeWidth; x++)
        {
            for (int z = 0; z < _mazeDepth; z++)
            {
                int vectorX = x * _sizeMultiplier;
                int vectorZ = z * _sizeMultiplier;
                _mazeGrid[x, z] = Instantiate(_mazeCellPrefab, new Vector3(vectorX, 0, vectorZ), Quaternion.identity);
            }
        }

        GenerateMobPositions();
        GenerateMaze(null, _mazeGrid[0, 0]);
        SpawnUser();
    }

    private void GenerateMobPositions()
    {
        // Generate bot spawn points, push them to the list and decrement the mob count
        for (int i = 0; i < _mobCount; i++)
        {
            int x = Random.Range(0, _mazeWidth);
            int z = Random.Range(0, _mazeDepth);

            if (_mazeGrid[x, z].IsSpawnPointUsed)
            {
                i--;
                continue;
            }

            _mazeGrid[x, z].SetSpawnPointUsed();
            _mobSpawnPoints.Add(new List<int> { x, z });
        }
        // Generate user spawn point that is not in mob spawn points
        // x must be the last index and z must be random
        do
        {
            _userSpawnPoint = new List<int> { _mazeWidth - 1, Random.Range(0, _mazeDepth) };
        } while (_mobSpawnPoints.Any(spawnPoint => spawnPoint[0] == _userSpawnPoint[0] && spawnPoint[1] == _userSpawnPoint[1]));
    }

    private void SpawnUser()
    {
        Instantiate(_player, new Vector3(_userSpawnPoint[0] * _sizeMultiplier, 0, _userSpawnPoint[1] * _sizeMultiplier), Quaternion.identity);
        Instantiate(_stalker, new Vector3(_userSpawnPoint[0] * _sizeMultiplier, 0, _userSpawnPoint[1] * _sizeMultiplier), Quaternion.identity);
    }

    private void GenerateMaze(MazeCell previousCell, MazeCell currentCell)
    {
        int x = ((int)currentCell.transform.position.x) / _sizeMultiplier;
        int z = ((int)currentCell.transform.position.z) / _sizeMultiplier;

        // if the current position is in mob spawn points, remove it from the list and instantiate a mob prefab
        if (_mobSpawnPoints.Any(spawnPoint => spawnPoint[0] == x && spawnPoint[1] == z))
        {
            _mobSpawnPoints.RemoveAll(spawnPoint => spawnPoint[0] == x && spawnPoint[1] == z);
            Instantiate(_mobPrefab, new Vector3(x * _sizeMultiplier, 0, z * _sizeMultiplier), Quaternion.identity);
        }

        currentCell.Visit();
        ClearWalls(previousCell, currentCell);

        MazeCell nextCell;

        do
        {
            nextCell = GetNextUnvisitedCell(currentCell);

            if (nextCell != null)
            {
                GenerateMaze(currentCell, nextCell);
            }
        } while (nextCell != null);
    }

    private MazeCell GetNextUnvisitedCell(MazeCell currentCell)
    {
        var unvisitedCells = GetUnvisitedCells(currentCell);

        return unvisitedCells.OrderBy(_ => Random.Range(1, 10)).FirstOrDefault();
    }

    private IEnumerable<MazeCell> GetUnvisitedCells(MazeCell currentCell)
    {
        int x = ((int)currentCell.transform.position.x) / _sizeMultiplier;
        int z = ((int)currentCell.transform.position.z) / _sizeMultiplier;

        if (x + 1 < _mazeWidth)
        {
            var cellToRight = _mazeGrid[x + 1, z];

            if (cellToRight.IsVisited == false)
            {
                yield return cellToRight;
            }
        }

        if (x - 1 >= 0)
        {
            var cellToLeft = _mazeGrid[x - 1, z];

            if (cellToLeft.IsVisited == false)
            {
                yield return cellToLeft;
            }
        }

        if (z + 1 < _mazeDepth)
        {
            var cellToFront = _mazeGrid[x, z + 1];

            if (cellToFront.IsVisited == false)
            {
                yield return cellToFront;
            }
        }

        if (z - 1 >= 0)
        {
            var cellToBack = _mazeGrid[x, z - 1];

            if (cellToBack.IsVisited == false)
            {
                yield return cellToBack;
            }
        }
    }

    private void ClearWalls(MazeCell previousCell, MazeCell currentCell)
    {
        int x = ((int)currentCell.transform.position.x) / _sizeMultiplier;
        int z = ((int)currentCell.transform.position.z) / _sizeMultiplier;

        // Create an exit in the last cell 
        if (x == 0 && z == _isExitIndex)
        {
            currentCell.ClearLeftWall();
        }

        if (previousCell == null)
        {
            return;
        }

        // Remove light from the cell with a chance of _lightDisappearChance
        if (Random.Range(0, 100) <= _lightDisappearChance)
        {
            currentCell.DesactivateLight();
        }

        if (previousCell.transform.position.x < currentCell.transform.position.x)
        {
            previousCell.ClearRightWall();
            currentCell.ClearLeftWall();
            return;
        }

        if (previousCell.transform.position.x > currentCell.transform.position.x)
        {
            previousCell.ClearLeftWall();
            currentCell.ClearRightWall();
            return;
        }

        if (previousCell.transform.position.z < currentCell.transform.position.z)
        {
            previousCell.ClearFrontWall();
            currentCell.ClearBackWall();
            return;
        }

        if (previousCell.transform.position.z > currentCell.transform.position.z)
        {
            previousCell.ClearBackWall();
            currentCell.ClearFrontWall();
            return;
        }
    }

}
