using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class TankAI : MonoBehaviour
{
    private FSMSystem 状态机;

    void Start()
    {
        InitFSM();
    }

    void Update()
    {
        状态机.Update(this.gameObject);
    }

    void InitFSM()
    {
        状态机 = new FSMSystem();

        FSMState 巡逻脚本 = new Patrol(状态机);//此状态为添加的第一个状态,是默认状态
        巡逻脚本.添加转换信息(转换条件.看到玩家,状态.追击);

        FSMState 追击脚本 = new Chase(状态机);
        追击脚本.添加转换信息(转换条件.可以攻击,状态.攻击);
        追击脚本.添加转换信息(转换条件.丢失玩家,状态.巡逻);

        FSMState 攻击脚本 = new Attack(状态机);
        攻击脚本.添加转换信息(转换条件.攻击结束, 状态.追击);

        状态机.添加状态脚本(巡逻脚本);
        状态机.添加状态脚本(追击脚本);
        状态机.添加状态脚本(攻击脚本);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 18);
        Gizmos.DrawRay(transform.position, transform.TransformDirection(Vector3.forward * 18 * 0.866f + Vector3.left * 18 * 0.5f));
        Gizmos.DrawRay(transform.position, transform.TransformDirection(Vector3.forward * 18 * 0.866f + Vector3.right * 18 * 0.5f));
    }
}
