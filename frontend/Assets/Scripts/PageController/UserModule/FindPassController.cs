using DG.Tweening;
using UnityEngine.UI;
using UnityEngine;

public class FindPassController : MonoBehaviour
{
    string Email;
    /// <summary>
    /// 接收初始化信息
    /// </summary>
    public void GetMsg(int id)
    {
        if (id == 1)
        {
            GameObject FindPage = GameObject.Find("PasswordFindPage1");
            FindPage.transform.SetParent(GameObject.Find("Canvas").transform);
            FindPage.GetComponent<RectTransform>().sizeDelta = new Vector2(BasicInformation.ScreenWidth, BasicInformation.ScreenHeight);
            FindPage.GetComponent<RectTransform>().localPosition = new Vector2(BasicInformation.ScreenWidth, 0);
            FindPage.transform.DOMoveX(BasicInformation.ScreenWidth / 2, 0.1f);
            GameObject.Find("PasswordFindPage1/Return").GetComponent<Button>().onClick.AddListener(() => { OnReturnClicked1(); });
            GameObject.Find("PasswordFindPage1/FindArea/SendCode").GetComponent<Button>().onClick.AddListener(() => { OnGetCodeClicked(); });
            GameObject.Find("PasswordFindPage1/FindArea/Submit").GetComponent<Button>().onClick.AddListener(() => { OnSubmitClicked1(); });
        }
        else
        {
            GameObject FindPage2 = GameObject.Find("PasswordFindPage2");
            FindPage2.transform.SetParent(GameObject.Find("Canvas").transform);
            FindPage2.GetComponent<RectTransform>().sizeDelta = new Vector2(BasicInformation.ScreenWidth, BasicInformation.ScreenHeight);
            FindPage2.GetComponent<RectTransform>().localPosition = new Vector2(BasicInformation.ScreenWidth, 0);
            FindPage2.transform.DOMoveX(BasicInformation.ScreenWidth / 2, 0.1f).OnComplete(() => { Object.Destroy(GameObject.Find("PasswordFindPage1")); });
            GameObject.Find("PasswordFindPage2/Return").GetComponent<Button>().onClick.AddListener(() => { OnReturnClicked2(); });
            GameObject.Find("PasswordFindPage2/FindArea/Submit").GetComponent<Button>().onClick.AddListener(() => { OnSubmitClicked2(); });
        }
    }

    /// <summary>
    /// 接收邮箱
    /// </summary>
    public void GetEmail(string email)
    {
        Email = email;
    }

    /// <summary>
    /// 返回1
    /// </summary>
    public void OnReturnClicked1()
    {
        GameObject x = GameObject.Find("PasswordFindPage1");
        x.transform.DOMoveX(3 * BasicInformation.ScreenWidth / 2, 0.1f).OnComplete(() => { Destroy(x); });
    }

    /// <summary>
    /// 返回2
    /// </summary>
    public void OnReturnClicked2()
    {
        GameObject x = GameObject.Find("PasswordFindPage2");
        x.transform.DOMoveX(3 * BasicInformation.ScreenWidth / 2, 0.1f).OnComplete(() => { Destroy(x); });
    }

    /// <summary>
    /// 获取验证码
    /// </summary>
    public void OnGetCodeClicked()
    {
        Text EmailNote = GameObject.Find("PasswordFindPage1/FindArea/EmailNote").GetComponent<Text>();
        string Email = GameObject.Find("PasswordFindPage1/FindArea/Email").GetComponent<InputField>().text;
        TextValidator validator = new TextValidator();
        if (Email == "")
            EmailNote.text = "邮箱不能为空";
        else if (!validator.EmailTest(Email))
            EmailNote.text = "邮箱格式错误";
        else
        {
            EmailNote.text = "";
            StartCoroutine(BasicInformation.UserInterface.IF01_1(Email));
        }
    }

    /// <summary>
    /// 提交验证码核对
    /// </summary>
    public void OnSubmitClicked1()
    {
        Text CodeNote = GameObject.Find("PasswordFindPage1/FindArea/CodeNote").GetComponent<Text>();
        string Code = GameObject.Find("PasswordFindPage1/FindArea/Code").GetComponent<InputField>().text,
            Email = GameObject.Find("PasswordFindPage1/FindArea/Email").GetComponent<InputField>().text;
        TextValidator validator = new TextValidator();
        if (Code == "")
            CodeNote.text = "验证码不能为空";
        else if (!validator.CodeTest(Code))
            CodeNote.text = "验证码需为6位数字";
        else
        {
            CodeNote.text = "";
            StartCoroutine(BasicInformation.UserInterface.IF07(Email, int.Parse(Code)));
        }
    }

    /// <summary>
    /// 提交新密码
    /// </summary>
    public void OnSubmitClicked2()
    {
        Text PassNote1 = GameObject.Find("PasswordFindPage2/FindArea/PassNote1").GetComponent<Text>(),
            PassNote2 = GameObject.Find("PasswordFindPage2/FindArea/PassNote2").GetComponent<Text>();
        string Pass1 = GameObject.Find("PasswordFindPage2/FindArea/NewPassword").GetComponent<InputField>().text,
            Pass2 = GameObject.Find("PasswordFindPage2/FindArea/NewPassword2").GetComponent<InputField>().text;
        TextValidator validator = new TextValidator();
        if (Pass1 == "")
            PassNote1.text = "密码不能为空";
        else if (!validator.PasswordTest(Pass1))
            PassNote1.text = "密码需为8-16位";
        else if (Pass2 != Pass1)
        {
            PassNote1.text = "";
            PassNote2.text = "两次输入密码不一致";
        }
        else
        {
            PassNote1.text = PassNote2.text = "";
            StartCoroutine(BasicInformation.UserInterface.IF08(Email, Pass1));
        }
    }
}