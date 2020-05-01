using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TodoController : MonoBehaviour
{
    float ContentHeight = 25f, ItemPos = -25f;

    GameObject Content;

    TodoList TodoList;

    /// <summary>
    /// 脚本加载
    /// </summary>
    private void Awake()
    {
        Content = GameObject.Find("TodoPage/Scroll/View/Content");
        GameObject CntDownPage;
        CntDownPage = GameObject.Find("TodoPage");
        CntDownPage.transform.SetParent(GameObject.Find("Canvas").transform);
        CntDownPage.GetComponent<RectTransform>().sizeDelta = new Vector2(BasicInformation.ScreenWidth, BasicInformation.ScreenHeight);
        CntDownPage.GetComponent<RectTransform>().localPosition = new Vector2(BasicInformation.ScreenWidth, 0);
        CntDownPage.transform.DOMoveX(BasicInformation.ScreenWidth / 2, 0.1f);
        GameObject.Find("TodoPage/Return").GetComponent<Button>().onClick.AddListener(() => { OnReturnClicked(); });
        GameObject.Find("TodoPage/Add").GetComponent<Button>().onClick.AddListener(() => { OnAddClicked(); });
        GameObject.Find("TodoPage/Delete").GetComponent<Button>().onClick.AddListener(() => { OnDeleteClicked(); });
    }

    /// <summary>
    /// 接收待办清单
    /// </summary>
    public void GetTodoList(TodoList list)
    {
        TodoList = list;
        GameObject.Find("TodoPage/Name").GetComponent<Text>().text = TodoList.Name;
        RefreshItems();
    }

    /// <summary>
    /// 接收新条目
    /// </summary>
    public void GetNewItem(TodoListItem item)
    {
        TodoList.Items.Add(item);
        RefreshItems();
    }

    /// <summary>
    /// 接收删除条目id
    /// </summary>
    /// <param name="id"></param>
    public void GetDeleteId(int id)
    {
        for (int i = 0; i < TodoList.Items.Count; i++)
            if (TodoList.Items[i].Id == id)
            {
                TodoList.Items.RemoveAt(i);
                break;
            }
        RefreshItems();
    }

    /// <summary>
    /// 接收修改条目id
    /// </summary>
    public void GetChangedId(int[] nums)
    {
        for (int i = 0; i < TodoList.Items.Count; i++)
            if (TodoList.Items[i].Id == nums[0])
            {
                TodoList.Items[i].Prior = nums[1];
                break;
            }
        RefreshItems();
    }

    /// <summary>
    /// 接收勾选条目id
    /// </summary>
    /// <param name="arrayList"></param>
    public void GetCheckedId(ArrayList arrayList)
    {
        for (int i = 0; i < TodoList.Items.Count; i++)
            if (TodoList.Items[i].Id == (int)arrayList[0])
            {
                TodoList.Items[i].IsChecked = (bool)arrayList[1];
                break;
            }
        RefreshItems();
    }

    /// <summary>
    /// 返回
    /// </summary>
    private void OnReturnClicked()
    {
        GameObject x = GameObject.Find("TodoPage");
        x.transform.DOMoveX(3 * BasicInformation.ScreenWidth / 2, 0.1f).OnComplete(() => { Destroy(x); });
    }

    /// <summary>
    /// 点击条目
    /// </summary>
    private void OnPriorClicked(int id)
    {
        for (int i = 0; i < TodoList.Items.Count; i++)
            if (TodoList.Items[i].Id == id)
            {
                StartCoroutine(BasicInformation.OtherInterface.IF08(id, (TodoList.Items[i].Prior + 1) % 3));
                break;
            }

    }

    /// <summary>
    /// 勾选条目
    /// </summary>
    private void OnCheckClicked(int id, bool ison)
    {
        if (ison)
            StartCoroutine(BasicInformation.OtherInterface.IF06(id));
        else
            StartCoroutine(BasicInformation.OtherInterface.IF07(id));
    }

    /// <summary>
    /// 添加条目
    /// </summary>
    private void OnAddClicked()
    {
        GameObject CreateTodoItemPage = (GameObject)Instantiate(Resources.Load("Prefabs/OtherModule/CreateTodoItemPage"));
        CreateTodoItemPage.name = "CreateTodoItemPage~";
        CreateTodoItemPage.AddComponent<AddListItemController>();
        CreateTodoItemPage.SendMessage("GetListId", TodoList.Id);
    }

    /// <summary>
    /// 删除清单
    /// </summary>
    private void OnDeleteClicked()
    {
        GameObject Warning = (GameObject)Instantiate(Resources.Load("Prefabs/Warning"));
        Warning.name = "Warning";
        Warning.transform.SetParent(GameObject.Find("Canvas").transform);
        Warning.GetComponent<RectTransform>().sizeDelta = new Vector2(BasicInformation.ScreenWidth, BasicInformation.ScreenHeight);
        Warning.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
        GameObject.Find("Warning/Info/Text").GetComponent<Text>().text = "即将删除清单，是否确认？";
        GameObject.Find("Warning/Info/Submit").GetComponent<Button>().onClick.AddListener(() => {
            Destroy(Warning);
            StartCoroutine(BasicInformation.OtherInterface.IF10(TodoList.Id));
        });
        GameObject.Find("Warning/Info/Cancel").GetComponent<Button>().onClick.AddListener(() => {
            Destroy(Warning);
        });
    }

    /// <summary>
    /// 删除条目
    /// </summary>
    private void OnDeleteItemClicked(int id)
    {
        GameObject Warning = (GameObject)Instantiate(Resources.Load("Prefabs/Warning"));
        Warning.name = "Warning";
        Warning.transform.SetParent(GameObject.Find("Canvas").transform);
        Warning.GetComponent<RectTransform>().sizeDelta = new Vector2(BasicInformation.ScreenWidth, BasicInformation.ScreenHeight);
        Warning.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
        GameObject.Find("Warning/Info/Text").GetComponent<Text>().text = "即将删除待办内容，是否确认？";
        GameObject.Find("Warning/Info/Submit").GetComponent<Button>().onClick.AddListener(() => {
            Destroy(Warning);
            StartCoroutine(BasicInformation.OtherInterface.IF09(id));
        });
        GameObject.Find("Warning/Info/Cancel").GetComponent<Button>().onClick.AddListener(() => {
            Destroy(Warning);
        });
    }

    /// <summary>
    /// 刷新界面
    /// </summary>
    private void RefreshItems()
    {
        TodoList.Items.Sort((left, right) =>
        {
            if (left.Prior < right.Prior)
                return 1;
            else if (left.Prior == right.Prior)
                return 0;
            else
                return -1;
        });
        while (Content.transform.childCount > 0)
            DestroyImmediate(Content.transform.GetChild(0).gameObject);
        if (TodoList.Items.Count == 0)
        {
            GameObject.Find("TodoPage/Scroll/Warn").GetComponent<CanvasGroup>().alpha = 1;
            return;
        }
        ContentHeight = 25f;
        ItemPos = -25f;
        ContentHeight += 175 * TodoList.Items.Count;
        Content.GetComponent<RectTransform>().sizeDelta = new Vector2(BasicInformation.ScreenWidth, ContentHeight);
        for (int i = 0; i < TodoList.Items.Count; i++)
        {
            GameObject TodoItem;
            TodoItem = (GameObject)Instantiate(Resources.Load("Prefabs/OtherModule/TodoItem"));
            TodoItem.name = "TodoItem" + i;
            TodoItem.transform.SetParent(GameObject.Find("TodoPage/Scroll/View/Content").transform);
            TodoItem.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, ItemPos);
            TodoItem.GetComponent<RectTransform>().sizeDelta = new Vector2(0.96f * BasicInformation.ScreenWidth, 150f);
            GameObject.Find("TodoPage/Scroll/View/Content/" + TodoItem.name + "/Content").GetComponent<Text>().text = TodoList.Items[i].Content;
            GameObject.Find("TodoPage/Scroll/View/Content/" + TodoItem.name + "/Check").GetComponent<Toggle>().isOn = TodoList.Items[i].IsChecked;
            if (TodoList.Items[i].Prior == 0)
                GameObject.Find("TodoPage/Scroll/View/Content/" + TodoItem.name + "/Prior").GetComponent<Image>().sprite = Resources.Load<Sprite>("Pictures/Icons/fire1");
            else if (TodoList.Items[i].Prior == 1)
                GameObject.Find("TodoPage/Scroll/View/Content/" + TodoItem.name + "/Prior").GetComponent<Image>().sprite = Resources.Load<Sprite>("Pictures/Icons/fire2");
            else
                GameObject.Find("TodoPage/Scroll/View/Content/" + TodoItem.name + "/Prior").GetComponent<Image>().sprite = Resources.Load<Sprite>("Pictures/Icons/fire3");
            TodoListItem s = TodoList.Items[i];
            GameObject.Find("TodoPage/Scroll/View/Content/" + TodoItem.name + "/Prior").GetComponent<Button>().onClick.AddListener(() => { OnPriorClicked(s.Id); });
            GameObject.Find("TodoPage/Scroll/View/Content/" + TodoItem.name + "/Check").GetComponent<Toggle>().onValueChanged.AddListener((ison) => { OnCheckClicked(s.Id, ison); });
            GameObject.Find("TodoPage/Scroll/View/Content/" + TodoItem.name).GetComponent<Button>().onClick.AddListener(() => { OnDeleteItemClicked(s.Id); });
            ItemPos = ItemPos - 165f;
        }
    }
}
