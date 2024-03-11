using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using static Utility.ClassManager;

public class TotalMaterialsCalculator : MonoBehaviour{

    const int HEIGHT = 50;
    const int WIDTH = 1200;
    // const int X_POSITION = -325;
    // const int STARTING_Y_POSITION = 230;

    private readonly List<MaterialInfo> materialsNeeded = new List<MaterialInfo>();
    private SpriteAtlas materialAtlas;

    public void Start(){
        materialAtlas = Resources.Load<SpriteAtlas>("Materials/MaterialAtlas");
    }   

    public void SumTotalMaterialsNeeded(){
        materialsNeeded.Clear();

        foreach (Building building in GetBuildingController().GetBuildings()){
            foreach (MaterialInfo material in building.GetMaterialsNeeded()){
                if (material.IsSpecial){
                    MaterialInfo specialMaterial = materialsNeeded.FirstOrDefault(item => item.howToGet == material.howToGet);
                    if (specialMaterial != null) specialMaterial.amount += material.amount;
                    else materialsNeeded.Add(material);
                    continue;
                }
                else{
                    MaterialInfo materialInfo = materialsNeeded.FirstOrDefault(item => item.name == material.name);
                    if (materialInfo != null) materialInfo.amount += material.amount;
                    else materialsNeeded.Add(material);
                }
            }
        }
        DisplayMaterials();
    }

    private void DisplayMaterials(){
        GameObject materialsNeededPanel = GameObject.FindGameObjectWithTag("TotalMaterialsNeededPanel");
        materialsNeededPanel.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
        GameObject scrollContent = materialsNeededPanel.transform.GetChild(1).GetChild(0).gameObject;
        int counter = 0;

        for (int childIndex = 0; childIndex < scrollContent.transform.childCount; childIndex++){
            Destroy(scrollContent.transform.GetChild(childIndex).gameObject);
        }

        foreach (MaterialInfo material in materialsNeeded){
            GameObject entry = new GameObject($"{material}Entry");
            entry.transform.SetParent(scrollContent.transform);
            entry.AddComponent<RectTransform>().sizeDelta = new Vector2(WIDTH, HEIGHT);
            entry.transform.position = new Vector3(50, -50 + counter * HEIGHT , 0);
            if (!material.IsSpecial){
                AddTextGameObject(material, entry.transform);
                AddImage(material, entry.transform);
            }
            else AddSpecialTextGameObject(material, entry.transform);
            counter++;
        }
        materialsNeededPanel.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
    }

    private void AddTextGameObject(MaterialInfo material, Transform parent){
        if (material.IsSpecial) throw new ArgumentException("Material is special, use CreateSpecialTextGameObject() instead.");
        GameObject textGameObject = new GameObject(material.name.ToString());
        textGameObject.AddComponent<Text>().text = $"{material.amount:N0}x {material.name}";
        textGameObject.GetComponent<Text>().fontSize = 50;
        textGameObject.GetComponent<Text>().color = Color.black;
        textGameObject.GetComponent<Text>().font = Resources.Load<Font>("StardewValley");
        textGameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(WIDTH, HEIGHT);
        textGameObject.GetComponent<RectTransform>().pivot = new Vector2(0, 0.5f);
        textGameObject.transform.SetParent(parent);
    }

    private void AddSpecialTextGameObject(MaterialInfo material, Transform parent){
        if (!material.IsSpecial) throw new ArgumentException("Material is not special, use CreateTextGameObject() instead.");
        GameObject textGameObject = new GameObject(material.howToGet);
        textGameObject.AddComponent<Text>().text = $"{material.amount:N0}x {material.howToGet}";
        textGameObject.GetComponent<Text>().fontSize = 50;
        textGameObject.GetComponent<Text>().color = Color.black;
        textGameObject.GetComponent<Text>().font = Resources.Load<Font>("StardewValley");
        textGameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(WIDTH, HEIGHT);
        textGameObject.GetComponent<RectTransform>().pivot = new Vector2(0, 0.5f);
        textGameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        textGameObject.transform.SetParent(parent);
    }

    private void AddImage(MaterialInfo material, Transform parent){
        if (material.IsSpecial) throw new ArgumentException("Special Material do not have images");
        GameObject imageGameObject = new GameObject(material.name.ToString());
        Sprite sprite = materialAtlas.GetSprite(material.name.ToString());
        imageGameObject.AddComponent<Image>().sprite = sprite;
        imageGameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(HEIGHT, HEIGHT);
        imageGameObject.GetComponent<RectTransform>().pivot = new Vector2(1, 0.5f);
        imageGameObject.transform.SetParent(parent);
    }
}
