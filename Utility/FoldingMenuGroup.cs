using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FoldingMenuGroup : MonoBehaviour {

    public List<FoldingMenuItem> menuItems = new();
    public bool isOpen = false;

    public void AddMenuItem(FoldingMenuItem item) {
        if (item.isAnchorButton) {
            if (menuItems.Any(button => button.isAnchorButton)) throw new System.Exception("Only one anchor button is allowed per menu group");
            menuItems.Insert(0, item);
        }
        else menuItems.Add(item);
    }

    public void ToggleMenu() {
        if (isOpen) CloseMenu();
        else OpenMenu();
    }

    public void OpenMenu() {
        // Debug.Log("Opening Menu");
        Vector3 startPosition = menuItems.First(button => button.GetComponent<FoldingMenuItem>().isAnchorButton).GetComponent<RectTransform>().localPosition;
        float buttonWidth = menuItems.First(button => button.GetComponent<FoldingMenuItem>().isAnchorButton).GetComponent<RectTransform>().rect.width;
        for (int childIndex = 0; childIndex < transform.childCount; childIndex++) {
            if (transform.GetChild(childIndex).GetComponent<FoldingMenuItem>().isAnchorButton) continue;
            Vector3 endPosition = startPosition - new Vector3((buttonWidth + 10) * menuItems.IndexOf(transform.GetChild(childIndex).GetComponent<FoldingMenuItem>()), 0, 0);
            StartCoroutine(ObjectMover.MoveUIObjectInConstantTime(transform.GetChild(childIndex).transform, startPosition, endPosition, 0.5f));
        }
        isOpen = true;
    }

    public void CloseMenu() {
        Vector3 endPosition = menuItems.First(button => button.GetComponent<FoldingMenuItem>().isAnchorButton).GetComponent<RectTransform>().localPosition;
        for (int childIndex = 0; childIndex < transform.childCount; childIndex++) {
            if (transform.GetChild(childIndex).GetComponent<FoldingMenuItem>().isAnchorButton) continue;
            Vector3 startPosition = transform.GetChild(childIndex).GetComponent<RectTransform>().localPosition;
            StartCoroutine(ObjectMover.MoveUIObjectInConstantTime(transform.GetChild(childIndex).transform, startPosition, endPosition, 0.5f));
        }
        isOpen = false;
    }
}
