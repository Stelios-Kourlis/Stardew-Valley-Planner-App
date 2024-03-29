using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static Utility.ClassManager;

public class TypeBarHandler : MonoBehaviour {
    private readonly float moveScale = 5f;
    private bool typeBarIsOpen = false;
    private bool isHidden;
    private readonly Sprite[] arrowButtons = new Sprite[2];

    // Start is called before the first frame update
    void Start() {
        gameObject.transform.position = new Vector3(Screen.width + 300, 5, 0);
        arrowButtons[0] = Sprite.Create(Resources.Load("UI/TypeBarUnhide") as Texture2D, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16);
        arrowButtons[1] = Sprite.Create(Resources.Load("UI/TypeBarHide") as Texture2D, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16);
    }

    void Update(){
        bool isCurrentBuildingFloor = GetBuildingController().GetCurrentBuildingType() == typeof(Floor);
        bool isCurrentBuildingFence = GetBuildingController().GetCurrentBuildingType() == typeof(Fence);
        var currentBuildingType = GetBuildingController().GetCurrentBuildingType();
        bool isCurrentlyBuildingMultipleTypeBuilding = currentBuildingType.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMultipleTypeBuilding<>));
        Debug.Log(isCurrentlyBuildingMultipleTypeBuilding);
        if (isCurrentlyBuildingMultipleTypeBuilding){
            if (!typeBarIsOpen){
                typeBarIsOpen = true;
                StartCoroutine(OpenBar());
            }   
            if (GetBuildingController().GetCurrentBuildingType() == typeof(Floor)){
                for (int i = 0; i < transform.GetChild(0).GetChild(0).childCount; i++){
                    GameObject child = transform.GetChild(0).GetChild(0).GetChild(i).gameObject;
                    if (child.name.Contains("Floor") || child.name.Contains("Path")) child.SetActive(true);
                    else child.SetActive(false); 
                    // if (i<13) transform.GetChild(0).GetChild(0).GetChild(i).gameObject.SetActive(true);
                    // else transform.GetChild(0).GetChild(0).GetChild(i).gameObject.SetActive(false);
                }
            }else if (GetBuildingController().GetCurrentBuildingType() == typeof(Fence)){
                for (int i = 0; i < transform.GetChild(0).GetChild(0).childCount; i++){
                    GameObject child = transform.GetChild(0).GetChild(0).GetChild(i).gameObject;
                    if (child.name.Contains("Fence")) child.SetActive(true);
                    else child.SetActive(false); 
                }
            }else{
                for (int i = 0; i < transform.GetChild(0).GetChild(0).childCount; i++){
                    GameObject child = transform.GetChild(0).GetChild(0).GetChild(i).gameObject;
                    if (child.name.Contains("Fence") || child.name.Contains("Floor") || child.name.Contains("Path")) child.SetActive(false);
                    else child.SetActive(true); 
                }
            }
        }else if (!isCurrentlyBuildingMultipleTypeBuilding && typeBarIsOpen) {
            typeBarIsOpen = false;
            StartCoroutine(CloseBar());
        }
        // if (isCurrentBuildingFloor && !typeBarIsOpen){
        //     typeBarIsOpen = true;
        //     for (int i = 0; i < transform.GetChild(0).GetChild(0).childCount; i++){
        //         if (i<13) transform.GetChild(0).GetChild(0).GetChild(i).gameObject.SetActive(true);
        //         else transform.GetChild(0).GetChild(0).GetChild(i).gameObject.SetActive(false);
        //     }
        //     StartCoroutine(OpenBar());
        // }
        // else if (isCurrentBuildingFence && !typeBarIsOpen){
        //     typeBarIsOpen = true;
        //     for (int i = 0; i < transform.GetChild(0).GetChild(0).childCount; i++){
        //         if (i<13) transform.GetChild(0).GetChild(0).GetChild(i).gameObject.SetActive(false);
        //         else transform.GetChild(0).GetChild(0).GetChild(i).gameObject.SetActive(true);
        //     }
        //     StartCoroutine(OpenBar());
        // }
        // else if (!isCurrentBuildingFence && !isCurrentBuildingFloor && typeBarIsOpen) {
        //     typeBarIsOpen = false;
        //     StartCoroutine(CloseBar());
        // }
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
