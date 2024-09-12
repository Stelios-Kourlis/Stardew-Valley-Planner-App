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
            // Debug.Log("g");
            button.transform.SetAsLastSibling();
            BuildingController.SetCurrentAction(action);
        }
        GameObject.Find("ActionButtons").GetComponent<FoldingMenuGroup>().ToggleMenu();

    }

}
