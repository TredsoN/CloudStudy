using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollViewDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private ScrollRect anotherScrollRect;

    public bool thisIsUpAndDown = true;

    private ScrollRect thisScrollRect;

    private void Start()
    {
        thisScrollRect = GetComponent<ScrollRect>();
        anotherScrollRect = GameObject.Find("MainScroll").GetComponent<ScrollRect>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        anotherScrollRect.OnBeginDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        anotherScrollRect.OnDrag(eventData);
        float angle = Vector2.Angle(eventData.delta, Vector2.up);
        if (angle > 45f && angle < 135f)
        {
            thisScrollRect.enabled = !thisIsUpAndDown;
            anotherScrollRect.enabled = thisIsUpAndDown;
        }
        else
        {
            anotherScrollRect.enabled = !thisIsUpAndDown;
            thisScrollRect.enabled = thisIsUpAndDown;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        anotherScrollRect.OnEndDrag(eventData);
        anotherScrollRect.enabled = true;
        thisScrollRect.enabled = true;
    }
}
