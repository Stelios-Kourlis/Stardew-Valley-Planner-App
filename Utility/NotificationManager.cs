using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using static Utility.ClassManager;

public class NotificationManager : MonoBehaviour{

    private class Notification{
        public readonly GameObject notificationGameObject;
        public readonly float durationAliveSeconds;

        public Notification(GameObject notificationGameObject){
            this.notificationGameObject = notificationGameObject;
            durationAliveSeconds = 0;
        }
    }

    private int MAX_POSSIBLE_NOTIFICATIONS = 5;
    private float NOTIFICATION_LIFETIME_SECONDS = 5;

    private List<Notification> notifications = new List<Notification>();

    public void SendNotification(string message){
        GameObject notificationGameObject = Resources.Load("UI/Notification") as GameObject;
        notificationGameObject = Instantiate(notificationGameObject, GetCanvasGameObject().transform);
        notificationGameObject.transform.GetChild(0).GetComponent<Text>().text = message;
        Notification notification = new Notification(notificationGameObject);
        notifications.Insert(0, notification);
         notificationGameObject.GetComponent<Button>().onClick.AddListener(() => {
            Destroy(notificationGameObject);
            notifications.Remove(notification);
            OnNotificationChanged();
        });
        OnNotificationChanged();
        StartCoroutine(StartLimetimeCountdown(notification));
    }

    private void OnNotificationChanged(){
        if (notifications.Count > MAX_POSSIBLE_NOTIFICATIONS){
            Destroy(notifications.Last().notificationGameObject);
            notifications.Remove(notifications.Last());
        }
        foreach (Notification notification in notifications){
            GameObject notificationGameObject = notification.notificationGameObject;
            int index = notifications.IndexOf(notification);
            // Debug.LogWarning(-Screen.width/2);
            notificationGameObject.GetComponent<RectTransform>().localPosition = new Vector3(-Screen.width/2, notificationGameObject.GetComponent<RectTransform>().sizeDelta.y * index - Screen.height/2, 0);
            // Debug.LogWarning(notificationGameObject.GetComponent<RectTransform>().localPosition);
        }
    }

    IEnumerator StartLimetimeCountdown(Notification notification){
        yield return new WaitForSeconds(NOTIFICATION_LIFETIME_SECONDS);
        Destroy(notification.notificationGameObject);
        notifications.Remove(notification);;
        OnNotificationChanged();
    }


}

