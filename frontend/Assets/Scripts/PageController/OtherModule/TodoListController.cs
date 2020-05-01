using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TodoListController : MonoBehaviour
{
    float ContentHeight = 25f, ItemPos = -25f;

    GameObject Content;

    List<TodoList> TodoList;

    /// <summary>
    /// 脚本加载
    /// </summary>
    private void Awake()
    {
        Content = GameObject.Find("TodoListPage/Scroll/View/Content");
        GameObject CntDownPage;
        CntDownPage = GameObject.Find("TodoListPage");
        CntDownPage.transform.SetParent(GameObject.Find("Canvas").transform);
        CntDownPage.GetComponent<RectTransform>().sizeDelta = new Vector2(BasicInformation.ScreenWidth, BasicInformation.ScreenHeight);
        CntDownPage.GetComponent<RectTransform>().localPosition = new Vector2(BasicInformation.ScreenWidth, 0);
        CntDownPage.transform.DOMoveX(BasicInformation.ScreenWidth / 2, 0.1f);
        GameObject.Find("TodoListPage/Return").GetComponent<Button>().onClick.AddListener(() => { OnReturnClicked(); });
        GameObject.Find("TodoListPage/Add").GetComponent<Button>().onClick.AddListener(() => { OnAddClicked(); });
    }

    /// <summary>
    /// 接收待办清单列表
    /// </summary>
    public void GetTodoLists(List<TodoList> lists)
    {
        TodoList = lists;
        RefreshList();
    }

    /// <summary>
    /// 接收新建待办清单
    /// </summary>
    public void GetNewList(TodoList list)
    {
        TodoList.Add(list);
        RefreshList();
    }

    /// <summary>
    /// 接收删除清单id
    /// </summary>
    public void GetDeleteId(int id)
    {
        for(int i=0;i<TodoList.Count;i++)
            if (TodoList[i].Id == id)
            {
                TodoList.RemoveAt(i);
                break;
            }
        RefreshList();
    }

    /// <summary>
    /// 返回
    /// </summary>
    private void OnReturnClicked()
    {
        GameObject x = GameObject.Find("TodoListPage");
        x.transform.DOMoveX(3 * BasicInformation.ScreenWidth / 2, 0.1f).OnComplete(() => { Destroy(x); });
    }

    /// <summary>
    /// 点击清单
    /// </summary>
    private void OnTodoListClicked(TodoList list)
    {
        GameObject TodoPage = (GameObject)Instantiate(Resources.Load("Prefabs/OtherModule/TodoPage"));
        TodoPage.name = "TodoPage";
        TodoPage.AddComponent<TodoController>();
        TodoPage.SendMessage("GetTodoList", list);
    }

    /// <summary>
    /// 添加倒计时
    /// </summary>
    private void OnAddClicked()
    {
        GameObject CreateTodoListPage = (GameObject)Instantiate(Resources.Load("Prefabs/OtherModule/CreateTodoListPage"));
        CreateTodoListPage.name = "CreateTodoListPage~";
        CreateTodoListPage.AddComponent<AddListController>();
    }

    /// <summary>
    /// 刷新界面
    /// </summary>
    private void RefreshList()
    {
        while (Content.transform.childCount > 0)
            DestroyImmediate(Content.transform.GetChild(0).gameObject);
        if (TodoList.Count == 0)
        {
            GameObject.Find("EventSystem").SendMessage("GetTodoCount", 3);
            GameObject.Find("TodoListPage/Scroll/Warn").GetComponent<CanvasGroup>().alpha = 1;
            return;
        }
        TodoList.Sort((left, right) =>
        {
            if (left.GetUnDone() < right.GetUnDone())
                return 1;
            else if (left.GetUnDone() == right.GetUnDone())
                return 0;
            else
                return -1;
        });
        if (TodoList.Count >= 2)
        {
            GameObject.Find("EventSystem").SendMessage("GetTodoCount", 2);
            GameObject.Find("EventSystem").SendMessage("GetTodo1", new ArrayList() {TodoList[0].Name, TodoList[0].GetUnDone() });
            GameObject.Find("EventSystem").SendMessage("GetTodo2", new ArrayList() { TodoList[1].Name, TodoList[1].GetUnDone() });
        }
        else if (TodoList.Count == 1)
        {
            GameObject.Find("EventSystem").SendMessage("GetTodoCount", 1);
            GameObject.Find("EventSystem").SendMessage("GetTodo1", new ArrayList() { TodoList[0].Name, TodoList[0].GetUnDone() });
        }
        ContentHeight = 25f;
        ItemPos = -25f;
        ContentHeight += 225 * TodoList.Count;
        Content.GetComponent<RectTransform>().sizeDelta = new Vector2(BasicInformation.ScreenWidth, ContentHeight);
        for (int i = 0; i < TodoList.Count; i++)
        {
            GameObject TodoListItem;
            TodoListItem = (GameObject)Instantiate(Resources.Load("Prefabs/OtherModule/TodoListItem"));
            TodoListItem.name = "TodoListItem" + i;
            TodoListItem.transform.SetParent(GameObject.Find("TodoListPage/Scroll/View/Content").transform);
            TodoListItem.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, ItemPos);
            TodoListItem.GetComponent<RectTransform>().sizeDelta = new Vector2(0.96f * BasicInformation.ScreenWidth, 200f);
            GameObject.Find("TodoListPage/Scroll/View/Content/" + TodoListItem.name + "/Name").GetComponent<Text>().text = TodoList[i].Name;
            GameObject.Find("TodoListPage/Scroll/View/Content/" + TodoListItem.name + "/Count").GetComponent<Text>().text = TodoList[i].GetUnDone() + "条待办";
            TodoList s = TodoList[i];
            GameObject.Find("TodoListPage/Scroll/View/Content/" + TodoListItem.name).GetComponent<Button>().onClick.AddListener(() => { OnTodoListClicked(s); });
            ItemPos = ItemPos - 225f;
        }
    }
}