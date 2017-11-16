using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Chase : FSMState
{
    private Rigidbody rigidbody;
    private Transform 目标;
    private Vector3 target;
    private Vector3 offset;
    private float timer;
    private float attackTime;
    private float chaseSpeed;
    private float radius;
    private float stopDis;
    private float attackDis;
    private float angle;

    public Chase(FSMSystem 归属管理器) : base(归属管理器)
    {
        状态 = 状态.追击;
        
        目标 = GameObject.FindGameObjectWithTag("Player").transform;
        attackTime = 1.5f;
        attackDis = 1;
        timer = 1.5f;
        chaseSpeed = 15;
        radius = 36;
        stopDis = 150;
    }

    public override void 自身状态行为(GameObject AI)
    {
        追击(AI);
    }

    public override void 判断转换条件(GameObject AI)
    {
        丢失目标(AI);
        到达攻击范围(AI);
    }

    public void 追击(GameObject AI)
    {
        Debug.Log("追击状态");
        rigidbody = AI.GetComponent<Rigidbody>();
        target = 目标.transform.position;
        offset = target - AI.transform.position;
        offset.y = 0;
        angle = Vector3.Angle(offset, AI.transform.forward);
        
        float minAngle = Mathf.Min(angle, 100 * Time.deltaTime);//防止旋转角度过小
        //Vector3.Cross(a,b)代表a向量与b向量的×积，垂直于a和b，并且决定了旋转的方向                                                     
        //围绕垂直的轴没帧旋转最小的角度，旋转角度固定
        Vector3 axis = Vector3.Cross(AI.transform.forward, offset);
        
        axis.x = axis.z = 0;
        AI.transform.Rotate(axis, minAngle);

        if (timer < attackTime)
        {
            timer += Time.deltaTime;
        }

        if (Vector3.SqrMagnitude(offset) > stopDis)     //判断自身位置与目标位置距离是否小于停止距离
        {
            //没帧朝着前方移动,移动速度固定
            rigidbody.velocity += AI.transform.forward * Time.deltaTime * chaseSpeed;
            
        }
        else if (Vector3.SqrMagnitude(offset) < stopDis) //如果跑到了攻击范围内
        {
            //没帧朝着后方移动,移动速度固定
            rigidbody.velocity -= AI.transform.forward * Time.deltaTime * chaseSpeed;
        }

        //Debug.Log(rigidbody.velocity + " " + rigidbody.velocity.magnitude);
    }

    public void 丢失目标(GameObject AI)
    {
        //判断是否看到了玩家
        Collider[] enemys;
        enemys = Physics.OverlapSphere(AI.transform.position, radius);

        foreach (var enemy in enemys)
        {
            if (enemy.tag == "Player")
            {
                float angle = Vector3.Angle(AI.transform.forward, enemy.transform.position - AI.transform.position);
                if (angle < 35)
                {
                    Ray ray = new Ray(AI.transform.position + AI.transform.up * 0.1f, enemy.transform.position - AI.transform.position);
                    RaycastHit hit;
                    Physics.Raycast(ray, out hit);
                    if (hit.transform == null || hit.transform.tag != "Player")
                    {
                        状态机.转换状态(转换条件.丢失玩家);
                    }
                }
                else
                {
                    状态机.转换状态(转换条件.丢失玩家);
                }
            }
        }
    }

    public void 到达攻击范围(GameObject AI)
    {
        if (Mathf.Abs(Vector3.SqrMagnitude(offset) - stopDis) < attackDis && timer >= attackTime)
        {
            timer = 0;
            状态机.转换状态(转换条件.可以攻击);
            //Debug.Log("AI发射炮弹");
        }
    }
}
