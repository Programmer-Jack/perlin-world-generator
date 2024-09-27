using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    [SerializeField] int _width, _height;
    [SerializeField] float _xOrigin, _yOrigin;
    [SerializeField] Vector2 _xOffsetVariance;
    [SerializeField] Vector2 _yOffsetVariance;
    [SerializeField] float _scale;

    [SerializeField] float _thresholdMultiplier;
    float _threshold;
    bool _wasBlockPlaced = false;

    Dictionary<Vector3Int, GameObject> _blockDatabase = new();
    Dictionary<Vector3Int, GameObject> _blockDatabaseDuplicate;

    [SerializeField] GameObject _blockContainerPrefab;
    Transform _blockContainer;

    [SerializeField] GameObject _grass, _stone, _gold, _dirt, _water, _sand;
    [SerializeField] int _beachSize;
    [SerializeField] int _seaLevel;
    [SerializeField] int _lastDirtChance;
    [SerializeField] int _goldHeightLimit, _goldChance, _extendedGoldChance;

    [ExecuteInEditMode]

    bool WeightedCoinFlip(int probabilityPercent) => Random.Range(1, 101) <= probabilityPercent;

    public void GenerateYXZ()
    {
        if (_blockContainer)
        {
            DestroyImmediate(_blockContainer.gameObject);
            _blockContainer = null;
        }

        _blockContainer = Instantiate(_blockContainerPrefab, new Vector3(0, _height, 0), Quaternion.identity).transform;

        /*
         * Iteration 1
         * 
         * World generation and initial block assignment
         */

        float z = 0f;

        while (z < _width)
        {
            float x = 0f;

            while (x < _width)
            {
                float y = _height;

                while (y > 0)
                {
                    float xCoord = _xOrigin + x / _width * _scale;
                    float yCoord = _yOrigin + y / _width * _scale;
                    float pointValue = Mathf.PerlinNoise(xCoord, yCoord);
                    _threshold = Mathf.Clamp01(y / (float)_height * _thresholdMultiplier);

                    if (pointValue > _threshold)
                    {
                        GameObject _blockToPlace;

                        if (_wasBlockPlaced)
                        {
                            if (y <= _goldHeightLimit && WeightedCoinFlip(_goldChance))
                            {
                                _blockToPlace = _gold;
                            }
                            else
                            {
                                _blockToPlace = _stone;
                            }
                        }
                        else
                        {
                            _blockToPlace = _grass;
                        }

                        _blockDatabase.Add(Vector3Int.RoundToInt(new Vector3(x, y, z)), _blockToPlace);

                        _wasBlockPlaced = true;
                    }
                    else
                    {
                        _wasBlockPlaced = false;

                        if (y <= _seaLevel)
                        {
                            _blockDatabase.Add(Vector3Int.RoundToInt(new Vector3(x, y, z)), _water);
                        }
                    }

                    y--;
                }
                _wasBlockPlaced = false;
                x++;
            }
            float randomXOffset = Random.Range(_xOffsetVariance.x, _xOffsetVariance.y) * _scale;
            float randomYOffset = Random.Range(_yOffsetVariance.x, _yOffsetVariance.y) * _scale;

            _xOrigin += randomXOffset;
            _yOrigin += randomYOffset;

            _wasBlockPlaced = false;
            z++;
        }

        /*
         * Iteration 2
         * 
         * Ore extension, block replacement
         */

        _blockDatabaseDuplicate = new Dictionary<Vector3Int, GameObject>(_blockDatabase);

        foreach (var blockData in _blockDatabase)
        {
            if (blockData.Value.Equals(_gold))
            {
                if (WeightedCoinFlip(_extendedGoldChance))
                {
                    Vector3Int[] adjacentCoords = new Vector3Int[]
                    {
                        Vector3Int.forward,
                        Vector3Int.back,
                        Vector3Int.left,
                        Vector3Int.right,
                        Vector3Int.up,
                        Vector3Int.down
                    };

                    foreach (var coord in adjacentCoords)
                    {
                        if (_blockDatabaseDuplicate.ContainsKey(blockData.Key + coord) && _blockDatabaseDuplicate[blockData.Key + coord] != _grass && WeightedCoinFlip(66))
                        {
                            _blockDatabaseDuplicate[blockData.Key + coord] = _gold;
                        }
                    }
                }
            }

            if (blockData.Value.Equals(_grass))
            {
                int[] stepsBelow = new int[]
                {
                    1,
                    2
                };

                foreach (var step in stepsBelow)
                {
                    Vector3Int stepCoord = new(0, step, 0);

                    if (_blockDatabaseDuplicate.ContainsKey(blockData.Key - stepCoord))
                        _blockDatabaseDuplicate[blockData.Key - stepCoord] = _dirt;
                }

                if (_blockDatabaseDuplicate.ContainsKey(blockData.Key - new Vector3Int(0, 3, 0)) && WeightedCoinFlip(_lastDirtChance))
                {
                    _blockDatabaseDuplicate[blockData.Key - new Vector3Int(0, 3, 0)] = _dirt;
                }
            }
        }

        /*
         * Iteration 3
         * 
         * Block placement
         */

        foreach (var blockData in _blockDatabaseDuplicate)
        {
            GameObject instance = Instantiate(blockData.Value, blockData.Key, Quaternion.identity, _blockContainer);
            instance.isStatic = true;
        }

        _blockDatabase.Clear();
        _blockDatabaseDuplicate.Clear();
    }

    public void GenerateWorld()
    {
        if (_blockContainer)
        {
            DestroyImmediate(_blockContainer.gameObject);
            _blockContainer = null;
        }

        _blockContainer = Instantiate(_blockContainerPrefab, new Vector3(0, _height, 0), Quaternion.identity).transform;

        /*
         * Iteration 1
         * 
         * World generation and initial block assignment
         */

        float y = _height;

        while (y > 0)
        {
            float z = 0f;

            while (z < _width)
            {
                float x = 0f;

                while (x < _width)
                {
                    float xCoord = _xOrigin + x / _width * _scale;
                    float yCoord = _yOrigin + z / _width * _scale;
                    float pointValue = Mathf.PerlinNoise(xCoord, yCoord);
                    _threshold = Mathf.Clamp01(y / (float)_height * _thresholdMultiplier);
                    GameObject blockToAdd = null;

                    if (pointValue > _threshold)
                    {
                        blockToAdd = WeightedCoinFlip(_goldChance) ? _gold : _stone;
                    }
                    else
                    {
                        if (y <= _seaLevel)
                        {
                            blockToAdd = _water;
                        }
                    }

                    if (blockToAdd)
                    {
                        _blockDatabase.Add(Vector3Int.RoundToInt(new Vector3(x, y, z)), blockToAdd);
                    }
                    
                    x++;
                }
                z++;
            }
            float randomXOffset = Random.Range(_xOffsetVariance.x, _xOffsetVariance.y) * _scale;
            float randomYOffset = Random.Range(_yOffsetVariance.x, _yOffsetVariance.y) * _scale;

            _xOrigin += randomXOffset;
            _yOrigin += randomYOffset;
            y--;
        }

        _blockDatabaseDuplicate = new Dictionary<Vector3Int, GameObject>(_blockDatabase);

        /*
         * Iteration 2
         * 
         * Grass, dirt, sand creation
         */

        foreach (var blockData in _blockDatabase)
        {
            if (blockData.Value != _water)
            {
                if (!_blockDatabase.ContainsKey(blockData.Key + Vector3Int.up))
                {
                    _blockDatabaseDuplicate[blockData.Key] = _grass;

                    for (int i = 1; i < 3; i++)
                    {
                        Vector3Int stepCoord = new(0, i, 0);

                        if (_blockDatabaseDuplicate.ContainsKey(blockData.Key - stepCoord))
                            _blockDatabaseDuplicate[blockData.Key - stepCoord] = _dirt;
                    }

                    if (_blockDatabaseDuplicate.ContainsKey(blockData.Key - new Vector3Int(0, 3, 0)) && WeightedCoinFlip(_lastDirtChance))
                    {
                        _blockDatabaseDuplicate[blockData.Key - new Vector3Int(0, 3, 0)] = _dirt;
                    }
                }
            }
            else
            {
                Vector3Int keyDown = blockData.Key + Vector3Int.down;

                if (_blockDatabase.ContainsKey(keyDown))
                {
                    if (_blockDatabaseDuplicate[keyDown] != _water)
                    {
                        _blockDatabaseDuplicate[keyDown] = _sand;
                    }
                }
            }
        }

        /*
         * Iteration 3
         * 
         * Gold expansion
         */

        foreach (var blockData in _blockDatabase)
        {
            if (_blockDatabaseDuplicate[blockData.Key].Equals(_grass))
            {
                bool createBeach = false;

                Vector3Int[] adjacentCoords = new Vector3Int[]
                {
                    Vector3Int.right,
                    Vector3Int.left,
                    Vector3Int.forward,
                    Vector3Int.back
                };

                foreach (var coord in adjacentCoords)
                {
                    for (int i = 1; i <= _beachSize; i++)
                    {
                        if (_blockDatabaseDuplicate.ContainsKey(blockData.Key + coord * i) && _blockDatabaseDuplicate[blockData.Key + coord * i].Equals(_water))
                        {
                            createBeach = true;
                            break;
                        }
                    }
                }

                if (createBeach)
                {
                    _blockDatabaseDuplicate[blockData.Key] = _sand;
                }
            }
            else if (blockData.Value.Equals(_gold))
            {
                if (WeightedCoinFlip(_extendedGoldChance))
                {
                    Vector3Int[] adjacentCoords = new Vector3Int[]
                    {
                        Vector3Int.forward,
                        Vector3Int.back,
                        Vector3Int.left,
                        Vector3Int.right,
                        Vector3Int.up,
                        Vector3Int.down
                    };

                    foreach (var coord in adjacentCoords)
                    {
                        if (_blockDatabaseDuplicate.ContainsKey(blockData.Key + coord) &&
                            _blockDatabaseDuplicate[blockData.Key + coord] != _grass &&
                            _blockDatabaseDuplicate[blockData.Key + coord] != _water &&
                            WeightedCoinFlip(66))
                        {
                            _blockDatabaseDuplicate[blockData.Key + coord] = _gold;
                        }
                    }
                }
            }
        }

        /*
         * Iteration 4
         * 
         * Block placement
         */

        foreach (var blockData in _blockDatabaseDuplicate)
        {
            GameObject instance = Instantiate(blockData.Value, blockData.Key, Quaternion.identity, _blockContainer);
            instance.isStatic = true;
        }

        _blockDatabase.Clear();
        _blockDatabaseDuplicate.Clear();
    }
}
