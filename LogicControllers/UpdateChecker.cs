using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UpdateChecker : MonoBehaviour {
    private readonly string currentVersion = "Beta-0.8";
    private GameObject currentVerGameObject;
    private GameObject newestVerGameObject;
    private GameObject updateAvailableGameObject;
    public bool updateAvailable = false;

    public void Start() {
        gameObject.transform.parent.Find("GoToGitHub").GetComponent<Button>().onClick.AddListener(() => Application.OpenURL("https://github.com/Stelios-Kourlis/Stardew-Valley-Planner-App"));

        currentVerGameObject = gameObject.transform.parent.Find("CurrentVer").gameObject;
        newestVerGameObject = gameObject.transform.parent.Find("NewestVer").gameObject;
        updateAvailableGameObject = gameObject.transform.parent.Find("UpdateAvailable").gameObject;
        currentVerGameObject.GetComponent<Text>().text = "Current Version: " + currentVersion;
        newestVerGameObject.GetComponent<Text>().text = "Newest Version: -";
        updateAvailableGameObject.SetActive(false);

        StartCoroutine(AskGitHubForReleaseDate());
    }

    public bool IsUpdateAvailable() {
        CheckForUpdate();
        return updateAvailable;
    }

    public void CheckForUpdate() {
        StartCoroutine(AskGitHubForReleaseDate());
    }

    private IEnumerator AskGitHubForReleaseDate() {
        string url = "https://api.github.com/repos/Stelios-Kourlis/Stardew-Valley-Planner-App/releases";
        using UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success) { Debug.Log(www.error); yield return false; }
        string[] lines = www.downloadHandler.text.Split('\n');
        GetNewestVersionFromResponse(lines);
    }

    private void GetNewestVersionFromResponse(string[] response) {
        foreach (string line in response) {
            if (line.Contains("tag_name")) {
                string newestVersion = line.Split()[5][1..^2];
                newestVerGameObject.GetComponent<Text>().text = "Newest Version: " + newestVersion;
                updateAvailableGameObject.SetActive(true);
                if (newestVersion != currentVersion) {
                    updateAvailableGameObject.GetComponent<Text>().color = Color.green;
                    updateAvailableGameObject.GetComponent<Text>().text = "Update Available!";
                    GameObject.FindGameObjectWithTag("UpdateIcon").GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/SettingsUpdate");
                }
                else {
                    updateAvailableGameObject.GetComponent<Text>().color = Color.red;
                    updateAvailableGameObject.GetComponent<Text>().text = "No Updates Available :(";
                    GameObject.FindGameObjectWithTag("UpdateIcon").GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/NoSettingsUpdate");
                }
                updateAvailable = newestVersion != currentVersion;
                break;
            }
        }
        updateAvailable = false;
    }
}
