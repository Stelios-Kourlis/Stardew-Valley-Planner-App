using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HUDButtonCotroller : MonoBehaviour {
    public void Awake() {
        //Action Buttons
        GameObject.FindWithTag("PlaceButton").GetComponent<Button>().onClick.AddListener(() => { ActionButtonPressed(GameObject.FindWithTag("PlaceButton"), Actions.PLACE); });
        GameObject.FindWithTag("DeleteButton").GetComponent<Button>().onClick.AddListener(() => { ActionButtonPressed(GameObject.FindWithTag("DeleteButton"), Actions.DELETE); });
        GameObject.FindWithTag("PickupButton").GetComponent<Button>().onClick.AddListener(() => { ActionButtonPressed(GameObject.FindWithTag("PickupButton"), Actions.EDIT); });
        GameObject.Find("CloseMenuButton").GetComponent<Button>().onClick.AddListener(() => { GameObject.Find("ActionButtons").GetComponent<FoldingMenuGroup>().ToggleMenu(); });
    }

    private void ActionButtonPressed(GameObject button, Actions action) {
        if (GameObject.Find("ActionButtons").GetComponent<FoldingMenuGroup>().isOpen) {
            button.transform.SetAsLastSibling();
            BuildingController.SetCurrentAction(action);
        }
        GameObject.Find("ActionButtons").GetComponent<FoldingMenuGroup>().ToggleMenu();

    }

    public static GameObject CreatePanelNextToButton(GameObject panelPrefab, RectTransform buttonRectTransform) {
        Debug.Log($"Creating {panelPrefab.name} next to button");
        GameObject panel = Instantiate(panelPrefab, buttonRectTransform);
        Debug.Log($"Panel width: {panel.GetComponent<RectTransform>().rect.width}");
        panel.transform.localPosition = Vector3.zero - new Vector3(panel.GetComponent<RectTransform>().rect.width / 2 + 100, 0, 0);
        Debug.Log($"Panel position: {panel.transform.localPosition}");
        // panel.SetActive(false);
        return panel;
    }

}
