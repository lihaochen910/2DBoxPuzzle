using UnityEngine;
using System.Collections;

public sealed class MapMapper : MonoBehaviour
{
    private const string BoxTag = "Box";
    private GameObject[,] Map;
    void Awake()
    {
        Map = InitMap();
    }

    GameObject[,] InitMap()
    {
        var boxes = GameObject.FindGameObjectsWithTag(BoxTag);

        var map = new GameObject[99, 99];

        foreach (var box in boxes)
        {
            var x = box.transform.position.x;
            var y = box.transform.position.y;
            map[(int)x, (int)y] = box;
        }

        return map;
    }
    /* Scene视图 */
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
                Gizmos.DrawWireCube(new Vector3(i,j,0), Vector3.one);
            }
        }
    }
}
