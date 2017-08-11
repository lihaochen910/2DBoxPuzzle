using UnityEngine;

public class Box : MonoBehaviour
{
    /***
     * Box:
     *  (1)抛出消息/收到消息后处理什么
     *  (2)Box状态切换/检查
     *  (3)如何同步下落
     *  (4)怎么知道所有受影响的Box已经都收到消息
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

    
}
