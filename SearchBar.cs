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
    private int childCount;
    [SerializeField] private Button clearButton;
    public void Awake() {
        //Listeners
        InputField inputField;
        inputField = GetComponent<InputField>();
        inputField.onValueChanged.AddListener(OnValueChanged);
        inputField.onEndEdit.AddListener(OnEndEdit);

        //Set is searching to true on select
        EventTrigger trigger = inputField.gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new() {
            eventID = EventTriggerType.PointerClick
        };
        entry.callback.AddListener((data) => { OnSelect(); });
        trigger.triggers.Add(entry);

        //Update on change
        if (clearButton != null) clearButton.onClick.AddListener(() => { inputField.text = ""; });
        // contentGameObject.childrenChanged += () => { OnValueChanged(inputField.text); };
    }

    public void Update() {
        if (childCount != contentGameObject.transform.childCount) {
            childCount = contentGameObject.transform.childCount;
            OnValueChanged(GetComponent<InputField>().text);
        }
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
