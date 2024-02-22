using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsButton : MonoBehaviour {
    //private Button settingsButton;
    private GameObject settingsModal;
    private float moveScale = -10f;
    // Start is called before the first frame update
    void Start() {
        //settingsButton = gameObject.GetComponent<Button>();
        settingsModal = GameObject.FindGameObjectWithTag("SettingsModal");
        settingsModal.transform.position = new Vector3(0 + 960, 1100 + 540, 0);
    }

    // Update is called once per frame
    void Update() {
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
    }
}
