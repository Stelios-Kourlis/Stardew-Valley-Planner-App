using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static Utility.ClassManager;

public class HUDButtonCotroller : MonoBehaviour {

    public enum RelativePosition {
        LEFT,
        RIGHT,
        TOP,
        BOTTOM
    }

    public void Awake() {
        //Action Buttons
        GameObject.FindWithTag("PlaceButton").GetComponent<Button>().onClick.AddListener(() => { ActionButtonPressed(GameObject.FindWithTag("PlaceButton"), Actions.PLACE); });
        GameObject.FindWithTag("DeleteButton").GetComponent<Button>().onClick.AddListener(() => { ActionButtonPressed(GameObject.FindWithTag("DeleteButton"), Actions.DELETE); });
        GameObject.FindWithTag("PickupButton").GetComponent<Button>().onClick.AddListener(() => { ActionButtonPressed(GameObject.FindWithTag("PickupButton"), Actions.PICKUP); });
        GameObject.FindWithTag("CopyBuilding").GetComponent<Button>().onClick.AddListener(() => { ActionButtonPressed(GameObject.FindWithTag("CopyBuilding"), Actions.PICK_BUILDING); });
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
        // Debug.Log($"Panel width: {panel.GetComponent<RectTransform>().rect.width}");
        panel.transform.localPosition = Vector3.zero - new Vector3(panel.GetComponent<RectTransform>().rect.width / 2 + 100, 0, 0);
        // Debug.Log($"Panel position: {panel.transform.localPosition}");
        return panel;
    }

    public static GameObject CreateButtonNextToOtherButton(RectTransform existingButton, GameObject newButtonPrefab, RelativePosition relativePosition, Transform parent) {
        GameObject newButton = Instantiate(newButtonPrefab, parent);
        float existingButtonHalfHeight = existingButton.rect.height / 2;
        float existingButtonHalfWidth = existingButton.rect.width / 2;
        float newButtonHalfHeight = newButton.GetComponent<RectTransform>().rect.height / 2;
        float newButtonHalfWidth = newButton.GetComponent<RectTransform>().rect.width / 2;
        float buttonSpacing = 10;
        newButton.GetComponent<RectTransform>().anchoredPosition = relativePosition switch {
            RelativePosition.LEFT => new Vector3(existingButton.anchoredPosition.x - existingButtonHalfWidth - newButtonHalfWidth - buttonSpacing, existingButton.anchoredPosition.y, existingButton.position.z),
            RelativePosition.RIGHT => new Vector3(existingButton.anchoredPosition.x + existingButtonHalfWidth + newButtonHalfWidth + buttonSpacing, existingButton.anchoredPosition.y, existingButton.position.z),
            RelativePosition.TOP => new Vector3(existingButton.anchoredPosition.x, existingButton.anchoredPosition.y + existingButtonHalfHeight + newButtonHalfHeight + buttonSpacing, existingButton.position.z),
            RelativePosition.BOTTOM => new Vector3(existingButton.anchoredPosition.x, existingButton.anchoredPosition.y - existingButtonHalfHeight - newButtonHalfHeight - buttonSpacing, existingButton.position.z),
            _ => throw new Exception("Invalid relative position"),
        };
        return newButton;
    }


}
