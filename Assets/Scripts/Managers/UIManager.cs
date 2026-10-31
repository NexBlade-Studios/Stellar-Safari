using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject settingsMenu;

    public void ShowSettingsMenu()
    {
        if (settingsMenu.activeInHierarchy)
        {
            settingsMenu.SetActive(false);
        }
        else { settingsMenu.SetActive(true); }
    }

    public void MainMenu(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
