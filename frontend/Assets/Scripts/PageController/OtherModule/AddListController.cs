using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddListController : MonoBehaviour
{
    /// <summary>
    /// 脚本加载
    /// </summary>
    private void Awake()
    {
        GameObject CreateTodoListPage = GameObject.Find("CreateTodoListPage~");
        CreateTodoListPage.transform.SetParent(GameObject.Find("Canvas").transform);
        CreateTodoListPage.GetComponent<RectTransform>().sizeDelta = new Vector2(BasicInformation.ScreenWidth, BasicInformation.ScreenHeight);
        CreateTodoListPage.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
        GameObject.Find("CreateTodoListPage~/InfoArea/Back").GetComponent<Button>().onClick.AddListener(() => { OnReturnClicked(); });
        GameObject.Find("CreateTodoListPage~/InfoArea/Submit").GetComponent<Button>().onClick.AddListener(() => { OnSubmitClicked(); });
    }

    /// <summary>
    /// 返回
    /// </summary>
    private void OnReturnClicked()
    {
        Destroy(GameObject.Find("CreateTodoListPage~"));
    }

    /// <summary>
    /// 创建倒计时
    /// </summary>
    private void OnSubmitClicked()
    {
        Text Note = GameObject.Find("CreateTodoListPage~/InfoArea/Note").GetComponent<Text>();
        string Name = GameObject.Find("CreateTodoListPage~/InfoArea/Name").GetComponent<InputField>().text;
        if (Name == "")
            Note.text = "待办清单名称不能为空";
        else if (Name.Length > 15)
            Note.text = "待办清单名称不能超过15位";
        else
        {
            Note.text = "";
            StartCoroutine(BasicInformation.OtherInterface.IF03(BasicInformation.CurUser.Id, Name));
        }
    }
}
