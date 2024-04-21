using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Utility.ClassManager;

public class SettingsButton : MonoBehaviour {
    private GameObject settingsModal;
    private float moveScale = -10f;
    void Start() {
        settingsModal = GameObject.FindGameObjectWithTag("SettingsModal");
        settingsModal.transform.position = new Vector3(0 + 960, 1100 + 540, 0);
    }

    public void ToggleSettingsModal() {
        StartCoroutine(OnClickCour());
    }

    IEnumerator OnClickCour() {
        for (int i = 0; i < 110; i++) {
            settingsModal.transform.position = new Vector3(settingsModal.transform.position.x, settingsModal.transform.position.y + moveScale, settingsModal.transform.position.z);
            yield return null;
        }
        moveScale = -moveScale;
        if (settingsModal.transform.position.y == 540) GetInputHandler().IsSearching = true;
        else GetInputHandler().IsSearching = false;
        // Debug.Log(settingsModal.transform.position.y);
        // Debug.Log(GetInputHandler().IsSearching);
    }
}
