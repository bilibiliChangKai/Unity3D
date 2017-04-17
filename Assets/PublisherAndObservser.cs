/*
 * 描 述：发布者和订阅者接口，和发布者的实际实现
 * 作 者：hza 
 * 创建时间：2017/04/16 09:55:51
 * 版 本：v 1.0
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Publish
{
    void notify(ActorState state, int pos, GameObject actor);
    // 发布函数

    void add(Observer observer);
    // 委托添加事件

    void delete(Observer observer);
    // 委托取消事件
}

public interface Observer
{
    void notified(ActorState state, int pos, GameObject actor);
    // 实现接收函数
}

public enum ActorState { ENTER_AREA, DEATH }

public class Publisher : Publish {

    private delegate void ActionUpdate(ActorState state, int pos, GameObject actor);
    private ActionUpdate updatelist;
    // 委托定义

    /// <summary>
    /// 单实例模式
    /// </summary>
    private static Publish _instance;
    public static Publish getInstance()
    {
        if (_instance == null) _instance = new Publisher();
        return _instance;
    }

    public void notify(ActorState state, int pos, GameObject actor)
    {
        if (updatelist != null) updatelist(state, pos, actor);
        // 发布信息
    }

    public void add(Observer observer)
    {
        updatelist += observer.notified;
    }

    public void delete(Observer observer)
    {
        updatelist -= observer.notified;
    }
}
