/*
 * 描 述：UI控制
 * 作 者：hza 
 * 创建时间：2017/04/16 00:20:37
 * 版 本：v 1.0
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController {

    public Text centerText;

	public void loseGame()
    {
        centerText.text = "LOSE!";
    }

    public void resetGame()
    {
        centerText.text = "";
    }
}
