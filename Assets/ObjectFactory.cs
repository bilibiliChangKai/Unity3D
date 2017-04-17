/*
 * 描 述：工厂模型，用于生产物体和保存空闲物体
 * 作 者：hza 
 * 创建时间：2017/04/16 00:22:43
 * 版 本：v 1.0
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFactory : MonoBehaviour
{

    private static List<GameObject> used = new List<GameObject>();
    // 正在使用的对象链表
    private static List<GameObject> free = new List<GameObject>();
    // 正在空闲的对象链表

    // 此函数表示将物体放到一个指定位置，并且面向方向指定
    public GameObject setObjectOnPos(Vector3 targetposition, Quaternion faceposition)
    {
        if (free.Count == 0)
        {
            GameObject aGameObject = Instantiate(Resources.Load("prefabs/Patrol")
                , targetposition, faceposition) as GameObject;
            // 新建实例，将位置设置成为targetposition，将面向方向设置成faceposition
            used.Add(aGameObject);
        }
        else
        {
            used.Add(free[0]);
            free.RemoveAt(0);
            used[used.Count - 1].SetActive(true);
            used[used.Count - 1].transform.position = targetposition;
            used[used.Count - 1].transform.localRotation = faceposition;
        }
        return used[used.Count - 1];
    }

    public void freeObject(GameObject oj)
    {
        oj.SetActive(false);
        used.Remove(oj);
        free.Add(oj);
    }
}
