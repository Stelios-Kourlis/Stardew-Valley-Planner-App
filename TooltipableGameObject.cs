using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using static Utility.ClassManager; 

public abstract class TooltipableGameObject : MonoBehaviour, IPointerEnterHandler{
    public abstract string TooltipMessage {get;}
    public abstract void OnAwake();
    public abstract void OnUpdate();
    private GraphicRaycaster graphicRaycaster;

    public void Awake(){
        graphicRaycaster = GetComponent<GraphicRaycaster>();
        if (graphicRaycaster == null){graphicRaycaster = gameObject.AddComponent<GraphicRaycaster>();}
        Debug.Log("TooltipableGameObject Awake");

        OnAwake();
    }

    public virtual bool ShowTooltipCondition(){
        GraphicRaycaster graphicRaycaster = GetComponent<GraphicRaycaster>();
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current){
            position = Input.mousePosition
            };
        List<RaycastResult> results = new List<RaycastResult>();
        graphicRaycaster.Raycast(pointerEventData, results);
        return results.Exists(result => result.gameObject == gameObject);
    }

    protected void StartTooltipCountdown(){
        if (TooltipMessage == "") return;
        if (GetNotificationManager().IsShowingTooltip) return;
        GetNotificationManager().StartTooltipCountdown(this);
    }
    public void OnPointerEnter(PointerEventData eventData){
        StartTooltipCountdown();
    }

    public void Update(){
        if (ShowTooltipCondition()) StartTooltipCountdown();
        OnUpdate();
    }
}
