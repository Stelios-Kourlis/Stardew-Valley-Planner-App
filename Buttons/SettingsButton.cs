using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Utility.ClassManager;

public class SettingsButton : MonoBehaviour {
    private GameObject settingsModal;
    public bool SettingsModalIsOpen {get; set;} = false;
    private bool isMoving;
    private readonly float moveScale = 1000f;
    void Awake() {
        settingsModal = GameObject.FindGameObjectWithTag("SettingsModal");
        // Debug.Log(settingsModal);
        settingsModal.transform.position = new Vector3(0 + 960, 1100 + 540, 0);
    }

    public void ToggleSettingsModal() {
        if (isMoving) return;
        if (SettingsModalIsOpen) StartCoroutine(CloseSettingsModal());
        else StartCoroutine(OpenSettingsModal());
    }

    public IEnumerator OpenSettingsModal(){
        if (settingsModal == null) settingsModal = GameObject.FindGameObjectWithTag("SettingsModal");
        isMoving = true;
        while (settingsModal.transform.position.y > Screen.height/2){
            settingsModal.transform.position -= new Vector3(0, moveScale * Time.deltaTime, 0);
            yield return null;
        }
        SettingsModalIsOpen = true;
        isMoving = false;
    }

    public IEnumerator CloseSettingsModal(){
        if (settingsModal == null) settingsModal = GameObject.FindGameObjectWithTag("SettingsModal");
        isMoving = true;
        while (settingsModal.transform.position.y < Screen.height + 500){
            settingsModal.transform.position += new Vector3(0, moveScale * Time.deltaTime, 0);
            yield return null;
        }
        SettingsModalIsOpen = false;
        isMoving = false;
    }
}
