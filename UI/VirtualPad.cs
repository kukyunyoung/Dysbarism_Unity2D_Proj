using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.GlobalIllumination;

public class VirtualPad : MonoBehaviour
    //,IBeginDragHandler, IEndDragHandler, IDragHandler//IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] TextMeshProUGUI stateText;

    public Vector2 dir;

    float maxLength = 70;

    RectTransform myTr;
    PlayerMove player;
    SpriteRenderer playerSprite;

    Vector2 defPos;
    Vector2 downPos;

    private void Awake()
    {
        Application.targetFrameRate = 45;
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMove>();
        playerSprite = GameObject.FindGameObjectWithTag("Player").GetComponent<SpriteRenderer>();
        
        myTr = GetComponent<RectTransform>();
        defPos = myTr.localPosition;
    }
}
