using UnityEngine;
using System.Collections;

public sealed class MapMapper : MonoBehaviour
{
    private const string            BoxTag = "Box";
    private readonly Vector2        nilPosition = new Vector2(-1, -1);

    public static MapMapper         ins = null;

    private Box[,]           Map = null;

    public Box this[int x,int y] {
        get { return Map[x, y]; }
        //private set {  }
    }
    public Box this[float x, float y]
    {
        get { return Map[(int)x, (int)y]; }
    }

    #region MonoBehaviour
    void Awake()
    {
        ins = this;
        Map = InitMap();
    }
    #endregion

    /* 初始化地图数组 */
    Box[,] InitMap()
    {
        var boxes = GameObject.FindGameObjectsWithTag(BoxTag);

        var map = new Box[30, 30];

        foreach (var box in boxes)
        {
            var x = box.transform.position.x;
            var y = box.transform.position.y;
            map[(int)x, (int)y] = box.GetComponent<Box>();
        }

        return map;
    }
    /* 获取地图原数组 */
    public Box[,] GetMap()
    {
        return Map;
    }
    /* 指定位置是否存在Box */
    public bool IsExist(Vector2 position)
    {
        if (this[position.x, position.y])
            return true;
        else return false;
    }
    /* 索引是否在地图范围中 */
    public bool InRange(Vector2 position)
    {
        var x = position.x;
        var y = position.y;

        if (x > Map.GetLength(0))
            return false;
        if (y > Map.GetLength(1))
            return false;

        return true;
    }
    /* 获取元素在地图中的索引 */
    public Vector2 GetBoxIndex(Box box)
    {
        for(int x = 0;x < Map.GetLength(0); x++)
            for (int y = 0; y < Map.GetLength(1); y++)
                if (Map[x, y] == box)
                    return new Vector2(x, y);
        return nilPosition;
    }
    /* 移动地图中的元素 */
    public void MoveTo(Box box,Vector2 destination)
    {
        if (InRange(destination))
        {
            var index = GetBoxIndex(box);
            print("destination:" + destination);
            Map[(int)destination.x, (int)destination.y] = box;
            Map[(int)index.x, (int)index.y] = null;
        }
    }

    #region DrawOnSceneView
    void OnDrawGizmos()
    {
        DrawMapRange();
        DrawBox();
    }
    void DrawBox()
    {
        if (Map == null)
            return;
        for (int i = 0; i < Map.GetLength(0); i++)
        {
            for (int j = 0; j < Map.GetLength(1); j++)
            {
                if (!Map[i, j])
                    continue;
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(Map[i, j].transform.position, Vector3.one);
            }
        }
    }
    void DrawMapRange()
    {
        if (Map == null)
            return;
        for (int i = 0; i < Map.GetLength(0); i++)
        {
            for (int j = 0; j < Map.GetLength(1); j++)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(new Vector3(i, j, 0), Vector3.one);
            }
        }
    }
    #endregion
}
