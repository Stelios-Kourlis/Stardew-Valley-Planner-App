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

    public static NotificationManager Instance { get; private set; }

    private readonly int MAX_POSSIBLE_NOTIFICATIONS = 5;
    private readonly float NOTIFICATION_LIFETIME_SECONDS = 5;

    public readonly List<Notification> notifications = new();
    [SerializeField] private SpriteAtlas spriteAtlas;
    [SerializeField] private GameObject notificationPrefab;

    public void Start() {
        if (Instance == null) Instance = this;
        else Destroy(this);
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
}

