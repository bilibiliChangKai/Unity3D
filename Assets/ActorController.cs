/*
 * 描 述：挂在在Actor上的脚本，用于控制主角移动
 * 作 者：hza 
 * 创建时间：2017/04/14 00:10:36
 * 版 本：v 1.0
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]

public class ActorController : MonoBehaviour {

    private Animator ani;
    private AnimatorStateInfo currentBaseState;
    private Rigidbody rig;

    private Vector3 velocity;
    // 我实在不知道怎么用力学表示匀速，不如用运动学

    private float rotateSpeed = 15f;
    private float runSpeed = 5f;
    // 旋转速度，奔跑速度

    // Use this for initialization
    void Start () {
        ani = GetComponent<Animator>();
        rig = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate () {
        if (!ani.GetBool("isLive")) return;
        // 如果死亡，不执行所有动作

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        ani.SetFloat("Speed", Mathf.Max(Mathf.Abs(x), Mathf.Abs(z)));
        // 设置速度
        ani.speed = 1 + ani.GetFloat("Speed") / 3;
        // 调整跑步的时候的动画速度

        velocity = new Vector3(x, 0, z);

        // 如果处于运动，则转向
        if (x != 0 || z != 0)
        {
            Quaternion rotation = Quaternion.LookRotation(velocity);
            if (transform.rotation != rotation) transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.fixedDeltaTime * rotateSpeed);
        }

        this.transform.position += velocity * Time.fixedDeltaTime * runSpeed;
        // 主角移动
    }

    /// <summary>
    /// 用于检测Actor进入某个区域
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Area"))
        {
            Publish publish = Publisher.getInstance();
            int patrolType = other.gameObject.name[other.gameObject.name.Length - 1] - '0';
            publish.notify(ActorState.ENTER_AREA, patrolType, this.gameObject);
            // 进入区域后，发布消息
        }
    }

    /// <summary>
    /// 用于检测Actor与Patrol碰撞后死亡
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Patrol") && ani.GetBool("isLive"))
        {
            ani.SetBool("isLive", false);
            ani.SetTrigger("toDie");
            // 执行死亡动作

            Publish publish = Publisher.getInstance();
            publish.notify(ActorState.DEATH, 0, null);
            // 碰撞后，发布死亡信息
        }
    }
}
