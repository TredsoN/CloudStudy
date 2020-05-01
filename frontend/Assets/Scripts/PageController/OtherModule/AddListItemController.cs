using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddListItemController : MonoBehaviour
{
    /// <summary>
    /// 脚本加载
    /// </summary>
    private void Awake()
    {
        GameObject CreateTodoItemPage = GameObject.Find("CreateTodoItemPage~");
        CreateTodoItemPage.transform.SetParent(GameObject.Find("Canvas").transform);
        CreateTodoItemPage.GetComponent<RectTransform>().sizeDelta = new Vector2(BasicInformation.ScreenWidth, BasicInformation.ScreenHeight);
        CreateTodoItemPage.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
        GameObject.Find("CreateTodoItemPage~/InfoArea/Back").GetComponent<Button>().onClick.AddListener(() => { OnReturnClicked(); });

    }

    /// <summary>
    /// 接收清单id
    /// </summary>
    /// <param name="id"></param>
    public void GetListId(int id)
    {
        GameObject.Find("CreateTodoItemPage~/InfoArea/Submit").GetComponent<Button>().onClick.AddListener(() => { OnSubmitClicked(id); });
    }

    /// <summary>
    /// 返回
    /// </summary>
    private void OnReturnClicked()
    {
        Destroy(GameObject.Find("CreateTodoItemPage~"));
    }

    /// <summary>
    /// 创建待办事项
    /// </summary>
    private void OnSubmitClicked(int id)
    {
        Text Note1 = GameObject.Find("CreateTodoItemPage~/InfoArea/Note1").GetComponent<Text>(),
             Note2 = GameObject.Find("CreateTodoItemPage~/InfoArea/Note2").GetComponent<Text>();
        string Prior = GameObject.Find("CreateTodoItemPage~/InfoArea/Prior").GetComponent<InputField>().text,
            Content = GameObject.Find("CreateTodoItemPage~/InfoArea/Content").GetComponent<InputField>().text;
        if (Content.Length == 0)
            Note2.text = "待办内容不能为空";
        else if (Content.Length >= 30)
            Note2.text = "待办内容不能超过30个字";
        else
        {
            Note2.text = "";
            try
            {
                int prior = int.Parse(Prior);
                if (prior >= 1 && prior <= 3)
                {
                    Note1.text = "";
                    StartCoroutine(BasicInformation.OtherInterface.IF04(id, Content, prior-1));
                }
                else
                    Note1.text = "优先级请填写1,2,3";
            }
            catch
            {
                Note1.text = "优先级请填写1,2,3";
            }
        }
    }
}