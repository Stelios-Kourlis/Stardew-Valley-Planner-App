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
    private bool typeBarisHidden;
    private bool searchBarIsHidden = true;
    private readonly Sprite[] arrowButtons = new Sprite[2];
    private Type lastType;
    private Type currentBuildingType;

    // Start is called before the first frame update
    void Start() {
        gameObject.transform.position = new Vector3(Screen.width + 300, 5, 0);
        arrowButtons[0] = Sprite.Create(Resources.Load("UI/TypeBarUnhide") as Texture2D, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16);
        arrowButtons[1] = Sprite.Create(Resources.Load("UI/TypeBarHide") as Texture2D, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16);

        lastType = GetBuildingController().GetCurrentBuildingType();
    }

    void Update(){
        lastType = currentBuildingType;
        currentBuildingType = GetBuildingController().GetCurrentBuildingType();
        if (currentBuildingType == lastType) return; 
        bool isCurrentlyBuildingMultipleTypeBuilding = IsMultipleTypeBuilding(currentBuildingType);
        if (isCurrentlyBuildingMultipleTypeBuilding){
            if (!typeBarIsOpen){
                typeBarIsOpen = true;
                StartCoroutine(OpenBar());
            }   
            Transform typeBarContent = transform.GetChild(0).GetChild(0);
            GameObject temp = new();
            dynamic buildingTemp = temp.AddComponent(currentBuildingType);
            GameObject[] buttons = buildingTemp.CreateButtonsForAllTypes();
            Destroy(temp);
            for(int i = 0; i<typeBarContent.childCount; i++) Destroy(typeBarContent.GetChild(i).gameObject);
            foreach (GameObject button in buttons){
                button.transform.SetParent(typeBarContent);
                button.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            }
        }else if (!isCurrentlyBuildingMultipleTypeBuilding && typeBarIsOpen) {
            typeBarIsOpen = false;
            StartCoroutine(CloseBar());
        }
    }

    public bool IsMultipleTypeBuilding(Type type){
        if (type == typeof(Crop)) return false;
        if (type == typeof(Craftables)) return false;
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
        if (typeBarisHidden) StartCoroutine(OpenBar());
        else StartCoroutine(HideBar());
    }

    public void ToggleSearchBar(){
        if (!typeBarIsOpen) return;
        if (searchBarIsHidden) StartCoroutine(OpenSearchBar());
        else StartCoroutine(HideSearchBar());
    }

    IEnumerator OpenBar() {
        while (gameObject.transform.position.x > Screen.width - 10) {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x - moveScale, gameObject.transform.position.y, gameObject.transform.position.z);
            yield return null;
        }
        transform.GetChild(1).GetComponent<Image>().sprite = arrowButtons[1];
        StartCoroutine(ShowSearchBarButton());
        typeBarisHidden = false;
    }

    IEnumerator CloseBar() {
        while (gameObject.transform.position.x < Screen.width + gameObject.GetComponent<RectTransform>().rect.width) {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x + moveScale, gameObject.transform.position.y, gameObject.transform.position.z);
            yield return null;
        }
        StartCoroutine(CloseSearchBar());
    }

    IEnumerator HideBar() {
        while (gameObject.GetComponent<RectTransform>().anchoredPosition.x < Screen.width/2 + GetComponent<RectTransform>().rect.width) {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x + moveScale, gameObject.transform.position.y, gameObject.transform.position.z);
            yield return null;
        }
        transform.GetChild(1).GetComponent<Image>().sprite = arrowButtons[0];
        typeBarisHidden = true;
        StartCoroutine(CloseSearchBar());
    }

    IEnumerator OpenSearchBar(){
        Debug.Log("Opened Search Bar");
        Transform searchBar = GameObject.FindGameObjectWithTag("TypeSearchBar").transform;
        while (searchBar.position.x > Screen.width - 10) {
            searchBar.position = new Vector3(searchBar.position.x - moveScale, searchBar.position.y, searchBar.position.z);
            yield return null;
        }
        searchBarIsHidden = false;
        searchBar.GetChild(0).GetComponent<Image>().sprite = arrowButtons[1];
    }

    IEnumerator ShowSearchBarButton(){
        Debug.Log("Opened Search Bar Button");
        Transform searchBar = GameObject.FindGameObjectWithTag("TypeSearchBar").transform;
        Transform searchBarButton = searchBar.GetChild(0);
        while (searchBarButton.GetComponent<RectTransform>().position.x > Screen.width) {
            searchBar.position = new Vector3(searchBar.position.x - moveScale, searchBar.position.y, searchBar.position.z);
            yield return null;
        }
        searchBarIsHidden = true;
        searchBar.GetChild(0).GetComponent<Image>().sprite = arrowButtons[0];
    }

    IEnumerator HideSearchBar(){
        Debug.Log("Hidden Search Bar");
        Transform searchBar = GameObject.FindGameObjectWithTag("TypeSearchBar").transform;
        Transform searchBarButton = searchBar.GetChild(0);
        while (searchBarButton.GetComponent<RectTransform>().position.x < Screen.width) {
            searchBar.position = new Vector3(searchBar.position.x + moveScale, searchBar.position.y, searchBar.position.z);
            yield return null;
        }
        searchBarIsHidden = true;
        searchBar.GetChild(0).GetComponent<Image>().sprite = arrowButtons[0];
    }

    IEnumerator CloseSearchBar(){
        Debug.Log("Closed Search Bar");
        Transform searchBar = GameObject.FindGameObjectWithTag("TypeSearchBar").transform;
        while (searchBar.position.x < Screen.width + searchBar.GetComponent<RectTransform>().rect.width + 50) {
            searchBar.position = new Vector3(searchBar.position.x + moveScale, searchBar.position.y, searchBar.position.z);
            yield return null;
        }
    }
}
