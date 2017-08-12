using UnityEngine;
using HighlightingSystem;
using System.Collections;

public class Box : MonoBehaviour
{
    /***
     * Box:
     *  (1)抛出消息/收到消息后处理什么
     *  (2)Box状态切换/检查
     *  (3)如何同步下落
     */
    #region Private variables
    private MapMapper _map;
    #endregion
    
    #region Public variables
    public BoxState State;
    #endregion

    public enum BoxState
    {
        Linked,
        Isolated,
    }

    #region MonoBehaviour
    void Start()
    {
        _map = MapMapper.ins;
        State = BoxState.Linked;
    }
    #endregion

    #region TempCode
    public void OnMoveEnd()
    {
        Send();
        print("end");
    }
    public void Send()
    {
        var map = _map.GetMap();
        var origin = _map.GetBoxIndex(this);

        var boxes = new Box[3];

        var start = -1;
        for (int i = 0; i < boxes.Length; i++)
        {
            boxes[i] = _map[origin.x + start, origin.y + 1];
            start++;
        }

        foreach (var box in boxes)
        {
            if (box)
                box.Recive();
        }
        

    }
    public void Recive()
    {
        Debug.LogWarning(gameObject.name + " OnRecive");
        Send();
        StartCoroutine(light());
    }
    IEnumerator light()
    {
        var hler = GetComponent<Highlighter>();
        if (hler == null)
        {
            hler = gameObject.AddComponent<Highlighter>();
        }
        hler.ConstantOn(Color.white);
        yield return new WaitForSeconds(0.7f);
        hler.ConstantOff();
    }
    #endregion
}
