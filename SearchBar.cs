using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Utility.ClassManager;

public class SearchBar : MonoBehaviour{

    public void Awake(){
        InputField inputField;
        inputField = GetComponent<InputField>();
        inputField.onValueChanged.AddListener(OnValueChanged);
        inputField.onEndEdit.AddListener(OnEndEdit);
        EventTrigger trigger = inputField.gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry{
            eventID = EventTriggerType.PointerClick
        };
        entry.callback.AddListener((data) => { OnSelect(); });
        trigger.triggers.Add(entry);
    }

    public void OnValueChanged(string text){
        GameObject content = null;
        for (int childIndex = 0; childIndex < gameObject.transform.parent.childCount; childIndex++){
            if (gameObject.transform.parent.GetChild(childIndex).gameObject.activeSelf){
                content = gameObject.transform.parent.GetChild(childIndex).GetChild(0).gameObject;
                break;
            }
        }
        if (content == null) throw new Exception("Content not found");
        for (int index = 0; index < content.transform.childCount; index++){
            GameObject child = content.transform.GetChild(index).gameObject;
            child.SetActive(child.name.ToLower().Contains(text.ToLower()));
        }
    }

    private void OnEndEdit(string text){
        GetInputHandler().IsSearching = false;
    }

    private void OnSelect(){
        GetInputHandler().IsSearching = true;
    }
}
