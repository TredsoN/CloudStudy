using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class RegisterController : MonoBehaviour
{
    private string RegistArea = "RegisterPage/RegisterArea/View/Content";

    /// <summary>
    /// 脚本加载
    /// </summary>
    private void Awake()
    {
        GameObject RegistPage = GameObject.Find("RegisterPage");
        RegistPage.transform.SetParent(GameObject.Find("Canvas").transform);
        RegistPage.GetComponent<RectTransform>().sizeDelta = new Vector2(BasicInformation.ScreenWidth, BasicInformation.ScreenHeight);
        RegistPage.GetComponent<RectTransform>().localPosition = new Vector2(BasicInformation.ScreenWidth, 0);
        RegistPage.transform.DOMoveX(BasicInformation.ScreenWidth / 2, 0.1f);
        GameObject.Find("RegisterPage/Return").GetComponent<Button>().onClick.AddListener(() => { OnReturnClicked(); });
        GameObject.Find(RegistArea + "/SendCode").GetComponent<Button>().onClick.AddListener(() => { OnGetCodeClicked(); });
        GameObject.Find(RegistArea + "/Submit").GetComponent<Button>().onClick.AddListener(() => { OnSubmitClicked(); });
    }

    /// <summary>
    /// 返回
    /// </summary>
    public void OnReturnClicked()
    {
        GameObject x = GameObject.Find("RegisterPage");
        x.transform.DOMoveX(3 * BasicInformation.ScreenWidth / 2, 0.1f).OnComplete(() => { Object.Destroy(x); });
    }

    /// <summary>
    /// 获取验证码
    /// </summary>
    public void OnGetCodeClicked()
    {
        if (RegistInformTest())
        {
            if (RegistInformTest())
            {
                string Email = GameObject.Find(RegistArea + "/Email").GetComponent<InputField>().text;
                StartCoroutine(BasicInformation.UserInterface.IF01_0(Email));
            }
        }
    }

    /// <summary>
    /// 提交
    /// </summary>
    public void OnSubmitClicked()
    {
        TextValidator validator = new TextValidator();
        Text CodeNote = GameObject.Find(RegistArea + "/CodeNote").GetComponent<Text>(),
            IntroNote = GameObject.Find(RegistArea + "/IntroNote").GetComponent<Text>();
        string Name = GameObject.Find(RegistArea + "/Name").GetComponent<InputField>().text,
            Password = GameObject.Find(RegistArea + "/Password").GetComponent<InputField>().text,
            Password2 = GameObject.Find(RegistArea + "/Password2").GetComponent<InputField>().text,
            Age = GameObject.Find(RegistArea + "/Age").GetComponent<InputField>().text,
            Email = GameObject.Find(RegistArea + "/Email").GetComponent<InputField>().text,
            Code = GameObject.Find(RegistArea + "/Code").GetComponent<InputField>().text,
            Introduction = GameObject.Find(RegistArea + "/Introduction").GetComponent<InputField>().text;
        int Gender = GameObject.Find(RegistArea + "/Gender/Regist_GenderM").GetComponent<Toggle>().isOn ? 0 : 1;
        bool a = RegistInformTest(), b = validator.CodeTest(Code), c = validator.SigTest(Introduction);
        if (a && b && c)
        {
            CodeNote.text = "";
            StartCoroutine(BasicInformation.UserInterface.IF02(Name, Password, Gender, int.Parse(Age), Introduction, Email, int.Parse(Code)));
        }
        else
        {
            if (Code == "")
                CodeNote.text = "验证码不能为空";
            else
            {
                CodeNote.text = b ? "" : "验证码需为6位数字";
                IntroNote.text = c ? "" : "个人介绍不能超过45个字";
            }
        }
    }

    /// <summary>
    /// 注册信息格式验证
    /// </summary>
    private bool RegistInformTest()
    {
        TextValidator validator = new TextValidator();
        Text NameNote = GameObject.Find(RegistArea + "/NameNote").GetComponent<Text>(),
            PasswordNote = GameObject.Find(RegistArea + "/PasswordNote").GetComponent<Text>(),
            PasswordNote2 = GameObject.Find(RegistArea + "/PasswordNote2").GetComponent<Text>(),
            AgeNote = GameObject.Find(RegistArea + "/AgeNote").GetComponent<Text>(),
            EmailNote = GameObject.Find(RegistArea + "/EmailNote").GetComponent<Text>();
        string Name = GameObject.Find(RegistArea + "/Name").GetComponent<InputField>().text,
            Password = GameObject.Find(RegistArea + "/Password").GetComponent<InputField>().text,
            Password2 = GameObject.Find(RegistArea + "/Password2").GetComponent<InputField>().text,
            Age = GameObject.Find(RegistArea + "/Age").GetComponent<InputField>().text,
            Email = GameObject.Find(RegistArea + "/Email").GetComponent<InputField>().text;
        bool a = validator.NameTest(Name),
            b = validator.PasswordTest(Password),
            c = validator.AgeTest(Age),
            d = validator.EmailTest(Email);
        if (a && b && c && d)
        {
            if (Password2 != Password)
            {
                PasswordNote2.text = "两次输入密码不一致";
                return false;
            }
            NameNote.text = PasswordNote.text = PasswordNote2.text = AgeNote.text = EmailNote.text = "";
            return true;
        }
        else
        {
            if (Name == "")
                NameNote.text = "用户名不能为空";
            else
                NameNote.text = a ? "" : "用户名不能超过8位";
            if (Password == "")
                PasswordNote.text = "密码不能为空";
            else
                PasswordNote.text = b ? "" : "密码需为8-16位";
            if (Age == "")
                AgeNote.text = "年龄不能为空";
            else
                AgeNote.text = c ? "" : "请填写正确的年龄";
            if (Email == "")
                EmailNote.text = "邮箱不能为空";
            else
                EmailNote.text = d ? "" : "请填写正确的邮箱";
            return false;
        }
    }
}
