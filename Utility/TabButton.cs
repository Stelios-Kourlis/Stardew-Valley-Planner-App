using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
[Serializable]
public class TabButton : MonoBehaviour, IPointerEnterHandler {

    [SerializeField] private TabGroup tabGroup;

    void Start() {
        tabGroup.AddTabToTabGroup(this);
        GetComponent<Button>().onClick.AddListener(() => tabGroup.ChangeSelectedTab(this));
        if (gameObject.name == "Maps") tabGroup.ChangeSelectedTab(this); //Initialize the setting sto the map tab
    }

    public void OnPointerEnter(PointerEventData eventData) {
        GetComponent<Image>().color = Color.HSVToRGB(0, 0, 0.8f);
    }
}
