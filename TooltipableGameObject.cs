using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using static Utility.ClassManager;

public abstract class TooltipableGameObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    public abstract string TooltipMessage { get; }
    public abstract void OnAwake();
    public abstract void OnUpdate();
    public static Actions ActionBeforeEnteringSettings { get; private set; }
    public GameObject TooltipGameObject { get; set; }

    public void Awake() {
        if (!TryGetComponent(out GraphicRaycaster _)) { gameObject.AddComponent<GraphicRaycaster>(); }
        OnAwake();
    }

    public virtual bool ShowTooltipCondition() {
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
        if (GetNotificationManager().IsShowingTooltip) return;
        GetNotificationManager().StartTooltipCountdown(this);
    }
    public void OnPointerEnter(PointerEventData eventData) {
        ActionBeforeEnteringSettings = BuildingController.CurrentAction;
        // BuildingController.SetCurrentAction(Actions.DO_NOTHING);
        GetInputHandler().SetCursor(InputHandler.CursorType.Default);
        StartTooltipCountdown();
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (!(BuildingController.CurrentAction == Actions.DO_NOTHING)) return;
        // if (GetSettingsModalController().IsOpen) return; //todo fix
        // if (GetTotalMaterialsCalculator().IsOpen) return;
        // BuildingController.SetCurrentAction(ActionBeforeEnteringSettings);
    }

    public void Update() {
        if (ShowTooltipCondition()) StartTooltipCountdown();
        OnUpdate();
    }
}
