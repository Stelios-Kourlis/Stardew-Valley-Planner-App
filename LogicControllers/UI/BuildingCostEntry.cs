using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BuildingCostEntry : MonoBehaviour {

    private int amount;
    private List<MaterialCostEntry> originalMaterialsNeededForThisEntry;

    public void Start() {
        amount = 0;
    }

    public void SetMaterials(List<MaterialCostEntry> materialCostEntries) {
        originalMaterialsNeededForThisEntry = new List<MaterialCostEntry>(materialCostEntries);
    }

    public void SetTitleName(string newName) {
        newName.Replace("Crop", "");
        newName.Replace("Craftables", "");
        transform.Find("Text").Find("BuildingName").Find("name").GetComponent<TMP_Text>().text = newName;

    }

    public void SetTitleAmount(int newAmount) {
        amount = newAmount;
        if (newAmount == 1) transform.Find("Text").Find("BuildingName").Find("amount").GetComponent<TMP_Text>().text = "";
        else transform.Find("Text").Find("BuildingName").Find("amount").GetComponent<TMP_Text>().text = $"{newAmount}x";
        List<MaterialCostEntry> materialsNeededForThisEntry = new();
        foreach (MaterialCostEntry material in originalMaterialsNeededForThisEntry) {
            // Debug.Log($"Originally this needed {originalMaterialsNeededForThisEntry.Find(item => item.EntryText == material.EntryText).amount} {material.EntryText}");
            var materialCopy = material.Clone() as MaterialCostEntry;
            materialCopy.amount *= newAmount;
            materialsNeededForThisEntry.Add(materialCopy);
        }
        UpdateAllMaterials(materialsNeededForThisEntry);
    }

    public void IncrementTitleAmount() {
        SetTitleAmount(amount + 1);
    }

    private void UpdateAllMaterials(List<MaterialCostEntry> materialsNeededForThisEntry) {
        foreach (Transform child in transform.Find("Text").Find("Materials")) {
            Destroy(child.gameObject);
        }
        foreach (MaterialCostEntry material in materialsNeededForThisEntry) {
            TotalMaterialsCalculator.Instance.CreateTextGameObject(material, transform.Find("Text").Find("Materials"));
        }
    }
}
