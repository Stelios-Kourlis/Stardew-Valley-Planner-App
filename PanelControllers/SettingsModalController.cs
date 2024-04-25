using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Utility.ClassManager;

public class SettingsModalController : MonoBehaviour, IToggleablePanel {
    private GameObject settingsModal;
    public bool IsMoving {get; private set;}
    public bool IsOpen {get; private set;}
    // public Actions ActionBeforeEnteringSettings {get; private set;}
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
        if (IsOpen) yield break;
        IToggleablePanel.PanelsCurrentlyOpen++;
        IsMoving = true;
        GameObject buildingPanel = GameObject.FindGameObjectWithTag("Panel");
        StartCoroutine(GetTotalMaterialsCalculator().ClosePanel());
        if (Building.CurrentAction != Actions.DO_NOTHING){
            IToggleablePanel.ActionBeforeEnteringSettings = Building.CurrentAction;
            Building.CurrentAction = Actions.DO_NOTHING;
        }
        StartCoroutine(GetCamera().GetComponent<CameraController>().BlurBasedOnDistance(gameObject, new Vector3(Screen.width/2, Screen.height/2, 0)));// the close to 0,0 the more blur we want
        StartCoroutine(buildingPanel.GetComponent<BuildingMenuController>().ClosePanel());

        IsOpen = true;
        while (settingsModal.transform.position.y > Screen.height/2){
            settingsModal.transform.position -= new Vector3(0, IToggleablePanel.MOVE_SCALE * Time.deltaTime, 0);
            yield return null;
        }
        Building.CurrentAction = Actions.DO_NOTHING;
        settingsModal.transform.position = new Vector3(Screen.width/2, Screen.height/2, 0);
        IsMoving = false;
        
    }

    public IEnumerator ClosePanel(){
        if (!IsOpen) yield break;
        IsMoving = true;
        IToggleablePanel.PanelsCurrentlyOpen--;
        StartCoroutine(GetCamera().GetComponent<CameraController>().BlurBasedOnDistance(gameObject, new Vector3(Screen.width/2, Screen.height/2, 0)));// the close to 0,0 the more blur we want, so when it goes away put the start point
        
        IsOpen = false;
        while (settingsModal.transform.position.y < Screen.height + 500){
            settingsModal.transform.position += new Vector3(0, IToggleablePanel.MOVE_SCALE * Time.deltaTime, 0);
            yield return null;
        }
        settingsModal.transform.position = new Vector3(Screen.width/2, Screen.height + 500, 0);
        IsMoving = false;
        if (IToggleablePanel.PanelsCurrentlyOpen == 0) Building.CurrentAction = IToggleablePanel.ActionBeforeEnteringSettings;
    }
}
