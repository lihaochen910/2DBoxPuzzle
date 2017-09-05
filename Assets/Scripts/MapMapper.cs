using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public sealed class MapMapper : MonoBehaviour
{
    private const string            BoxTag = "Box";
    private readonly Vector2        nilPosition = new Vector2(-1, -1);

    public static MapMapper         ins = null;

    //private Box[,]           Map = null;
    private Dictionary<Vector2, Box> Map = null;

    public Box this[int x,int y] {
        get
        {
            var key = new Vector2(x, y);
            if (Map.ContainsKey(key))
                return Map[key];
            else return null;
        }
        //private set {  }
    }
    public Box this[float x, float y]
    {
        get
        {
            var key = new Vector2(x, y);
            if (Map.ContainsKey(key))
                return Map[key];
            else return null;
        }
    }
    public Box this[Vector2 pos]
    {
        get
        {
            if (Map.ContainsKey(pos))
                return Map[pos];
            else return null;
        }
    }

    #region MonoBehaviour
    void Awake()
    {
        ins = this;
        Map = InitMap();
    }
    #endregion

    /* 初始化地图数组 */
    Dictionary<Vector2, Box> InitMap()
    {
        var boxes = GameObject.FindGameObjectsWithTag(BoxTag);

        var map = new Dictionary<Vector2, Box>();

        foreach (var box in boxes)
        {
            var x = box.transform.position.x;
            var y = box.transform.position.y;
            map[new Vector2(x,y)] = box.GetComponent<Box>();
        }

        return (from b in map
                orderby b.Key.y ascending
                select b).ToDictionary(k => k.Key, v => v.Value);
    }

    void SortMap()
    {
        Map = (from b in Map
            orderby b.Key.y ascending
            select b).ToDictionary(k => k.Key, v => v.Value);
    }
    /* 获取地图原数组 */
    public Dictionary<Vector2, Box> GetMap()
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
    [Obsolete]
    public bool InRange(Vector2 position)
    {
        //var x = position.x;
        //var y = position.y;

        //if (x > Map.GetLength(0) || x < 0)
        //    return false;
        //if (y > Map.GetLength(1) || y < 0)
        //    return false;

        return true;
    }
    /* 获取元素在地图中的索引 */
    public Vector2 GetBoxIndex(Box box)
    {
        //for(int x = 0;x < Map.GetLength(0); x++)
        //    for (int y = 0; y < Map.GetLength(1); y++)
        //        if (Map[x, y] == box)
        //        {
        //            return new Vector2(x, y);
        //        }
        //Debug.LogWarning(box.gameObject.name + " not found.");
        //return nilPosition;
        return (from b in Map where b.Value == box select b.Key).ToArray()[0];
    }
    /* 移动地图中的元素到指定坐标 */
    public void MoveTo(Box box,Vector2 destination)
    {
        var index = GetBoxIndex(box);
        //Map[destination] = box;
        if (Map.ContainsKey(destination))
        {
            Debug.LogWarning(box.gameObject.name + " source:" + index);
            Debug.LogError(destination + " ContainInMap:" + Map[destination].gameObject.name);
        }
        Map.Add(destination, box);
        Map.Remove(index);
        //Map[index] = null;
        SortMap();
    }

    #region DrawOnSceneView
    void OnDrawGizmos()
    {
        //DrawMapRange();
        //DrawBox();
    }
    void DrawMapRange()
    {
        //if (Map == null)
        //    return;
        //for (int i = 0; i < Map.GetLength(0); i++)
        //{
        //    for (int j = 0; j < Map.GetLength(1); j++)
        //    {
        //        if (Map[i, j] == null)
        //        {
        //            //Gizmos.color = Color.red;
        //            //Gizmos.DrawWireCube(new Vector3(i, j, 0), Vector3.one);
        //            continue;
        //        }
        //        Gizmos.color = Color.green;
        //        Gizmos.DrawWireCube(new Vector3(i, j, 0), Vector3.one);
        //    }
        //}
    }
    #endregion
}
