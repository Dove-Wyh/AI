using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol : FSMState
{
    private Transform[] 巡逻点;
    private Rigidbody rigidbody;
    private int index;
    private Vector3 target;
    private Vector3 offset;
    private float patrolSpeed;
    private float radius;
    private float stopDis;
    private float angle;

    public Patrol(FSMSystem 归属管理器) : base(归属管理器)
    {
        状态 = 状态.巡逻;
        //包含父节点的坐标,不过这里不影响,所以不做处理
        巡逻点 = GameObject.Find("WayPoint").GetComponentsInChildren<Transform>();
        patrolSpeed = 10;
        radius = 18;
        index = 0;
        stopDis = 0.1f;
    }

    public override void 自身状态行为(GameObject AI)
    {
        自动巡逻(AI);
    }

    public override void 判断转换条件(GameObject AI)
    {
        看到目标(AI);
    }

    public void 自动巡逻(GameObject AI)
    {
        Debug.Log("巡逻状态");
        rigidbody = AI.GetComponent<Rigidbody>();
        
        target = 巡逻点[index].position;
        offset = target - AI.transform.position;
        offset.y = 0;
        //Debug.Log(offset.y);
        angle = Vector3.Angle(offset, AI.transform.forward);
        if (Vector3.SqrMagnitude(offset) > stopDis)     //判断自身位置与目标位置距离是否小于停止距离
        {
            float minAngle = Mathf.Min(angle, 100 * Time.deltaTime);//防止旋转角度过小
            //Vector3.Cross(a,b)代表a向量与b向量的×积，垂直于a和b，并且决定了旋转的方向                                                     //围绕垂直的轴没帧旋转最小的角度，旋转角度固定
            AI.transform.Rotate(Vector3.Cross(AI.transform.forward, offset), minAngle);
            //每帧向前移动
            AI.transform.position += AI.transform.forward * Time.deltaTime * patrolSpeed;
            //rigidbody.velocity += AI.transform.forward * Time.deltaTime * patrolSpeed;
        }
        else
        {
            index++;
            index %= 巡逻点.Length;
        }
    }

    public void 看到目标(GameObject AI)
    {
        Collider[] enemys;
        enemys = Physics.OverlapSphere(AI.transform.position, radius);
        //Debug.Log(enemys.Length);
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
                    if (hit.transform != null && hit.transform.tag == "Player")
                    { 
                        状态机.转换状态(转换条件.看到玩家);
                    }
                }
            }
        }
    }
}
