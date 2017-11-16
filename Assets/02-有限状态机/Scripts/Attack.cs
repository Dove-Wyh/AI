using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : FSMState
{
    private bool attacked;
    private GameObject shell;

    public Attack(FSMSystem 归属管理器) : base(归属管理器)
    {
        shell = Resources.Load<GameObject>("Shell");
        attacked = false;
    }

    public override void 自身状态行为(GameObject AI)
    {
        攻击();
    }

    public override void 判断转换条件(GameObject AI)
    {
        继续攻击();
    }

    public void 攻击()
    {
        GameObject.Instantiate(shell);
        attacked = true;
    }

    public void 继续攻击()
    {
        if (attacked)
        {
            状态机.转换状态(转换条件.攻击结束);
        }
    }
}
