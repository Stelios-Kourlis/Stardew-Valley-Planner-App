using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Utility.ClassManager;

public class SettingsModalController : MonoBehaviour, IToggleablePanel {
    private GameObject settingsModal;
    public bool IsMoving {get; private set;}
    public bool IsOpen {get; private set;}
    void Awake() {
        settingsModal = gameObject;
        settingsModal.transform.position = new Vector3(0 + 960, 1100 + 540, 0);
    }

    public void TogglePanel() {
        if (IsMoving) return;
        if (IsOpen) StartCoroutine(ClosePanel());
        else StartCoroutine(OpenPanel());
    }

    public IEnumerator OpenPanel(){
        GameObject buildingPanel = GameObject.FindGameObjectWithTag("Panel");
        StartCoroutine(buildingPanel.GetComponent<BuildingMenuController>().ClosePanel());
        IsMoving = true;
        IsOpen = true;
        while (settingsModal.transform.position.y > Screen.height/2){
            settingsModal.transform.position -= new Vector3(0, IToggleablePanel.MOVE_SCALE * Time.deltaTime, 0);
            yield return null;
        }
        
        IsMoving = false;
    }

    public IEnumerator ClosePanel(){
        // if (settingsModal == null) settingsModal = GameObject.FindGameObjectWithTag("SettingsModal");
        IsMoving = true;
        IsOpen = false;
        while (settingsModal.transform.position.y < Screen.height + 500){
            settingsModal.transform.position += new Vector3(0, IToggleablePanel.MOVE_SCALE * Time.deltaTime, 0);
            yield return null;
        }
        
        IsMoving = false;
    }
}
