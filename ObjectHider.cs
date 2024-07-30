using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ObjectHider : MonoBehaviour, IPointerEnterHandler {

    private readonly float hideoutTimer = 3f;
    private readonly Vector3[] childPositions = new Vector3[5];

    public void Start() {
        if (GetComponent<GraphicRaycaster>() == null) gameObject.AddComponent<GraphicRaycaster>();
        for (int i = 0; i < transform.childCount; i++) {
            childPositions[i] = transform.GetChild(i).position;
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        StartCoroutine(StartObjectHideTimer());
    }

    public virtual bool HoverCondition() {
        GraphicRaycaster graphicRaycaster = GetComponent<GraphicRaycaster>();
        PointerEventData pointerEventData = new(EventSystem.current) {
            position = Input.mousePosition
        };
        List<RaycastResult> results = new();
        graphicRaycaster.Raycast(pointerEventData, results);
        return results.Exists(result => result.gameObject == gameObject || result.gameObject.transform.IsChildOf(transform));
    }

    IEnumerator StartObjectHideTimer() {
        float timer = 0;
        while (timer < hideoutTimer) {
            if (!HoverCondition()) yield break;
            timer += Time.deltaTime;
            yield return null;
        }
        BuildingController.SetCurrentAction(TooltipableGameObject.ActionBeforeEnteringSettings);
        for (int i = 0; i < transform.childCount; i++) {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        while (HoverCondition()) {
            yield return null;
        }
        for (int i = 0; i < transform.childCount; i++) {
            transform.GetChild(i).gameObject.SetActive(true);
        }
    }
}
