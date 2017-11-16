using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIButterfly : MonoBehaviour
{
    //引用
    public GameObject target;

    //组件
    private Rigidbody rigidbody;
    private Animator animator;

    //变量
    public Vector3 velocity;                                //速度
    public Vector3 sumForce = Vector3.zero;                 //三个力的和
    private Vector3 acceleratedVelocity;                     //加速度，由四个力的和所决定

    public Vector3 targetForce;                            //像目标方向的力
    private float targetWeight = 5;                        //目标方向的力的权重

    public Vector3 separationForce = Vector3.zero;         //分离的力
    private float separationWeight = 1;                     //分离的力的权重
    private float separationDis = 1f;                       //分离距离

    public Vector3 alignmentForce = Vector3.zero;          //队列的力
    private float alignmentWeight = 1;                      //队列的力的权重
    private float alignmentDis = 100f;                      //队列距离

    public Vector3 cohesionForce = Vector3.zero;           //聚合的力
    private float cohesionWeight = 1;                       //聚合的力的权重


    private List<GameObject> separationNeighbors = new List<GameObject>();//添加分离力的物体的集合
    private List<GameObject> alignmentNeighbors = new List<GameObject>();//添加队列力的物体的集合

    private float checkInterval = 0.2f;                 //检查的时间间隔，用invokerepeating而不在update中每帧调用

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();

        target = GameObject.Find("Target");

        Invoke("PlayAnimation",Random.Range(0,0.5f));
        InvokeRepeating("CalcForce", 0, checkInterval);
    }

    void PlayAnimation()
    {
        animator.Play("Fly");
    }


    void CalcForce()
    {
        sumForce = Vector3.zero;
        separationForce = Vector3.zero;
        alignmentForce = Vector3.zero;
        cohesionForce = Vector3.zero;
        targetForce = Vector3.zero;

        Collider[] colliders;

        #region 分离
        //获取分离碰撞提
        colliders = Physics.OverlapSphere(transform.position, separationDis);
        separationNeighbors.Clear();//每次计算分离力的时候，先清空分离列表，然后再把新的添加进去
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject != this.gameObject)
            {
                separationNeighbors.Add(collider.gameObject);
            }
        }

        //计算分离的力
        foreach (GameObject separationNeighbor in separationNeighbors)
        {
            Vector3 dir = transform.position - separationNeighbor.transform.position;
            separationForce += dir.normalized / dir.magnitude;//方向为反方向，距离越远力度越小
        }

        #endregion

        #region 队列
        //获取队列碰撞提
        colliders = Physics.OverlapSphere(transform.position, alignmentDis);
        alignmentNeighbors.Clear();//每次计算分离力的时候，先清空分离列表，然后再把新的添加进去
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject != this.gameObject)
            {
                alignmentNeighbors.Add(collider.gameObject);
            }
        }

        //计算队列的力
        if (alignmentNeighbors.Count != 0)
        {
            Vector3 alignmentAvgDir = Vector3.zero;//先归零
            foreach (GameObject alignmentNeighbor in alignmentNeighbors)
            {
                alignmentAvgDir += alignmentNeighbor.transform.forward;//把队列邻居中所有的前方向量相加（单位化）
            }
            alignmentAvgDir /= alignmentNeighbors.Count;//获取平均运动方向
            alignmentForce = alignmentAvgDir - transform.forward;//获取自己的偏移
        }

        #endregion

        #region 聚集

        if (separationNeighbors.Count == 0 && alignmentNeighbors.Count != 0)
        {
            Vector3 center = Vector3.zero;
            foreach (GameObject alignmentNeighbor in alignmentNeighbors)
            {
                center += alignmentNeighbor.transform.position; //把队列邻居中所有位置相加，然后除以个数就是中心位置
            }
            center /= alignmentNeighbors.Count; //获取到队列的中心位置
            cohesionForce = center - transform.position; //计算到自己到中心位置的偏移,就是力的大小
        }
        #endregion

        #region 目标

        targetForce = (target.transform.position - transform.position).normalized;   //像目标方向的力

        #endregion

        separationForce *= separationWeight;//最后分离的力乘以自己的权重
        alignmentForce *= alignmentWeight;//最后队列的力乘以自己的权重
        cohesionForce *= cohesionWeight;//最后聚集的力乘以自己的权重
        targetForce *= targetWeight;//最后飞向目标的力乘以自己的权重
        sumForce += separationForce;
        sumForce += alignmentForce;
        sumForce += cohesionForce;
        sumForce += targetForce;

        //Debug.DrawLine(transform.position, transform.position + sumForce);
        //if (transform.name == "Butterfly")
        //{
        //    //Debug.Log("分离的力=" + separationForce.magnitude);
        //    //Debug.Log("队列的力=" + alignmentForce.magnitude);
        //    //Debug.Log("聚合的力=" + cohesionForce.magnitude);
        //    //Debug.Log("目标的力=" + targetForce.magnitude);
        //    Debug.DrawLine(transform.position, transform.position + separationForce, Color.red, 0.2f);
        //    Debug.DrawLine(transform.position, transform.position + alignmentForce, Color.yellow, 0.2f);
        //    Debug.DrawLine(transform.position, transform.position + cohesionForce, Color.green, 0.2f);
        //    Debug.DrawLine(transform.position, transform.position + targetForce, Color.magenta, 0.2f);
        //}
    }

    void Update()
    {
        acceleratedVelocity = sumForce / rigidbody.mass / 10;

        velocity += acceleratedVelocity * Time.deltaTime * 10;//速度+加速度*时间=新速度

        if (velocity.sqrMagnitude >= 2)
        {
            velocity = velocity.normalized * 2;
        }

        //if (transform.name == "Butterfly")
        //{
        //    Debug.DrawLine(transform.position, transform.position + velocity, Color.cyan, Time.deltaTime);
        //}

        transform.rotation = Quaternion.LookRotation(velocity);

        rigidbody.velocity = velocity;
    }
}
