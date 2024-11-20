using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Utility.ClassManager;

public class TooltipManager : MonoBehaviour {

    public static TooltipManager Instance { get; private set; }
    private readonly float TOOLTIP_DELAY_SECONDS = 0.75f;
    [SerializeField] private GameObject tooltipPrefab;
    public bool IsShowingTooltip { get; private set; } = false;

    public void Start() {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    //Whole tooltip system rework?
    public void StartTooltipCountdown(TooltipableGameObject tooltipableScript) {
        // if (IsShowingTooltip) return;
        if (tooltipableScript.TooltipMessage == "") return;

        StartCoroutine(StartTooltipCountdownCoroutine(tooltipableScript));
    }
    private IEnumerator StartTooltipCountdownCoroutine(TooltipableGameObject tooltipableScript) {
        // GameObject tooltipableGameObject = tooltipableScript.gameObject;
        float counter = 0;
        while (counter < TOOLTIP_DELAY_SECONDS) {
            if (IsShowingTooltip) {
                yield return null;
                continue;
            }
            counter += Time.deltaTime;
            if (tooltipableScript.ShowTooltipCondition()) yield return null;
            else {
                IsShowingTooltip = false;
                yield break;
            }
        }
        if (tooltipableScript.ShowTooltipCondition()) tooltipableScript.TooltipGameObject = ShowTooltipOnGameObject(tooltipableScript);

    }

    public GameObject ShowTooltipOnGameObject(TooltipableGameObject tooltipableGameObject) {
        IsShowingTooltip = true;
        GameObject tooltipGameObject = Instantiate(tooltipPrefab, GetCanvasGameObject().transform);
        tooltipGameObject.transform.GetChild(0).GetComponent<Text>().text = tooltipableGameObject.TooltipMessage;
        tooltipGameObject.GetComponent<RectTransform>().position = new Vector3(Input.mousePosition.x - 10, Input.mousePosition.y - 48, 0);
        StartCoroutine(MakeTooltipFollowCursor(tooltipableGameObject, tooltipGameObject));
        return tooltipGameObject;
    }

    IEnumerator MakeTooltipFollowCursor(TooltipableGameObject tooltipedGameObjectscript, GameObject tooltip) {
        while (true) {
            if (tooltipedGameObjectscript.ShowTooltipCondition() && tooltip != null) {
                tooltip.GetComponent<RectTransform>().position = new Vector3(Input.mousePosition.x - 10, Input.mousePosition.y - 48, 0);
                yield return null;
            }
            else {
                IsShowingTooltip = false;
                Destroy(tooltip);
                yield break;
            }

        }
    }
}
