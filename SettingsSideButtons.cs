using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsSideButtons : MonoBehaviour{

    void Start(){
        GetComponent<Button>().onClick.AddListener(SideButtonPressed); 
        if (gameObject.name == "Maps") SideButtonPressed();
    }

    private void SideButtonPressed(){
        //Set the active panel to the one that corresponds to the button that was pressed
        GameObject modal = GameObject.FindGameObjectWithTag("SettingsModal");
        for (int i = 1; i<modal.transform.childCount;i++){
            GameObject panel = modal.transform.GetChild(i).gameObject;
            if (panel.name == gameObject.name) panel.SetActive(true);
            else panel.SetActive(false);
        }

        //Change the color of the button that was pressed to indicate that it is active
        GameObject sideButtonsParent = modal.transform.Find("SideButtons").gameObject;
        for (int i = 0; i<sideButtonsParent.transform.childCount;i++){
            GameObject button = sideButtonsParent.transform.GetChild(i).gameObject;
            float V;
            if (button == gameObject) V = 1;
            else V = 0.6f;
            button.GetComponent<Image>().color = Color.HSVToRGB(0, 0, V);
        }
    }
}
