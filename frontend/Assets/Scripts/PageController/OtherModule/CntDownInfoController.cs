using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class CntDownInfoController : MonoBehaviour
{
    /// <summary>
    /// 脚本加载
    /// </summary>
    private void Awake()
    {
        GameObject EditCntDownPage = GameObject.Find("EditCntDownPage~");
        EditCntDownPage.transform.SetParent(GameObject.Find("Canvas").transform);
        EditCntDownPage.GetComponent<RectTransform>().sizeDelta = new Vector2(BasicInformation.ScreenWidth, BasicInformation.ScreenHeight);
        EditCntDownPage.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
        GameObject.Find("EditCntDownPage~/InfoArea/Back").GetComponent<Button>().onClick.AddListener(() => { OnReturnClicked(); });
    }

    /// <summary>
    /// 接收获取星球
    /// </summary>
    /// <param name="c"></param>
    public void GetCntDown(CntDown c)
    {
        GameObject.Find("EditCntDownPage~/InfoArea/Name").GetComponent<InputField>().text = c.Name;
        GameObject.Find("EditCntDownPage~/InfoArea/Remark").GetComponent<InputField>().text = c.Remark;
        GameObject.Find("EditCntDownPage~/InfoArea/EndTime").GetComponent<InputField>().text = c.EndTime.ToString("yyyy-MM-dd");
        GameObject.Find("EditCntDownPage~/InfoArea/Submit").GetComponent<Button>().onClick.AddListener(() => { OnSubmitClicked(c.Id); });
        GameObject.Find("EditCntDownPage~/InfoArea/Delete").GetComponent<Button>().onClick.AddListener(() => { OnDeleteClicked(c.Id); });
    }

    /// <summary>
    /// 返回
    /// </summary>
    private void OnReturnClicked()
    {
        Destroy(GameObject.Find("EditCntDownPage~"));
    }

    /// <summary>
    /// 删除倒计时
    /// </summary>
    private void OnDeleteClicked(int id)
    {
        GameObject Warning = (GameObject)Instantiate(Resources.Load("Prefabs/Warning"));
        Warning.name = "Warning";
        Warning.transform.SetParent(GameObject.Find("Canvas").transform);
        Warning.GetComponent<RectTransform>().sizeDelta = new Vector2(BasicInformation.ScreenWidth, BasicInformation.ScreenHeight);
        Warning.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
        GameObject.Find("Warning/Info/Text").GetComponent<Text>().text = "即将删除倒计时，是否确认？";
        GameObject.Find("Warning/Info/Submit").GetComponent<Button>().onClick.AddListener(() => {
            Destroy(Warning);
            StartCoroutine(BasicInformation.OtherInterface.IF14(id));
        });
        GameObject.Find("Warning/Info/Cancel").GetComponent<Button>().onClick.AddListener(() => {
            Destroy(Warning);
        });
    }

    /// <summary>
    /// 修改倒计时
    /// </summary>
    private void OnSubmitClicked(int id)
    {
        Text Note1 = GameObject.Find("EditCntDownPage~/InfoArea/Note1").GetComponent<Text>(),
            Note2 = GameObject.Find("EditCntDownPage~/InfoArea/Note1").GetComponent<Text>(),
            Note3 = GameObject.Find("EditCntDownPage~/InfoArea/Note3").GetComponent<Text>();
        string Name = GameObject.Find("EditCntDownPage~/InfoArea/Name").GetComponent<InputField>().text,
            Remark = GameObject.Find("EditCntDownPage~/InfoArea/Remark").GetComponent<InputField>().text,
                Time = GameObject.Find("EditCntDownPage~/InfoArea/EndTime").GetComponent<InputField>().text;
        if (Name == "")
            Note1.text = "倒计时名称不能为空";
        else if (Name.Length > 12)
            Note1.text = "倒计时名称不能超过12位";
        else
        {
            Note1.text = "";
            if (Remark.Length > 30)
                Note2.text = "星球简介不能超过30位";
            else
            {
                Note2.text = "";
                if (Time == "")
                    Note3.text = "结束时间不能为空";
                else
                {
                    try
                    {
                        DateTime dt;
                        DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();
                        dtFormat.ShortDatePattern = "yyyy-MM-dd";
                        dt = Convert.ToDateTime(Time, dtFormat);
                        Note3.text = "";
                        StartCoroutine(BasicInformation.OtherInterface.IF12(id, Name, Remark, dt.ToString("yyyy-MM-dd")));
                    }
                    catch
                    {
                        Note3.text = "时间格式错误";
                    }
                }
            }
        }
    }
}
