/*
 * 描 述：场景控制类，控制UI和Disk的生产
 * 作 者：hza 
 * 创建时间：2017/04/16 00:16:32
 * 版 本：v 1.0
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneController : MonoBehaviour, Observer
{
    public Text scoreText;
    public Text centerText;

    private ScoreRecorder record;
    private UIController UI;
    private ObjectFactory fac;

    private float[] posx = { -5, 7, -5, 5 };
    private float[] posz = { -5, -7, 5, 5 };
    // 开始位置

    void Start()
    {
        record = new ScoreRecorder();
        record.scoreText = scoreText;
        UI = new UIController();
        UI.centerText = centerText;
        fac = Singleton<ObjectFactory>.Instance;

        Publish publisher = Publisher.getInstance();
        publisher.add(this);
        // 添加事件

        LoadResources();
    }

    private void LoadResources()
    {
        Instantiate(Resources.Load("prefabs/Ami"), new Vector3(2, 0, -2), Quaternion.Euler(new Vector3(0, 180, 0)));
        // 初始化主角
        ObjectFactory fac = Singleton<ObjectFactory>.Instance;
        for (int i = 0; i < posx.Length; i++)
        {
            GameObject patrol = fac.setObjectOnPos(new Vector3(posx[i], 0, posz[i]), Quaternion.Euler(new Vector3(0, 180, 0)));
            patrol.name = "Patrol" + (i + 1);
            // 初始化巡逻兵
        }
    }

    /// <summary>
    /// 如果角色死亡，显示LOSE
    /// </summary>
    /// <param name="state">订阅状态</param>
    /// <param name="pos"></param>
    public void notified(ActorState state, int pos, GameObject actor)
    {
        if (state == ActorState.ENTER_AREA) record.addScore(1);
        else UI.loseGame();
    }
}
