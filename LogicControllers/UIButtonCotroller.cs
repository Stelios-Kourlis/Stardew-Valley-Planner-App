using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonCotroller : MonoBehaviour {
    private bool ActionButtonMenuIsOpen = false;

    public void Awake() {
        //Action Buttons
        GameObject.FindWithTag("PlaceButton").GetComponent<Button>().onClick.AddListener(() => { ActionButtonPressed(GameObject.FindWithTag("PlaceButton"), Actions.PLACE); });
        GameObject.FindWithTag("DeleteButton").GetComponent<Button>().onClick.AddListener(() => { ActionButtonPressed(GameObject.FindWithTag("DeleteButton"), Actions.DELETE); });
        GameObject.FindWithTag("PickupButton").GetComponent<Button>().onClick.AddListener(() => { ActionButtonPressed(GameObject.FindWithTag("PickupButton"), Actions.EDIT); });
    }

    public void ToggleActionButtonMenu() {
        if (ActionButtonMenuIsOpen) CloseActionButtonMenu();
        else OpenActionButtonMenu();
    }

    public void OpenActionButtonMenu() {

        StartCoroutine(OpenActionButtonMenuCoroutine());

        GameObject.Find("ActionButtons").transform.Find("PickupButton").SetAsLastSibling();
        GameObject.Find("ActionButtons").transform.Find("DeleteButton").SetAsLastSibling();
        GameObject.Find("ActionButtons").transform.Find("PlaceButton").SetAsLastSibling();
        GameObject.Find("ActionButtons").transform.Find("CloseMenuButton").SetAsLastSibling();
        ActionButtonMenuIsOpen = true;
    }

    private IEnumerator OpenActionButtonMenuCoroutine() {
        HorizontalLayoutGroup actionButtonLayoutGroup = GameObject.Find("ActionButtons").GetComponent<HorizontalLayoutGroup>();
        while (actionButtonLayoutGroup.spacing < 0) {
            actionButtonLayoutGroup.spacing += 5;
            yield return null;
        }
        actionButtonLayoutGroup.spacing = 0;
    }

    public void CloseActionButtonMenu() {
        GameObject.Find("ActionButtons").transform.Find("CloseMenuButton").SetAsFirstSibling();
        StartCoroutine(CloseActionButtonMenuCoroutine());
        ActionButtonMenuIsOpen = false;
    }

    private IEnumerator CloseActionButtonMenuCoroutine() {
        HorizontalLayoutGroup actionButtonLayoutGroup = GameObject.Find("ActionButtons").GetComponent<HorizontalLayoutGroup>();
        while (actionButtonLayoutGroup.spacing > actionButtonLayoutGroup.transform.childCount * -110) {
            actionButtonLayoutGroup.spacing -= 5;
            yield return null;
        }
        actionButtonLayoutGroup.spacing = actionButtonLayoutGroup.transform.childCount * -110;
    }

    private void ActionButtonPressed(GameObject button, Actions action) {
        Debug.Log(ActionButtonMenuIsOpen);
        if (!ActionButtonMenuIsOpen) {
            BuildingController.SetCurrentAction(action);
            button.transform.SetAsLastSibling();
            CloseActionButtonMenu();
        }
        else OpenActionButtonMenu();

    }
}
