using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarInfoChangeController : MonoBehaviour
{
    /// <summary>
    /// 脚本加载
    /// </summary>
    private void Awake()
    {
        GameObject StarInfoChangePage = GameObject.Find("StarInfoChangePage~");
        StarInfoChangePage.transform.SetParent(GameObject.Find("Canvas").transform);
        StarInfoChangePage.GetComponent<RectTransform>().sizeDelta = new Vector2(BasicInformation.ScreenWidth, BasicInformation.ScreenHeight);
        StarInfoChangePage.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
        GameObject.Find("StarInfoChangePage~/InfoArea/Back").GetComponent<Button>().onClick.AddListener(() => { OnReturnClicked(); });
    }

    /// <summary>
    /// 接收星球id
    /// </summary>
    /// <param name="id"></param>
    public void GetStar(Star star)
    {
        GameObject.Find("StarInfoChangePage~/InfoArea/Name").GetComponent<InputField>().text = star.Name;
        GameObject.Find("StarInfoChangePage~/InfoArea/Introduce").GetComponent<InputField>().text = star.Introduce;
        GameObject.Find("StarInfoChangePage~/InfoArea/Submit").GetComponent<Button>().onClick.AddListener(() => { OnSubmitClicked(star.Id); });
    }

    /// <summary>
    /// 返回
    /// </summary>
    private void OnReturnClicked()
    {
        Destroy(GameObject.Find("StarInfoChangePage~"));
    }

    /// <summary>
    /// 提交
    /// </summary>
    /// <param name="starid">星球编号</param>
    private void OnSubmitClicked(int starid)
    {
        Text Note1 = GameObject.Find("StarInfoChangePage~/InfoArea/Note1").GetComponent<Text>(),
            Note2 = GameObject.Find("StarInfoChangePage~/InfoArea/Note2").GetComponent<Text>();
        string Name = GameObject.Find("StarInfoChangePage~/InfoArea/Name").GetComponent<InputField>().text,
                Intro = GameObject.Find("StarInfoChangePage~/InfoArea/Introduce").GetComponent<InputField>().text;
        if (Name == "")
            Note1.text = "星球名称不能为空";
        else if (Name.Length > 15)
            Note1.text = "星球名称不能超过15位";
        else
        {
            Note1.text = "";
            if (Intro == "")
                Note2.text = "星球简介不能为空";
            else if (Intro.Length > 25)
                Note2.text = "星球简介不能超过25位";
            else
            {
                Note2.text = "";
                StartCoroutine(BasicInformation.StarInterface.IF02(starid, Name, Intro));
            }
        }
    }
}
