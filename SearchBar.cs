using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Utility.ClassManager;

public class SearchBar : MonoBehaviour {

    [SerializeField] GameObject contentGameObject;
    public void Awake() {
        // if (gameObject.name == "Search") contentGameObject = transform.parent.GetChild(0).GetChild(0).gameObject;
        // else if (gameObject.name == "TypeSearchBar") contentGameObject = GameObject.Find("TypeSelectBar").transform.GetChild(0).GetChild(0).gameObject;
        InputField inputField;
        inputField = GetComponent<InputField>();
        inputField.onValueChanged.AddListener(OnValueChanged);
        inputField.onEndEdit.AddListener(OnEndEdit);
        EventTrigger trigger = inputField.gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new() {
            eventID = EventTriggerType.PointerClick
        };
        entry.callback.AddListener((data) => { OnSelect(); });
        trigger.triggers.Add(entry);
    }

    public void OnValueChanged(string text) {
        for (int childIndex = 0; childIndex < contentGameObject.transform.childCount; childIndex++) {
            if (contentGameObject.transform.GetChild(childIndex).name.ToLower().Contains(text.ToLower())) {
                contentGameObject.transform.GetChild(childIndex).gameObject.SetActive(true);
            }
            else contentGameObject.transform.GetChild(childIndex).gameObject.SetActive(false);
        }
    }

    private void OnEndEdit(string text) {
        GetInputHandler().IsSearching = false;
    }

    private void OnSelect() {
        GetInputHandler().IsSearching = true;
    }
}
