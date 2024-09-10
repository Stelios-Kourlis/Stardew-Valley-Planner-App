using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Utility.ClassManager;

public class Notification : MonoBehaviour {
    private float durationAliveSeconds;
    private float maxLifetimeSeconds;

    public void Start() {
        durationAliveSeconds = 0;
    }

    public Notification InitializeParameters(float maxLifetimeSeconds, string message, Sprite icon) {
        GetComponent<Button>().onClick.AddListener(() => { Destroy(gameObject); });

        gameObject.transform.Find("TextBackground").Find("Text").GetComponent<Text>().text = message;
        gameObject.transform.Find("Square").Find("Icon").GetComponent<Image>().sprite = icon;

        this.maxLifetimeSeconds = maxLifetimeSeconds;
        StartCoroutine(StartLimetimeCountdown());
        return this;
    }

    private IEnumerator StartLimetimeCountdown() {
        while (durationAliveSeconds < maxLifetimeSeconds) {
            durationAliveSeconds += Time.deltaTime;
            GameObject progressCircle = gameObject.transform.Find("Square").Find("Progress").gameObject;
            progressCircle.GetComponent<Image>().fillAmount = durationAliveSeconds / maxLifetimeSeconds;
            yield return null;
        }
        Destroy(gameObject);
    }

    public void OnDestroy() {
        NotificationManager.Instance.notifications.Remove(this);
        StopAllCoroutines();
    }
}
