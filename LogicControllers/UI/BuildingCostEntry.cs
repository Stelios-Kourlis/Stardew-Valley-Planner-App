using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BuildingCostEntry : MonoBehaviour {

    private int amount;

    public void Start() {
        amount = 0;
    }

    public void SetTitleName(string newName) {
        transform.Find("Text").Find("BuildingName").Find("name").GetComponent<TMP_Text>().text = newName;
    }

    public void SetTitleAmount(int newAmount) {
        amount = newAmount;
        if (newAmount == 0) transform.Find("Text").Find("BuildingName").Find("amount").GetComponent<TMP_Text>().text = "";
        else transform.Find("Text").Find("BuildingName").Find("amount").GetComponent<TMP_Text>().text = $"{newAmount}x";
    }

    public void IncrementTitleAmount() {
        SetTitleAmount(amount + 1);
    }

    // private void UpdateAllMaterials() {
    //     var parent = ;
    //     foreach (Transform material in parent) {
    //         int amount = int.Parse(material.transform.Find("Text").Find("Amount").GetComponent<TMP_Text>().text.Replace("x", ""));
    //     }
    // }
}
