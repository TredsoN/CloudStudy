using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class AddCntDownController : MonoBehaviour
{
    /// <summary>
    /// 脚本加载
    /// </summary>
    private void Awake()
    {
        GameObject CreateCntDownPage = GameObject.Find("CreateCntDownPage~");
        CreateCntDownPage.transform.SetParent(GameObject.Find("Canvas").transform);
        CreateCntDownPage.GetComponent<RectTransform>().sizeDelta = new Vector2(BasicInformation.ScreenWidth, BasicInformation.ScreenHeight);
        CreateCntDownPage.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
        GameObject.Find("CreateCntDownPage~/InfoArea/Back").GetComponent<Button>().onClick.AddListener(() => { OnReturnClicked(); });
        GameObject.Find("CreateCntDownPage~/InfoArea/Submit").GetComponent<Button>().onClick.AddListener(() => { OnSubmitClicked(); });
    }

    /// <summary>
    /// 返回
    /// </summary>
    private void OnReturnClicked()
    {
        Destroy(GameObject.Find("CreateCntDownPage~"));
    }

    /// <summary>
    /// 创建倒计时
    /// </summary>
    private void OnSubmitClicked()
    {
        Text Note1 = GameObject.Find("CreateCntDownPage~/InfoArea/Note1").GetComponent<Text>(),
            Note2 = GameObject.Find("CreateCntDownPage~/InfoArea/Note2").GetComponent<Text>(),
            Note3 = GameObject.Find("CreateCntDownPage~/InfoArea/Note3").GetComponent<Text>();
        string Name = GameObject.Find("CreateCntDownPage~/InfoArea/Name").GetComponent<InputField>().text,
            Remark = GameObject.Find("CreateCntDownPage~/InfoArea/Remark").GetComponent<InputField>().text,
                Time = GameObject.Find("CreateCntDownPage~/InfoArea/EndTime").GetComponent<InputField>().text;
        if (Name == "")
            Note1.text = "倒计时名称不能为空";
        else if (Name.Length > 12)
            Note1.text = "倒计时名称不能超过12位";
        else
        {
            Note1.text = "";
            if (Remark.Length > 30)
                Note2.text = "备注不能超过30位";
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
                        StartCoroutine(BasicInformation.OtherInterface.IF11(BasicInformation.CurUser.Id, Name, Remark, dt.ToString("yyyy-MM-dd")));
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
