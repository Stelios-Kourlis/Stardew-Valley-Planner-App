using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static Utility.ClassManager;
using static Utility.BuildingManager;
using System.Reflection;
using Utility;

public class TypeBarHandler : MonoBehaviour {
    private readonly float moveScale = 5f;
    private bool typeBarIsOpen = false;
    private bool isHidden;
    private readonly Sprite[] arrowButtons = new Sprite[2];
    private Type lastType;

    // Start is called before the first frame update
    void Start() {
        gameObject.transform.position = new Vector3(Screen.width + 300, 5, 0);
        arrowButtons[0] = Sprite.Create(Resources.Load("UI/TypeBarUnhide") as Texture2D, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16);
        arrowButtons[1] = Sprite.Create(Resources.Load("UI/TypeBarHide") as Texture2D, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16);

        // Debug.Log("TBH: Getting all building types");
        var buildingType = typeof(MultipleTypeBuilding<>);
        var allTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMultipleTypeBuilding<>)));
        
        foreach (var type in allTypes){
            if (type == typeof(Sprinkler)) continue;
            GameObject temp = new();
            dynamic buildingTemp = temp.AddComponent(type);
            GameObject[] buttons = buildingTemp.CreateButtonsForAllTypes();
            Transform typeBarContent = transform.GetChild(0).GetChild(0);
            foreach (GameObject button in buttons){
                button.name = $"{type}{button.name}";
                button.transform.SetParent(typeBarContent);
                button.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            }
            Destroy(temp);
            // StartCoroutine(RemoveComponentNextFrame(buildingTemp)); //Components cant be removed the same frame they are added
        }

        lastType = GetBuildingController().GetCurrentBuildingType();
        // GameObject[] typeButtons = currentBuilding.CreateButtonsForAllTypes();
        // foreach (GameObject button in typeButtons) button.transform.SetParent(contentGameObject.transform);
    }

    void Update(){
        Type currentBuildingType = GetBuildingController().GetCurrentBuildingType();
        if (currentBuildingType == lastType) return; 
        bool isCurrentlyBuildingMultipleTypeBuilding = IsMultipleTypeBuilding(currentBuildingType);
        if (isCurrentlyBuildingMultipleTypeBuilding){
            if (!typeBarIsOpen){
                typeBarIsOpen = true;
                StartCoroutine(OpenBar());
            }   
            Transform typeBarContent = transform.GetChild(0).GetChild(0);
            for (int i = 0; i<typeBarContent.childCount; i++){
                if (typeBarContent.GetChild(i).name.Contains(currentBuildingType.Name)) typeBarContent.GetChild(i).gameObject.SetActive(true);
                else typeBarContent.GetChild(i).gameObject.SetActive(false);
            }
        }else if (!isCurrentlyBuildingMultipleTypeBuilding && typeBarIsOpen) {
            typeBarIsOpen = false;
            StartCoroutine(CloseBar());
        }
    }

    public bool IsMultipleTypeBuilding(Type type){
         var interfaces = type.GetInterfaces();
         foreach (var i in interfaces){
            if (i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMultipleTypeBuilding<>)){
                return true;
            }
        }
        return false;
    }


    /// <summary>
    /// if the bar is visible (floor is selected), toggle it collapsing to the side and back
    /// </summary>
    public void ToggleHideBar(){
        if (!typeBarIsOpen) return;
        if (isHidden) StartCoroutine(OpenBar());
        else StartCoroutine(HideBar());
        isHidden = !isHidden;
    }

    IEnumerator OpenBar() {
        while (gameObject.transform.position.x > Screen.width - 10) {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x - moveScale, gameObject.transform.position.y, gameObject.transform.position.z);
            yield return null;
        }
        transform.GetChild(1).GetComponent<Image>().sprite = arrowButtons[1];
    }

    IEnumerator CloseBar() {
        while (gameObject.transform.position.x < Screen.width + 300) {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x + moveScale, gameObject.transform.position.y, gameObject.transform.position.z);
            yield return null;
        }
    }

    IEnumerator HideBar() {
        while (gameObject.GetComponent<RectTransform>().anchoredPosition.x < Screen.width/2 + GetComponent<RectTransform>().sizeDelta.x) {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x + moveScale, gameObject.transform.position.y, gameObject.transform.position.z);
            yield return null;
        }
        transform.GetChild(1).GetComponent<Image>().sprite = arrowButtons[0];
    }
}
