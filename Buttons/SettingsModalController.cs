using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Utility.ClassManager;

public class SettingsModalController : MonoBehaviour {
    private GameObject settingsModal;
    public bool SettingsModalIsOpen {get; set;} = false;
    private bool isMoving;
    private readonly float moveScale = 1250f;
    void Awake() {
        settingsModal = gameObject;
        settingsModal.transform.position = new Vector3(0 + 960, 1100 + 540, 0);
    }

    public void ToggleSettingsModal() {
        if (isMoving) return;
        if (SettingsModalIsOpen) StartCoroutine(CloseSettingsModal());
        else StartCoroutine(OpenSettingsModal());
    }

    public IEnumerator OpenSettingsModal(){
        // StopCoroutine(CloseSettingsModal());
        if (settingsModal == null) settingsModal = GameObject.FindGameObjectWithTag("SettingsModal");
        isMoving = true;
        SettingsModalIsOpen = true;
        while (settingsModal.transform.position.y > Screen.height/2){
            // Debug.Log("Closings");
            settingsModal.transform.position -= new Vector3(0, moveScale * Time.deltaTime, 0);
            yield return null;
        }
        
        isMoving = false;
    }

    public IEnumerator CloseSettingsModal(){
        // StopCoroutine(OpenSettingsModal());
        if (settingsModal == null) settingsModal = GameObject.FindGameObjectWithTag("SettingsModal");
        isMoving = true;
        SettingsModalIsOpen = false;
        while (settingsModal.transform.position.y < Screen.height + 500){
            // Debug.Log("Opening");
            settingsModal.transform.position += new Vector3(0, moveScale * Time.deltaTime, 0);
            yield return null;
        }
        
        isMoving = false;
    }
}
