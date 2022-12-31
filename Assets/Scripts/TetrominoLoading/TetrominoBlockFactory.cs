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
    private const string MaterialBlockColorVar = "_BlockColor";

    private static int _blockNamer = 0;

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
            _blockNamer++;
            block.name = "Block " + _blockNamer;
            BlockPool.Add(block);
        }
    }

    /// <summary>
    /// Gets from the pool, or instantiates a Tetronimo block at the specified position and color
    /// </summary>
    /// <param name="position"></param>
    /// <param name="size"></param>
    /// <param name="color"></param>
    /// <param name="parent"></param>
    /// <param name="shouldLerp"></param>
    /// <returns></returns>
    public GameObject GetOrInstantiateBlock(Vector3 position, Vector2 size, Color color, Transform parent, bool shouldLerp)
    {
        // Check if there are any blocks available in the pool
        if (BlockPool.Count > 0)
        {
            // Get the first available block from the pool
            GameObject block = BlockPool[0];
            BlockPool.RemoveAt(0);

            SetBlockProperties(block, position, size, color, parent, shouldLerp);

            // Activate the block and return it
            block.SetActive(true);


            Debug.Log("removing from pool: " + block.gameObject.name);
            return block;
        }
        else
        {
            // There are no blocks available in the pool, so instantiate a new block
            GameObject block = Instantiate(_blockPrefab);
            _blockNamer++;
            block.name = "Block " + _blockNamer;
            SetBlockProperties(block, position, size, color, parent, shouldLerp);


            Debug.Log("removing from pool: " + block.gameObject.name);
            return block;
        }
    }

    private void SetBlockProperties(GameObject block, Vector3 pos, Vector2 size, Color color, Transform parent, bool shouldLerp)
    {
        // Set the block's position and color
        block.transform.SetParent(parent);
        block.transform.localPosition = pos;
        block.transform.localScale = Vector3.one;
        block.transform.GetChild(0).GetComponent<Image>().color = color;
        block.GetComponent<RectTransform>().sizeDelta = size;
        block.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = size;
        block.GetComponent<TransformLerper>().enabled = shouldLerp;
    }

    /// <summary>
    /// Returns the specified Tetronimo block to the pool
    /// </summary>
    /// <param name="block"></param>
    public void ReturnBlockToPool(GameObject block)
    {
        block.SetActive(false);
        BlockPool.Add(block);

        Debug.Log("adding to pool: " + block.gameObject.name);
    }
}
