using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Playables;
using UnityEngine.U2D;
using UnityEngine.UI;
using static Utility.ClassManager;

public class NotificationManager : MonoBehaviour {

    public enum Icons {
        ErrorIcon,
        SaveIcon,
        CheckIcon,
        InfoIcon
    }

    private readonly int MAX_POSSIBLE_NOTIFICATIONS = 5;
    private readonly float NOTIFICATION_LIFETIME_SECONDS = 5;
    private readonly float TOOLTIP_DELAY_SECONDS = 0.75f;
    public bool IsShowingTooltip { get; private set; } = false;

    public readonly List<Notification> notifications = new();
    private SpriteAtlas spriteAtlas;
    private GameObject notificationPrefab;

    public void Start() {
        spriteAtlas = Resources.Load<SpriteAtlas>("UI/NotificationIconsAtlas");
        notificationPrefab = Resources.Load("UI/Notification") as GameObject;
    }

    public void SendNotification(string message, Icons icon) {
        GameObject notificationGameObject = Instantiate(notificationPrefab, GetCanvasGameObject().transform.Find("NotificationArea"));
        Notification notification = notificationGameObject.GetComponent<Notification>().InitializeParameters(NOTIFICATION_LIFETIME_SECONDS, message, spriteAtlas.GetSprite(icon.ToString()));
        notifications.Add(notification);
        OnNotificationAdded();
    }

    private void OnNotificationAdded() {
        if (notifications.Count > MAX_POSSIBLE_NOTIFICATIONS) {
            Destroy(notifications.Last().gameObject);
        }
    }




    //Move this to another class?

    public GameObject ShowTooltipOnGameObject(TooltipableGameObject tooltipableGameObject) {
        GameObject tooltipGameObject = Resources.Load("UI/Tooltip") as GameObject;
        tooltipGameObject = Instantiate(tooltipGameObject, GetCanvasGameObject().transform);
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

    public void StartTooltipCountdown(TooltipableGameObject tooltipableScript) {
        if (IsShowingTooltip) return;
        if (tooltipableScript.TooltipMessage == "") return;
        // Debug.Log($"isShowingTooltip: {IsShowingTooltip}, TooltipMessage: {tooltipableScript.TooltipMessage}, can start");
        IsShowingTooltip = true;
        StartCoroutine(StartTooltipCountdownCoroutine(tooltipableScript));
    }
    private IEnumerator StartTooltipCountdownCoroutine(TooltipableGameObject tooltipableScript) {
        // GameObject tooltipableGameObject = tooltipableScript.gameObject;
        float counter = 0;
        while (counter < TOOLTIP_DELAY_SECONDS) {
            counter += Time.deltaTime;
            if (tooltipableScript.ShowTooltipCondition()) yield return null;
            else {
                IsShowingTooltip = false;
                yield break;
            }
        }
        if (tooltipableScript.ShowTooltipCondition()) tooltipableScript.TooltipGameObject = ShowTooltipOnGameObject(tooltipableScript);

    }


}

