using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryHeader : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [SerializeField] Transform invenTr;

    Vector2 beginPoint;
    Vector2 moveBegin;

    private void OnEnable()
    {
        invenTr = transform.parent;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        print("onpointerdown");
        beginPoint = invenTr.position;
        moveBegin = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        print("ondrag");
        invenTr.position = beginPoint + (eventData.position - moveBegin);
    }
}
