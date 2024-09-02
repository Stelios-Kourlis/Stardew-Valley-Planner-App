using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Utility.ClassManager;

[RequireComponent(typeof(Button))]
[Serializable]
public class TabButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    [SerializeField] private TabGroup tabGroup;
    public string TabName;

    void Start() {
        tabGroup.AddTabToTabGroup(this);
        GetComponent<Button>().onClick.AddListener(() => tabGroup.ChangeSelectedTab(this));
        GetComponent<Image>().color = Color.HSVToRGB(0, 0, 0.6f);

        if (gameObject.name == "Maps") tabGroup.ChangeSelectedTab(this);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (tabGroup.SelectedTab != this) GetComponent<Image>().color = Color.HSVToRGB(0, 0, 0.8f);
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (tabGroup.SelectedTab != this) GetComponent<Image>().color = Color.HSVToRGB(0, 0, 0.6f);
    }
}
