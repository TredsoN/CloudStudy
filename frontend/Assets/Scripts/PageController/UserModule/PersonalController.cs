using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class PersonalController : MonoBehaviour
{
    /// <summary>
    /// 脚本加载
    /// </summary>
    private void Awake()
    {
        GameObject PersonalPage = GameObject.Find("PersonalPage");
        PersonalPage.transform.SetParent(GameObject.Find("Canvas").transform);
        PersonalPage.GetComponent<RectTransform>().sizeDelta = new Vector2(BasicInformation.ScreenWidth, BasicInformation.ScreenHeight);
        PersonalPage.GetComponent<RectTransform>().localPosition = new Vector2(BasicInformation.ScreenWidth, 0);
        GameObject.Find("PersonalPage/PersonArea/View/Content/HeadShot").GetComponent<RectTransform>().sizeDelta = new Vector2(BasicInformation.ScreenWidth, BasicInformation.ScreenWidth);
        GameObject.Find("PersonalPage/PersonArea/View/Content").GetComponent<RectTransform>().sizeDelta = new Vector2(0, BasicInformation.ScreenWidth + 2000f);
        PersonalPage.transform.DOMoveX(BasicInformation.ScreenWidth / 2, 0.1f);
        GameObject.Find("PersonalPage/PersonArea/View/Content/Name/Text").GetComponent<Text>().text = BasicInformation.CurUser.Name.Length > 10 ? BasicInformation.CurUser.Name.Substring(0, 10) + "..." : BasicInformation.CurUser.Name;
        GameObject.Find("PersonalPage/PersonArea/View/Content/Gender/Text").GetComponent<Text>().text = BasicInformation.CurUser.Gender == 0 ? "男" : "女";
        GameObject.Find("PersonalPage/PersonArea/View/Content/Age/Text").GetComponent<Text>().text = BasicInformation.CurUser.Age.ToString();
        GameObject.Find("PersonalPage/PersonArea/View/Content/Signature/Text").GetComponent<Text>().text = BasicInformation.CurUser.Introduce.Length > 10 ? BasicInformation.CurUser.Introduce.Substring(0, 10)+"..." : BasicInformation.CurUser.Introduce;
        GameObject.Find("PersonalPage/PersonArea/View/Content/Email/Text").GetComponent<Text>().text = BasicInformation.CurUser.Email;
        GameObject.Find("PersonalPage/PersonArea/View/Content/RegistTime/Text").GetComponent<Text>().text = BasicInformation.CurUser.RegistTime;
        GameObject.Find("PersonalPage/PersonArea/View/Content/Exit").GetComponent<Button>().onClick.AddListener(() => { OnExitClicked(); });
        GameObject.Find("PersonalPage/Return").GetComponent<Button>().onClick.AddListener(() => { OnReturnClicked(); });
        GameObject.Find("PersonalPage/PersonArea/View/Content/HeadShot/Select").GetComponent<Button>().onClick.AddListener(() => { OnChangeHeadShot(); });
        GameObject.Find("PersonalPage/PersonArea/View/Content/Name").GetComponent<Button>().onClick.AddListener(() => { OnChangeInfoClicked(0); });
        GameObject.Find("PersonalPage/PersonArea/View/Content/Gender").GetComponent<Button>().onClick.AddListener(() => { OnChangeInfoClicked(1); });
        GameObject.Find("PersonalPage/PersonArea/View/Content/Age").GetComponent<Button>().onClick.AddListener(() => { OnChangeInfoClicked(2); });
        GameObject.Find("PersonalPage/PersonArea/View/Content/Signature").GetComponent<Button>().onClick.AddListener(() => { OnChangeInfoClicked(3); });
        GameObject.Find("PersonalPage/PersonArea/View/Content/ChangePass").GetComponent<Button>().onClick.AddListener(() => { OnChangePassClicked(); });
        GameObject.Find("PersonalPage/PersonArea/View/Content/Message").GetComponent<Button>().onClick.AddListener(() => { OnMessageClicked(); });
        StartCoroutine(DownloadHeadImage(BasicInformation.CurUser.HeadShot));
    }
   
    public void GetHeadshotUpdate()
    {
        StartCoroutine(DownloadHeadImage(BasicInformation.CurUser.HeadShot));
    }

    /// <summary>
    /// 返回
    /// </summary>
    public void OnReturnClicked()
    {
        GameObject x = GameObject.Find("PersonalPage");
        x.transform.DOMoveX(3 * BasicInformation.ScreenWidth / 2, 0.1f).OnComplete(() => { Destroy(x); });
    }

    /// <summary>
    /// 退出当前账号
    /// </summary>
    public void OnExitClicked()
    {
        GameObject Warning = (GameObject)Instantiate(Resources.Load("Prefabs/Warning"));
        Warning.name = "Warning";
        Warning.transform.SetParent(GameObject.Find("Canvas").transform);
        Warning.GetComponent<RectTransform>().sizeDelta = new Vector2(BasicInformation.ScreenWidth, BasicInformation.ScreenHeight);
        Warning.GetComponent<RectTransform>().localPosition = new Vector2(0, 0); GameObject.Find("Warning/Info/Text").GetComponent<Text>().text = "是否退出当前账号？";
        GameObject.Find("Warning/Info/Submit").GetComponent<Button>().onClick.AddListener(() =>
        {
            Destroy(Warning);
            GameObject x = GameObject.Find("PersonalPage");
            x.transform.DOMoveX(3 * BasicInformation.ScreenWidth / 2, 0.1f).OnComplete(() => { Destroy(x); });
            BasicInformation.CurUser.IsOnline = false;
            BasicInformation.CurUser = new User();
            PlayerPrefs.DeleteAll();
            GameObject.Find("Page3/View/Content/UserInfo/UserName").GetComponent<Text>().text = "未登录";
            GameObject.Find("Page3/View/Content/UserInfo/TotalTime").GetComponent<Text>().text = "0";
            GameObject.Find("Page3/View/Content/HeadShot/HeadShotMask/HeadShot").GetComponent<Image>().sprite = Resources.Load<Sprite>("Pictures/Images/DefaultHead");
            GameObject.Find("Pages/Page3/View/Content/TimeStatic/Loading").GetComponent<CanvasGroup>().alpha = 0;
            GameObject.Find("Pages/Page3/View/Content/TimeStatic/Loading").transform.SetAsFirstSibling();
            GameObject.Find("Pages/Page3/View/Content/TimeStatic/ChartMask").GetComponent<CanvasGroup>().alpha = 0;
            GameObject.Find("Pages/Page3/View/Content/TimeStatic/Note").GetComponent<CanvasGroup>().alpha = 1;
            GameObject.Find("Pages/Page3/View/Content/TimeStatic2/Loading").GetComponent<CanvasGroup>().alpha = 0;
            GameObject.Find("Pages/Page3/View/Content/TimeStatic2/Loading").transform.SetAsFirstSibling();
            GameObject.Find("Pages/Page3/View/Content/TimeStatic2/Chart2").GetComponent<CanvasGroup>().alpha = 0;
            GameObject.Find("Pages/Page3/View/Content/TimeStatic2/Note").GetComponent<CanvasGroup>().alpha = 1;
            GameObject.Find("Pages/Page1/View/Content/CountDownArea/CntTile1").GetComponent<CanvasGroup>().alpha = 0;
            GameObject.Find("Pages/Page1/View/Content/CountDownArea/CntTile2").GetComponent<CanvasGroup>().alpha = 0;
            GameObject.Find("Pages/Page1/View/Content/TodoListArea/TodoTile1").GetComponent<CanvasGroup>().alpha = 0;
            GameObject.Find("Pages/Page1/View/Content/TodoListArea/TodoTile2").GetComponent<CanvasGroup>().alpha = 0;
            GameObject.Find("Pages/Page3/View/Content/TimeStatic2/Note").GetComponent<Text>().text = "请登录后查看";
            GameObject.Find("Pages/Page1/View/Content/CountDownArea/Note").GetComponent<Text>().text = "请登录后查看";
            GameObject.Find("Pages/Page1/View/Content/TodoListArea/Note").GetComponent<Text>().text = "请登录后查看";
            GameObject.Find("Pages/Page1/View/Content/RecentStar/Note").GetComponent<Text>().text = "请登录后查看";
        });
        GameObject.Find("Warning/Info/Cancel").GetComponent<Button>().onClick.AddListener(() =>
        {
            Destroy(Warning);
        });
    }

    /// <summary>
    /// 显示修改个人信息界面
    /// </summary>
    /// <param name="id">0用户名，1性别，2年龄，3签名</param>
    public void OnChangeInfoClicked(int id)
    {
        GameObject InfoChangePage = (GameObject)Instantiate(Resources.Load("Prefabs/UserModule/InfoChangePage"));
        InfoChangePage.name = "InfoChangePage~";
        InfoChangePage.AddComponent<InfoChangeController>();
        InfoChangePage.SendMessage("GetMsg", id);
    }

    /// <summary>
    /// 显示修改密码界面
    /// </summary>
    public void OnChangePassClicked()
    {
        GameObject PassChangePage = (GameObject)Instantiate(Resources.Load("Prefabs/UserModule/PassChangePage"));
        PassChangePage.name = "PassChangePage~";
        PassChangePage.AddComponent<InfoChangeController>();
        PassChangePage.SendMessage("GetMsg", 4);
    }

    /// <summary>
    /// 查看收到消息
    /// </summary>
    public void OnMessageClicked()
    {
        StartCoroutine(BasicInformation.UserInterface.IF10(BasicInformation.CurUser.Id));
    }

    /// <summary>
    /// 修改头像
    /// </summary>
    private void OnChangeHeadShot()
    {
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            if (path != null)
            {
                Texture2D source = NativeGallery.LoadImageAtPath(path, -1, false);
                Texture2D result = new Texture2D(500, 500, source.format, false);
                int h = source.height, w = source.width;
                if (w > h)
                {
                    for (int i = 0; i < result.height; ++i)
                    {
                        for (int j = 0; j < result.width; ++j)
                        {
                            Color newColor = source.GetPixel((w - h) / 2 + j * h / 500, i * h / 500);
                            result.SetPixel(j, i, newColor);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < result.height; ++i)
                    {
                        for (int j = 0; j < result.width; ++j)
                        {
                            Color newColor = source.GetPixel(j * w / 500, (h - w) / 2 + i * w / 500);
                            result.SetPixel(j, i, newColor);
                        }
                    }
                }
                result.Apply();
                StartCoroutine(BasicInformation.UserInterface.IF12(BasicInformation.CurUser.Id, result.EncodeToJPG()));
            }
        }, "Select a PNG image", "image/png");
    }

    /// <summary>
    /// 下载头像
    /// </summary>
    /// <param name="Url">地址</param>
    private IEnumerator DownloadHeadImage(string Url)
    {
        UnityWebRequest request = new UnityWebRequest(Url);
        DownloadHandlerTexture DownloadTex = new DownloadHandlerTexture(true);
        request.downloadHandler = DownloadTex;
        yield return request.SendWebRequest();
        if (request.isHttpError || request.isNetworkError)
        {
            Debug.LogError(request.error);
        }
        else
        {
            Texture2D tex = DownloadTex.texture;
            GameObject.Find("PersonalPage/PersonArea/View/Content/HeadShot").GetComponent<Image>().color = Color.white;
            GameObject.Find("PersonalPage/PersonArea/View/Content/HeadShot").GetComponent<Image>().sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        }
    }
}
