using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class TabGroup : MonoBehaviour {
    private Color TAB_INACTIVE_COLOR = Color.HSVToRGB(0, 0, 0.6f);
    private Color TAB_ACTIVE_COLOR = Color.HSVToRGB(0, 0, 1);

    [SerializeField] private GameObject tabContentParent;
    [SerializeField] private List<TabButton> tabButtons;
    [SerializeField] private TabButton selectedTab;
    public void Awake() {
        tabButtons = new List<TabButton>();
    }

    public void AddTabToTabGroup(TabButton tabButton) {
        tabButtons.Add(tabButton);
    }

    public void ChangeSelectedTab(TabButton newSelectedTab) {
        selectedTab = newSelectedTab;
        foreach (TabButton button in tabButtons) button.GetComponent<Image>().color = TAB_INACTIVE_COLOR;
        selectedTab.GetComponent<Image>().color = TAB_ACTIVE_COLOR;
        foreach (Transform child in tabContentParent.transform) child.gameObject.SetActive(false);
        tabContentParent.transform.Find(selectedTab.gameObject.name).gameObject.SetActive(true);

    }
}
