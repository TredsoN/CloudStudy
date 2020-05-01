using UnityEngine;
using UnityEngine.UI;

public class InfoChangeController : MonoBehaviour
{
    /// <summary>
    /// 接收初始化信息
    /// </summary>
    public void GetMsg(int id)
    {
        if (id == 4)
        {
            GameObject PassChangePage = GameObject.Find("PassChangePage~");
            PassChangePage.transform.SetParent(GameObject.Find("Canvas").transform);
            PassChangePage.GetComponent<RectTransform>().sizeDelta = new Vector2(BasicInformation.ScreenWidth, BasicInformation.ScreenHeight);
            PassChangePage.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
            GameObject.Find("PassChangePage~/ChangeArea/Back").GetComponent<Button>().onClick.AddListener(() => { OnReturnPassClicked(); });
            GameObject.Find("PassChangePage~/ChangeArea/Submit").GetComponent<Button>().onClick.AddListener(() => { OnSubmitPassClicked(); });
        }
        else
        {
            GameObject InfoChangePage = GameObject.Find("InfoChangePage~");
            InfoChangePage.transform.SetParent(GameObject.Find("Canvas").transform);
            InfoChangePage.GetComponent<RectTransform>().sizeDelta = new Vector2(BasicInformation.ScreenWidth, BasicInformation.ScreenHeight);
            InfoChangePage.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
            if (id == 0)
            {
                GameObject.Find("InfoChangePage~/ChangeArea/Input").GetComponent<InputField>().text = BasicInformation.CurUser.Name;
                GameObject.Find("InfoChangePage~/ChangeArea/Hint").GetComponent<Text>().text = "请输入昵称";
            }
            else if (id == 1)
            {
                GameObject.Find("InfoChangePage~/ChangeArea/Input").GetComponent<InputField>().text = BasicInformation.CurUser.Gender == 0 ? "男" : "女";
                GameObject.Find("InfoChangePage~/ChangeArea/Hint").GetComponent<Text>().text = "请输入性别（男/女）";
            }
            else if (id == 2)
            {
                GameObject.Find("InfoChangePage~/ChangeArea/Input").GetComponent<InputField>().text = BasicInformation.CurUser.Age.ToString();
                GameObject.Find("InfoChangePage~/ChangeArea/Hint").GetComponent<Text>().text = "请输入年龄";
            }
            else
            {
                GameObject.Find("InfoChangePage~/ChangeArea/Input").GetComponent<InputField>().text = BasicInformation.CurUser.Introduce;
                GameObject.Find("InfoChangePage~/ChangeArea/Hint").GetComponent<Text>().text = "请输入个性签名";
            }
            GameObject.Find("InfoChangePage~/ChangeArea/Back").GetComponent<Button>().onClick.AddListener(() => { OnReturnClicked(); });
            GameObject.Find("InfoChangePage~/ChangeArea/Submit").GetComponent<Button>().onClick.AddListener(() => { OnSubmitClicked(id); });
        }
    }

    /// <summary>
    /// 返回
    /// </summary>
    public void OnReturnClicked()
    {
        GameObject x = GameObject.Find("InfoChangePage~");
        Destroy(x);
    }

    /// <summary>
    /// 返回
    /// </summary>
    public void OnReturnPassClicked()
    {
        GameObject x = GameObject.Find("PassChangePage~");
        Destroy(x);
    }

    /// <summary>
    /// 提交内容
    /// </summary>
    public void OnSubmitClicked(int type)
    {
        Text note = GameObject.Find("InfoChangePage~/ChangeArea/Note").GetComponent<Text>();
        string content = GameObject.Find("InfoChangePage~/ChangeArea/Input").GetComponent<InputField>().text;
        if (type == 0)
        {
            if (content == "")
                note.text = "用户名不能为空";
            else if (content.Length > 8)
                note.text = "用户名不能超过8位";
            else
            {
                note.text = "";
                StartCoroutine(BasicInformation.UserInterface.IF05(BasicInformation.CurUser.Id, 0, content));
            }
        }
        else if (type == 1)
        {
            if (content != "男" && content != "女")
                note.text = "请输入正确的性别";
            else
            {
                note.text = "";
                StartCoroutine(BasicInformation.UserInterface.IF05(BasicInformation.CurUser.Id, 1, content == "男" ? "0" : "1"));
            }

        }
        else if (type == 2)
        {
            try
            {
                int age = int.Parse(content);
                if (age >= 100 || age <= 0 || content.Contains("+") || content.Contains("-"))
                    note.text = "请输入正确的年龄";
                else
                {
                    note.text = "";
                    StartCoroutine(BasicInformation.UserInterface.IF05(BasicInformation.CurUser.Id, 2, content));
                }
            }
            catch
            {
                note.text = "请输入正确的年龄";
            }
        }
        else if (type == 3)
        {
            if (content.Length > 45)
                note.text = "个性签名不能超过45位";
            else
            {
                note.text = "";
                StartCoroutine(BasicInformation.UserInterface.IF05(BasicInformation.CurUser.Id, 3, content));
            }
        }
    }

    /// <summary>
    /// 提交内容
    /// </summary>
    public void OnSubmitPassClicked()
    {
        Text Note1 = GameObject.Find("PassChangePage~/ChangeArea/Note1").GetComponent<Text>(),
            Note2 = GameObject.Find("PassChangePage~/ChangeArea/Note2").GetComponent<Text>(),
            Note3 = GameObject.Find("PassChangePage~/ChangeArea/Note3").GetComponent<Text>();
        string OldPass = GameObject.Find("PassChangePage~/ChangeArea/OldPass").GetComponent<InputField>().text,
            NewPass = GameObject.Find("PassChangePage~/ChangeArea/NewPass").GetComponent<InputField>().text,
            NewPass2 = GameObject.Find("PassChangePage~/ChangeArea/NewPass2").GetComponent<InputField>().text;
        TextValidator validator = new TextValidator();
        bool a = validator.PasswordTest(OldPass), b = validator.PasswordTest(NewPass);
        if (a && b)
        {
            if (NewPass2 != NewPass)
                Note3.text = "两次输入密码不一致";
            else
            {
                Note1.text = Note2.text = Note3.text = "";
                StartCoroutine(BasicInformation.UserInterface.IF06(BasicInformation.CurUser.Id, OldPass, NewPass));
            }
        }
        else
        {
            if (OldPass.Length == 0)
                Note1.text = "原密码不能为空";
            else
                Note1.text = a ? "" : "密码需为8-16位";
            if (NewPass.Length == 0)
                Note2.text = "原密码不能为空";
            else
                Note2.text = b ? "" : "密码需为8-16位";
        }
    }
}
