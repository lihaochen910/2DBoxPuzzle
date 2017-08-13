using System;
using UnityEngine;

public sealed class ExecuteTimer{

    private static DateTime startTime;

    public static void OnExecutePrepare()
    {
        startTime = System.DateTime.Now;
    }

    public static void OnExecuteEnd()
    {
        DateTime endTime = System.DateTime.Now;
        TimeSpan ts = endTime.Subtract(startTime);
        Debug.LogWarning("代码执行时间:" + ts.TotalMilliseconds.ToString("0.00ms"));
    }
}
