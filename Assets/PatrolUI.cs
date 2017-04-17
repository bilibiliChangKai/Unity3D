/*
 * 描 述：挂在在Patrol上面，执行Patrol自动巡逻动作
 * 作 者：hza 
 * 创建时间：2017/04/13 11:24:41
 * 版 本：v 1.0
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tem.Action;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]

public class PatrolUI : SSActionManager, ISSActionCallback, Observer {

    public enum ActionState : int { IDLE, WALKLEFT, WALKFORWARD, WALKRIGHT, WALKBACK }
    // 各种动作

    private Animator ani;
    // 动作

    private SSAction currentAction;
    private ActionState currentState;
    // 保证当前只有一个动作
    private const float walkSpeed = 1f;
    private const float runSpeed = 3f;
    // 跑步和走路的速度

	// Use this for initialization
	new void Start () {
        ani = this.gameObject.GetComponent<Animator>();
        Publish publisher = Publisher.getInstance();
        publisher.add(this);
        // 添加事件

        currentState = ActionState.IDLE;
        idle();
        // 开始时，静止状态
	}
	
	// Update is called once per frame
	new void Update () {
        base.Update();
	}

    public void SSEventAction(SSAction source, SSActionEventType events = SSActionEventType.COMPLETED, int intParam = 0, string strParam = null, Object objParam = null)
    {
        currentState = currentState > ActionState.WALKBACK ? ActionState.IDLE : (ActionState)((int)currentState + 1);
        // 改变当前状态
        switch (currentState)
        {
            case ActionState.WALKLEFT:
                walkLeft();
                break;
            case ActionState.WALKRIGHT:
                walkRight();
                break;
            case ActionState.WALKFORWARD:
                walkForward();
                break;
            case ActionState.WALKBACK:
                walkBack();
                break;
            default:
                idle();
                break;
        }
        // 执行下个动作
    }

    public void idle()
    {
        currentAction = IdleAction.GetIdleAction(Random.Range(1, 1.5f), ani);
        this.runAction(this.gameObject, currentAction, this);
    }

    public void walkLeft()
    {
        Vector3 target = Vector3.left * Random.Range(3, 5) + this.transform.position;
        currentAction = WalkAction.GetWalkAction(target, walkSpeed, ani);
        this.runAction(this.gameObject, currentAction, this);
    }
    public void walkRight()
    {
        Vector3 target = Vector3.right * Random.Range(3, 5) + this.transform.position;
        currentAction = WalkAction.GetWalkAction(target, walkSpeed, ani);
        this.runAction(this.gameObject, currentAction, this);
    }

    public void walkForward()
    {
        Vector3 target = Vector3.forward * Random.Range(3, 5) + this.transform.position;
        currentAction = WalkAction.GetWalkAction(target, walkSpeed, ani);
        this.runAction(this.gameObject, currentAction, this);
    }
    
    public void walkBack()
    {
        Vector3 target = Vector3.back * Random.Range(3, 5) + this.transform.position;
        currentAction = WalkAction.GetWalkAction(target, walkSpeed, ani);
        this.runAction(this.gameObject, currentAction, this);
    }

    /// <summary>
    /// 当碰到墙壁或者出了巡逻区域后，向反方向走
    /// </summary>
    public void turnNextDirection()
    {
        currentAction.destory = true;
        // 销毁当前动作
        switch (currentState)
        {
            case ActionState.WALKLEFT:
                currentState = ActionState.WALKRIGHT;
                walkRight();
                break;
            case ActionState.WALKRIGHT:
                currentState = ActionState.WALKLEFT;
                walkLeft();
                break;
            case ActionState.WALKFORWARD:
                currentState = ActionState.WALKBACK;
                walkBack();
                break;
            case ActionState.WALKBACK:
                currentState = ActionState.WALKFORWARD;
                walkForward();
                break;
        }
        // 执行相反动作
    }

    public void getGoal(GameObject gameobject)
    {
        currentAction.destory = true;
        // 销毁当前动作
        currentAction = RunAction.GetRunAction(gameobject.transform, runSpeed, ani);
        this.runAction(this.gameObject, currentAction, this);
        // 跑向目标方向
    }

    public void loseGoal()
    {
        currentAction.destory = true;
        // 销毁当前动作
        idle();
        // 重新进行动作循环
    }

    public void stop()
    {
        currentAction.destory = true;
        currentAction = IdleAction.GetIdleAction(-1f, ani);
        this.runAction(this.gameObject, currentAction, this);
        // 永久站立
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject. name);
        Transform parent = collision.gameObject.transform.parent;
        if (parent != null && parent.CompareTag("Wall")) turnNextDirection();
        // 撞到墙
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Door")) turnNextDirection();
        // 走出巡逻区域
    }

    /// <summary>
    /// 接受信息后执行的动作，判断角色在哪个区域内，并判断角色死亡
    /// </summary>
    /// <param name="state">角色状态</param>
    /// <param name="pos">角色所在区域</param>
    /// <param name="actor">角色</param>
    public void notified(ActorState state, int pos, GameObject actor)
    {
        if (state == ActorState.ENTER_AREA)
        {
            if (pos == this.gameObject.name[this.gameObject.name.Length - 1] - '0')
                getGoal(actor);
            // 如果进入自己的区域，进行追击
            else loseGoal();
            // 如果离开自己的区域，放弃追击
        }
        else stop();
        // 角色死亡，结束动作
    }
}
