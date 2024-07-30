using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Utility.ClassManager;

public class OnboradingFlow : MonoBehaviour {

    private readonly float SCALE_CHANGE_RATE = 2.5f;
    private GameObject introText;
    private GameObject actionText;
    private GameObject settingsAndMaterialsText;
    private GameObject buildingText;
    private GameObject generalTipText;
    Transform parentOfActionButtons = null;
    Transform parentOfArrowButton = null;
    void Start() {
        introText = gameObject.transform.GetChild(1).gameObject;
        actionText = gameObject.transform.GetChild(2).gameObject;
        settingsAndMaterialsText = gameObject.transform.GetChild(3).gameObject;
        buildingText = gameObject.transform.GetChild(4).gameObject;
        generalTipText = gameObject.transform.GetChild(5).gameObject;
        if (PlayerPrefs.GetInt("HasDoneIntro") == 0) StartOnboardingFlow();
        else gameObject.SetActive(false);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.J)) StartCoroutine(PopEffect(GameObject.FindWithTag("PlaceButton")));
    }

    public void StartOnboardingFlow() {
        gameObject.SetActive(true);
        GameObject settingsModal = GameObject.FindGameObjectWithTag("SettingsModal");
        GameObject buildingPanel = GameObject.FindGameObjectWithTag("Panel");
        settingsModal.GetComponent<MoveablePanel>().SetPanelToClosedPosition();
        buildingPanel.GetComponent<MoveablePanel>().SetPanelToClosedPosition();
        // IToggleablePanel.PanelsCurrentlyOpen++;
        BuildingController.SetCurrentAction(Actions.DO_NOTHING);
        GetInputHandler().SetCursor(InputHandler.CursorType.Default);

        introText.SetActive(true);
        actionText.SetActive(false);
        settingsAndMaterialsText.SetActive(false);
        buildingText.SetActive(false);
        generalTipText.SetActive(false);
    }

    public void ShowHowToChangeAction() {
        GameObject placeButton = GameObject.FindWithTag("PlaceButton");
        GameObject editButton = GameObject.FindWithTag("PickupButton");
        GameObject deleteButton = GameObject.FindWithTag("DeleteButton");
        parentOfActionButtons = placeButton.transform.parent;
        placeButton.transform.SetParent(gameObject.transform);
        editButton.transform.SetParent(gameObject.transform);
        deleteButton.transform.SetParent(gameObject.transform);
        introText.SetActive(false);
        actionText.SetActive(true);
        StartCoroutine(PopEffect(placeButton));
        StartCoroutine(PopEffect(editButton));
        StartCoroutine(PopEffect(deleteButton));
    }

    public void ShowSettingAndMaterials() {
        GameObject placeButton = GameObject.FindWithTag("PlaceButton");
        GameObject editButton = GameObject.FindWithTag("PickupButton");
        GameObject deleteButton = GameObject.FindWithTag("DeleteButton");
        placeButton.transform.SetParent(parentOfActionButtons);
        editButton.transform.SetParent(parentOfActionButtons);
        deleteButton.transform.SetParent(parentOfActionButtons);
        actionText.SetActive(false);
        settingsAndMaterialsText.SetActive(true);
        GameObject settingsButton = GameObject.Find("settingsButton");
        GameObject materialsButton = GameObject.Find("ShowTotalMaterials");
        settingsButton.transform.SetParent(gameObject.transform);
        materialsButton.transform.SetParent(gameObject.transform);
        StartCoroutine(PopEffect(settingsButton));
        StartCoroutine(PopEffect(materialsButton));
    }

    public void ShowHowToChangeBuilding() {
        GameObject settingsButton = GameObject.Find("settingsButton");
        GameObject materialsButton = GameObject.Find("ShowTotalMaterials");
        settingsButton.transform.SetParent(parentOfActionButtons);
        materialsButton.transform.SetParent(parentOfActionButtons);
        settingsAndMaterialsText.SetActive(false);
        buildingText.SetActive(true);
        GameObject arrowButton = GameObject.Find("ArrowButton");
        parentOfArrowButton = arrowButton.transform.parent;
        arrowButton.transform.SetParent(gameObject.transform);
        StartCoroutine(PopEffect(arrowButton));
    }

    public void ShowGeneralTip() {
        GameObject arrowButton = GameObject.Find("ArrowButton");
        arrowButton.transform.SetParent(parentOfArrowButton);
        buildingText.SetActive(false);
        generalTipText.SetActive(true);
    }

    public void EndOnboardingFlow() {
        if (parentOfActionButtons != null) {
            GameObject placeButton = GameObject.FindWithTag("PlaceButton");
            GameObject editButton = GameObject.FindWithTag("PickupButton");
            GameObject deleteButton = GameObject.FindWithTag("DeleteButton");
            placeButton.transform.SetParent(parentOfActionButtons);
            editButton.transform.SetParent(parentOfActionButtons);
            deleteButton.transform.SetParent(parentOfActionButtons);
            GameObject settingsButton = GameObject.Find("settingsButton");
            GameObject materialsButton = GameObject.Find("ShowTotalMaterials");
            settingsButton.transform.SetParent(parentOfActionButtons);
            materialsButton.transform.SetParent(parentOfActionButtons);
        }
        if (parentOfArrowButton != null) {
            GameObject arrowButton = GameObject.Find("ArrowButton");
            arrowButton.transform.SetParent(parentOfArrowButton);
        }

        // IToggleablePanel.PanelsCurrentlyOpen--;

        GameObject settingsModal = GameObject.FindGameObjectWithTag("SettingsModal");
        settingsModal.SetActive(true);
        BuildingController.SetCurrentAction(Actions.PLACE);
        gameObject.SetActive(false);
        PlayerPrefs.SetInt("HasDoneIntro", 1);

    }

    public IEnumerator PopEffect(GameObject gameObj) {
        while (gameObj.transform.localScale.x < 1.5) {
            gameObj.transform.localScale += new Vector3(SCALE_CHANGE_RATE * Time.deltaTime, SCALE_CHANGE_RATE * Time.deltaTime);
            yield return null;
        }
        while (gameObj.transform.localScale.x > 1) {
            gameObj.transform.localScale -= new Vector3(SCALE_CHANGE_RATE * Time.deltaTime, SCALE_CHANGE_RATE * Time.deltaTime);
            yield return null;
        }
    }
}
