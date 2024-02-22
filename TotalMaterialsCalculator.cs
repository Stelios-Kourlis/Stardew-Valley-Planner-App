using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Utility.ClassManager;

public class TotalMaterialsCalculator : MonoBehaviour{

    const int HEIGHT = 50;
    const int WIDTH = 750;
    // const int X_POSITION = -325;
    // const int STARTING_Y_POSITION = 230;

    private readonly Dictionary<Materials, int> totalMaterialsNeeded = new Dictionary<Materials, int>();

    public void SumTotalMaterialsNeeded(){
        GameObject materialsNeededPanel = GameObject.FindGameObjectWithTag("TotalMaterialsNeededPanel");
        materialsNeededPanel.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
        GameObject scrollContent = materialsNeededPanel.transform.GetChild(1).GetChild(0).gameObject;
    
        totalMaterialsNeeded.Clear();
        foreach (Building building in GetBuildingController().GetBuildings()){
            foreach (KeyValuePair<Materials, int> material in building.materialsNeeded){
                if (totalMaterialsNeeded.ContainsKey(material.Key)) totalMaterialsNeeded[material.Key] += material.Value;
                else totalMaterialsNeeded.Add(material.Key, material.Value);
            }
        }

        for (int childIndex = 0; childIndex < scrollContent.transform.childCount; childIndex++){
            Destroy(scrollContent.transform.GetChild(childIndex).gameObject);
        }
       
        int counter = 2;
        foreach (KeyValuePair<Materials, int> material in totalMaterialsNeeded){
            GameObject line = new GameObject(material.Key.ToString());
            line.transform.SetParent(scrollContent.transform);
            line.AddComponent<RectTransform>();
            GameObject text = CreateTextGameObject(material.Key.ToString() + "Text", material.Key + ": " + material.Value.ToString("N0"), line.transform);
            GameObject image = CreateImageGameObject(material.Key.ToString() + "Image", "Materials/"+material.Key.ToString(), line.transform);
            counter++;
        }
    }

    private GameObject CreateTextGameObject(string name, string text, Transform parent){
        GameObject materialsNeededPanel = GameObject.FindGameObjectWithTag("TotalMaterialsNeededPanel");
        GameObject textGameObject = new GameObject(name);
        textGameObject.AddComponent<Text>().text = text;
        textGameObject.GetComponent<Text>().fontSize = 50;
        textGameObject.GetComponent<Text>().color = Color.black;
        textGameObject.GetComponent<Text>().font = Resources.Load<Font>("StardewValley");
        textGameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(WIDTH, HEIGHT);
        textGameObject.GetComponent<RectTransform>().pivot = new Vector2(0, 0.5f);
        textGameObject.transform.SetParent(parent);
        return textGameObject;
    }

    private GameObject CreateImageGameObject(string name, string spritePath, Transform parent){
        GameObject materialsNeededPanel = GameObject.FindGameObjectWithTag("TotalMaterialsNeededPanel");
        GameObject imageGameObject = new GameObject(name);
        Sprite sprite = Resources.Load<Sprite>(spritePath);
        imageGameObject.AddComponent<Image>().sprite = sprite;
        imageGameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(HEIGHT, HEIGHT);
        imageGameObject.GetComponent<RectTransform>().pivot = new Vector2(1, 0.5f);
        imageGameObject.transform.SetParent(parent);
        return imageGameObject;
    }
}
