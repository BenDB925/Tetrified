using System.Collections.Generic;
using Tetrified.Scripts.Gameplay;
using Tetrified.Scripts.Utility;
using UnityEngine;
using UnityEngine.UI;

public class TetrominoBlockFactory : Singleton<TetrominoBlockFactory>
{
    // The prefab for the Tetronimo blocks
    [SerializeField]
    private GameObject _blockPrefab;

    // The pool of Tetronimo blocks
    private List<GameObject> _blockPool;

    //the percentage of the grid that will be initially instantiated. ie. 0.4 means 40% of all potential blocks will be created
    const float _percentageOfGridToInitiallyPool = 0.4f;

    public List<GameObject> BlockPool
    {
        get
        {
            if (_blockPool == null)
            {
                InitPool();
            }
            return _blockPool;
        }
        set => _blockPool = value;
    }

    protected override void Awake()
    {
        base.Awake();
        InitPool();
    }

    private void InitPool()
    {
        // Initialize the block pool
        BlockPool = new List<GameObject>();

        int totalNumberOfBlocks = TetrisGridData.Instance._width * TetrisGridData.Instance._height;
        int blockPoolSize = Mathf.CeilToInt(totalNumberOfBlocks * _percentageOfGridToInitiallyPool);

        for (int i = 0; i < blockPoolSize; i++)
        {
            GameObject block = Instantiate(_blockPrefab);
            block.transform.SetParent(transform);
            block.SetActive(false);
            BlockPool.Add(block);
        }
    }

    // Gets from the pool, or instantiates a Tetronimo block at the specified position and color
    public GameObject GetOrInstantiateBlock(Vector3 position, Vector2 size, Color color)
    {
        // Check if there are any blocks available in the pool
        if (BlockPool.Count > 0)
        {
            // Get the first available block from the pool
            GameObject block = BlockPool[0];
            BlockPool.RemoveAt(0);

            SetBlockProperties(block, position, size, color);

            // Activate the block and return it
            block.SetActive(true);
            return block;
        }
        else
        {
            // There are no blocks available in the pool, so instantiate a new block
            GameObject block = Instantiate(_blockPrefab);
            SetBlockProperties(block, position, size, color);
            block.transform.SetParent(transform);
            return block;
        }
    }

    private void SetBlockProperties(GameObject block, Vector3 pos, Vector2 size, Color color)
    {
        // Set the block's position and color
        block.transform.position = pos;
        block.GetComponent<Image>().material.color = color;
        block.GetComponent<RectTransform>().sizeDelta = size;
    }

    // Returns the specified Tetronimo block to the pool
    public void ReturnBlockToPool(GameObject block)
    {
        block.SetActive(false);
        BlockPool.Add(block);
    }
}
