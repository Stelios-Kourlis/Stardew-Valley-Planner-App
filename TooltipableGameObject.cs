using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using static Utility.ClassManager;

public abstract class TooltipableGameObject : MonoBehaviour, IPointerEnterHandler {
    public abstract string TooltipMessage { get; }

    public GameObject TooltipGameObject { get; set; }

    public void Awake() {
        if (!TryGetComponent(out GraphicRaycaster _)) { gameObject.AddComponent<GraphicRaycaster>(); }
    }

    public virtual bool ShowTooltipCondition() {
        if (gameObject == null) return false;
        GraphicRaycaster graphicRaycaster = GetComponent<GraphicRaycaster>();
        PointerEventData pointerEventData = new(EventSystem.current) {
            position = Input.mousePosition
        };
        List<RaycastResult> results = new();
        graphicRaycaster.Raycast(pointerEventData, results);
        return results.Exists(result => result.gameObject == gameObject);
    }

    protected void StartTooltipCountdown() {
        if (TooltipMessage == "") return;
        if (TooltipManager.Instance.IsShowingTooltip) return;
        TooltipManager.Instance.StartTooltipCountdown(this);
    }
    public void OnPointerEnter(PointerEventData eventData) {
        if (ShowTooltipCondition()) StartTooltipCountdown();
    }
}
