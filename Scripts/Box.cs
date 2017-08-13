using UnityEngine;
using HighlightingSystem;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
    private readonly Vector2 _nilPosition = new Vector2(-1, -1);
    //记录被推动后的坐标,用于被推动的源物体
    private Vector2 _moveEndPosition;
    #endregion

    #region Public variables
    //Box实例当前状态
    public BoxState State;
    //当Box处于Isolated状态时预计算出的下落停靠目标点
    //Linked状态时应该置于(-1,-1)
    private Vector2 _linkedPosition = new Vector2(-1, -1);
    #endregion

    #region Shared variables
    /// <summary>
    /// 所有Box实例共享的下落速度
    /// </summary>
    protected static float DropSpeed = 2f;
    /// <summary>
    /// 所有Box实例共享的队列,用于从源物体推送同步下落事件
    /// </summary>
    protected static Queue<Box> To_be_synchronized_object = new Queue<Box>();
    /// <summary>
    /// 用于检索受影响的box,所有box实例共享此队列以节省内存
    /// </summary>
    protected static Queue<Box> Affected_Boxes = new Queue<Box>();

    protected static int ReciveCount = 0;
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
    //被推动动画结束后执行回调
    public void OnMoveEnd(Vector2 moveEndPosition)
    {
        ExecuteTimer.OnExecutePrepare();
        //保存被推动后的坐标
        _moveEndPosition = moveEndPosition;
        //递归检查
        SearchAffectedBoxes();
        //递归检查结束,所有box状态确定,下落坐标确定,可以下落
        pushDropEvent();

        ExecuteTimer.OnExecuteEnd();

        print("sendCount =" + ReciveCount);
        ReciveCount = 0;
    }

    /* 检查自身状态 */
    private void CheckSelf()
    {
        State = getCurrentState();
        if (State == BoxState.Isolated)
        {
            _linkedPosition = calcDropPosition();
            To_be_synchronized_object.Enqueue(this);
        }
    }
    /* 检索所有受影响box，入队 */
    private void SearchAffectedBoxes()
    {
        foreach (var box in GetAffectedBoxes(_map.GetBoxIndex(this)))
        {
            if (box)
                Affected_Boxes.Enqueue(box);
        }
        //原坐标保留后,使用被推动后的坐标更新此实例在数组中的位置
        DoMoveToInArray(_moveEndPosition);
        CheckSelf();

        while (Affected_Boxes.Count != 0)
        {
            var box = Affected_Boxes.Dequeue();

            box.Recive();

            foreach (var sub in GetAffectedBoxes(_map.GetBoxIndex(box)))
            {
                if (sub && !Affected_Boxes.Contains(sub))
                    Affected_Boxes.Enqueue(sub);
            }
        }
    }
    /* 受目标点影响的box */
    public IEnumerable<Box> GetAffectedBoxes(Vector2 originPosition)
    {
        var start = -1;
        for (int i = 0; i < 3; i++)
        {
            yield return _map[originPosition.x + start, originPosition.y + 1];
            start++;
        }
    }
    /* 检查此Box实例的当前状态 */
    private BoxState getCurrentState()
    {
        var currentPositionInArray = new Vector2(transform.position.x, transform.position.y);
            //_map.GetBoxIndex(this);

        var boxes = new Box[3];

        var start = -1;
        for (int i = 0; i < boxes.Length; i++)
        {
            boxes[i] = _map[currentPositionInArray.x + start, currentPositionInArray.y - 1];
            start++;
        }

        bool hasSupport = false;
        int supportCount = 0;
        foreach (var box in boxes)
        {
            if (box && box.State == BoxState.Linked)
                supportCount++;
        }

        if (supportCount > 0)
            return BoxState.Linked;
        else return BoxState.Isolated;
    }
    /* 当box处于孤立状态时(预下落状态),计算下落坐标 */
    private Vector2 calcDropPosition()
    {
        var currentPositionInArray = new Vector2(transform.position.x, transform.position.y);
        //_map.GetBoxIndex(this);
        var currentX = (int)currentPositionInArray.x;
        var currentY = (int)currentPositionInArray.y;
        //当前box实例在地图边界
        //if (currentY - 1 < 0)
        //    return new Vector2(currentX, -99);

        var hitX = new int[] { currentX - 1,currentX, currentX + 1};
        //从当前行倒序遍历地图数组(世界方向向下)
        //for (int y = currentY - 1; y >= 0; y--)
        //{
        //    for (int x = 0; x < _map.GetMap().GetLength(0); x++)
        //    {
        //        if (IntArrayContainElement(hitX, x))
        //        {
        //            if (_map[x, y])
        //            {
        //                if (_map[x, y].State == BoxState.Isolated)
        //                {
        //                    return new Vector2(currentX, _map[x, y]._linkedPosition.y + 1);
        //                }
        //                else
        //                {
        //                    return new Vector2(currentX, y + 1);
        //                }
        //            }
        //        }
        //    }
        //}
        /* TODO: 解决源物体以外下落坐标不正确的问题 */
        foreach (var box in _map.GetMap().Reverse())
        {
            //print(gameObject.name + " check " + box.Key + " " + box.Value.gameObject.name);
            if (box.Key.y < currentY && IntArrayContainElement(hitX, (int)box.Key.x))
            {
                //Debug.LogWarning(box.Value.gameObject.name+" checkInRange " + box.Value.State + " org:" + box.Key + " linked:" + box.Value._linkedPosition);
                if (box.Value.State == BoxState.Isolated)
                {
                    //Debug.LogWarning(gameObject.name + " use " + box.Value.gameObject.name + " _linkedPosition.y:" + box.Value._linkedPosition.y);
                    return new Vector2(currentX, box.Value._linkedPosition.y + 1);
                }
                else
                {
                    //Debug.LogWarning(gameObject.name + " use " + box.Value.gameObject.name + " .y:" + box.Key.y);
                    return new Vector2(currentX, box.Key.y + 1);
                }
            }
        }
        //如果没有找到停靠点，则返回地图外最下方的坐标
        return new Vector2(currentX, -99);
    }
    /* 工具方法,检查静态int数组中是否包括指定int元素 */
    private bool IntArrayContainElement(int[] array, int v)
    {
        foreach(var arrayValue in array)
            if (arrayValue == v)
                return true;
        return false;
    }
    /* 推送同步下落事件 */
    private void pushDropEvent()
    {
        while (To_be_synchronized_object.Count != 0)
            To_be_synchronized_object.Dequeue().DoDropTween();
    }
    /* 执行下落动画 */
    private void DoDropTween()
    {
        //print(gameObject.name+" DoDropTween 2pos:"+_linkedPosition);
        iTween.MoveTo(gameObject, 
            iTween.Hash("position", new Vector3(_linkedPosition.x, _linkedPosition.y,gameObject.transform.position.z), "easeType", "linear",
            "speed", DropSpeed, "oncomplete", "DoMoveToInArray_And_SwitchState", "oncompleteparams", _linkedPosition));
    }
    /* 下落动画完成后移动box在数组中的位置 */
    private void DoMoveToInArray_And_SwitchState(Vector2 position)
    {
        _map.MoveTo(this, position);
        State = BoxState.Linked;
    }
    private void DoMoveToInArray(Vector2 position)
    {
        _map.MoveTo(this, position);
    }
    /* 受影响时被调用 */
    protected void Recive()
    {
        ReciveCount ++;

        CheckSelf();

        StartCoroutine(DebugLight());
    }
    IEnumerator DebugLight()
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
