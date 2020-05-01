using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LoginController : MonoBehaviour
{
    private bool Password = false;

    /// <summary>
    /// 脚本加载
    /// </summary>
    private void Awake()
    {
        GameObject LoginPage = GameObject.Find("LoginPage");
        LoginPage.transform.SetParent(GameObject.Find("Canvas").transform);
        LoginPage.GetComponent<RectTransform>().sizeDelta = new Vector2(BasicInformation.ScreenWidth, BasicInformation.ScreenHeight);
        LoginPage.GetComponent<RectTransform>().localPosition = new Vector2(BasicInformation.ScreenWidth, 0);
        LoginPage.transform.DOMoveX(BasicInformation.ScreenWidth / 2, 0.1f);
        GameObject.Find("LoginPage/Return").GetComponent<Button>().onClick.AddListener(() => { OnReturnClicked(); });
        GameObject.Find("LoginPage/LoginArea/Submit").GetComponent<Button>().onClick.AddListener(() => { OnLoginClicked(); });
        GameObject.Find("LoginPage/LoginArea/Regist").GetComponent<Button>().onClick.AddListener(() => { OnRegistClicked(); });
        GameObject.Find("LoginPage/LoginArea/Password/ShowPassword").GetComponent<Button>().onClick.AddListener(() => { OnShowPsClicked(); });
        GameObject.Find("LoginPage/LoginArea/FindPass").GetComponent<Button>().onClick.AddListener(() => { OnFindPsClicked(); });
    }

    /// <summary>
    /// 返回
    /// </summary>
    public void OnReturnClicked()
    {
        GameObject x = GameObject.Find("LoginPage");
        x.transform.DOMoveX(3 * BasicInformation.ScreenWidth / 2, 0.1f).OnComplete(() => { Destroy(x); });
    }

    /// <summary>
    /// 显示密码
    /// </summary>
    public void OnShowPsClicked()
    {
        InputField inputField = GameObject.Find("LoginPage/LoginArea/Password").GetComponent<InputField>();
        Image image = GameObject.Find("LoginPage/LoginArea/Password/ShowPassword").GetComponent<Image>();
        inputField.contentType = Password ? InputField.ContentType.Password : InputField.ContentType.Standard;
        inputField.Select();
        image.color = Password ? Color.gray : new Color(0f,0f,0.5f);
        Password = !Password;
    }

    /// <summary>
    /// 登录
    /// </summary>
    public void OnLoginClicked()
    {
        Text EmailNote = GameObject.Find("LoginPage/LoginArea/EmailNote").GetComponent<Text>(),
            PassNote = GameObject.Find("LoginPage/LoginArea/PassNote").GetComponent<Text>();
        string Email = GameObject.Find("LoginPage/LoginArea/Email").GetComponent<InputField>().text;
        string Password = GameObject.Find("LoginPage/LoginArea/Password").GetComponent<InputField>().text;
        TextValidator validator = new TextValidator();
        bool a = validator.EmailTest(Email), b = validator.PasswordTest(Password);
        if (a && b)
        {
            EmailNote.text = PassNote.text = "";
            StartCoroutine(BasicInformation.UserInterface.IF03(Email, Password));
        }
        else
        {
            if (Email == "")
                EmailNote.text = "邮箱不能为空";
            else
                EmailNote.text = a ? "" : "邮箱格式错误";
            if (Password == "")
                PassNote.text = "密码不能为空";
            else
                PassNote.text = b ? "" : "密码需为8-16位";
        }
    }

    /// <summary>
    /// 跳转注册界面
    /// </summary>
    public void OnRegistClicked()
    {
        GameObject RegistPage = (GameObject)Instantiate(Resources.Load("Prefabs/UserModule/RegisterPage"));
        RegistPage.name = "RegisterPage";
        RegistPage.AddComponent<RegisterController>();
    }

    /// <summary>
    /// 跳转找回密码界面
    /// </summary>
    public void OnFindPsClicked()
    {
        GameObject FindPage = (GameObject)Instantiate(Resources.Load("Prefabs/UserModule/PasswordFindPage1"));
        FindPage.name = "PasswordFindPage1";
        FindPage.AddComponent<FindPassController>();
        FindPage.SendMessage("GetMsg", 1);
    }
}
