using System.Collections.Generic;
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

    //the initial size of the pool, actual size of the pool will be increased as needed
    const int InitialPoolSize = 100;

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

        for (int i = 0; i < InitialPoolSize; i++)
        {
            GameObject block = Instantiate(_blockPrefab);
            block.transform.SetParent(transform);
            block.SetActive(false);
            BlockPool.Add(block);
        }
    }

    // Gets from the pool, or instantiates a Tetronimo block at the specified position and color
    public GameObject GetOrInstantiateBlock(Vector3 position, Vector2 size, Color color, Transform parent)
    {
        // Check if there are any blocks available in the pool
        if (BlockPool.Count > 0)
        {
            // Get the first available block from the pool
            GameObject block = BlockPool[0];
            BlockPool.RemoveAt(0);

            SetBlockProperties(block, position, size, color, parent);

            // Activate the block and return it
            block.SetActive(true);
            return block;
        }
        else
        {
            // There are no blocks available in the pool, so instantiate a new block
            GameObject block = Instantiate(_blockPrefab);
            SetBlockProperties(block, position, size, color, parent);
            return block;
        }
    }

    private void SetBlockProperties(GameObject block, Vector3 pos, Vector2 size, Color color, Transform parent)
    {
        // Set the block's position and color
        block.transform.SetParent(parent);
        block.transform.localPosition = pos;
        block.GetComponent<Image>().color = color;
        block.GetComponent<RectTransform>().sizeDelta = size;
    }

    // Returns the specified Tetronimo block to the pool
    public void ReturnBlockToPool(GameObject block)
    {
        block.SetActive(false);
        BlockPool.Add(block);
    }
}
