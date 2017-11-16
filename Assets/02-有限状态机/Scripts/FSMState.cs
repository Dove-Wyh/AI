using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum 转换条件
{
    空 = 0,
    看到玩家 = 1,
    丢失玩家 = 2,
    受到攻击 = 3,
    生命值为空 = 4,
    可以攻击 = 5,
    攻击结束 = 6,
}

public enum 状态
{
    空 = 0,
    巡逻 = 1,
    追击 = 2,
    死亡 = 3,
    闲置 = 4,
    攻击 = 5,
}

public abstract class FSMState
{
    public 状态 状态;
    public Dictionary<转换条件,状态> 映射 = new Dictionary<转换条件, 状态>();
    public FSMSystem 状态机;//知道归属谁管理

    public FSMState(FSMSystem 归属管理器)
    {
        this.状态机 = 归属管理器;
    }

    public void 添加转换信息(转换条件 转换条件, 状态 转换到状态)
    {
        if (转换条件 == 转换条件.空)
        {
            Debug.LogError("添加的转换条件为空."+"转换条件:"+ 转换条件 + " " + "转换到:" + 转换到状态 + "状态");
            return;
        }
        if (转换到状态 == 状态.空)
        {
            Debug.LogError("添加的转换到状态为空" + "转换条件:" + 转换条件 + " " + "转换到:" + 转换到状态 + "状态");
            return;
        }
        if (映射.ContainsKey(转换条件))
        {
            Debug.LogError("添加的转换条件已经存在" + "转换条件:" + 转换条件 + " " + "已存在转换到:" + 转换到状态 + "状态" + " " + "无法继续添加转换到:" + 转换到状态 + "状态");
            return;
        }
        映射.Add(转换条件, 转换到状态);
    }

    public void 删除转换信息(转换条件 转换条件)
    {
        if (转换条件 == 转换条件.空)
        {
            Debug.LogError("删除的转换条件为空." + "转换条件:" + 转换条件);
            return;
        }
        if (!映射.ContainsKey(转换条件))
        {
            Debug.LogError("删除的转换条件不存在" + "转换条件:" + 转换条件);
            return;
        }
        映射.Remove(转换条件);
    }

    public virtual void 进入状态之前() { }
    public virtual void 离开状态之后() { }
    public abstract void 自身状态行为(GameObject AI);
    public abstract void 判断转换条件(GameObject AI);
}
