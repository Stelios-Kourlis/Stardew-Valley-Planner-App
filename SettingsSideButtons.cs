using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsSideButtons : MonoBehaviour{

    void Start(){
        GameObject modal = GameObject.FindGameObjectWithTag("SettingsModal");
        GetComponent<Button>().onClick.AddListener(() => {
            for (int i = 1; i<modal.transform.childCount;i++){
                if (modal.transform.GetChild(i).name == gameObject.name) modal.transform.GetChild(i).gameObject.SetActive(true);
                else modal.transform.GetChild(i).gameObject.SetActive(false);
            } 
        });
    }
}
