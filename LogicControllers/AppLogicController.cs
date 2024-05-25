using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Utility.BuildingManager;

public class AppLogicController : MonoBehaviour{
    public void Start(){
        DontDestroyOnLoad(gameObject);
        Cursor.SetCursor(Resources.Load("UI/Cursor") as Texture2D, Vector2.zero, CursorMode.ForceSoftware);
    }

    public void PlayerPressedCreateFarmButton(){
        // SceneManager.LoadScene("App");
    }

    public void PlayerPressedLoadFarmButton(){
        StartCoroutine(SwitchSceneThenLoad());
    }

    public IEnumerator SwitchSceneThenLoad(){
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("App");
        while (!asyncLoad.isDone) yield return null;
        Debug.Log("loading farm");
        Load();
        Debug.Log("loaded farm");
    }

    public void Quit() {
        GameObject quitConfirmPanel = GameObject.FindGameObjectWithTag("QuitConfirm");
        quitConfirmPanel.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
    }

    public void QuitApp(){
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}

internal class SceneLoader
{
}