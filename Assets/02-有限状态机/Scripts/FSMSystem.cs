using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMSystem
{
    private Dictionary<状态, FSMState> 状态字典 = new Dictionary<状态, FSMState>();

    public 状态 当前状态;
    public FSMState 当前状态脚本;

    public void Update(GameObject AI)
    {
        当前状态脚本.自身状态行为(AI);
        当前状态脚本.判断转换条件(AI);
    }

    public void 添加状态脚本(FSMState 状态脚本)
    {
        if (状态脚本 == null)
        {
            Debug.LogError("状态脚本为空");
            return;
        }
        //把第一个添加进来的状态设置为当前状态
        if (当前状态脚本 == null)
        {
            当前状态脚本 = 状态脚本;
            当前状态 = 当前状态脚本.状态;
        }
        if (状态字典.ContainsKey(状态脚本.状态))
        {
            Debug.LogError("状态脚本已经存在");
            return;
        }
        状态字典.Add(状态脚本.状态, 状态脚本);
    }

    public void 删除状态(FSMState 状态脚本)
    {
        //基本不可能
    }

    public void 转换状态(转换条件 转换条件)
    {
        if (转换条件 == 转换条件.空)
        {
            Debug.LogError("转换条件为空");
            return;
        }
        状态 转换到状态 = 当前状态脚本.映射[转换条件];

        if (!状态字典.ContainsKey(转换到状态))
        {
            Debug.LogError("状态字典不存在 " + 转换到状态 + " 状态,无法转换");
            return;
        }

        FSMState 转换到状态脚本 = 状态字典[转换到状态];
        当前状态脚本.离开状态之后();

        当前状态脚本 = 转换到状态脚本;
        当前状态 = 转换到状态;

        当前状态脚本.进入状态之前();
    }
}
