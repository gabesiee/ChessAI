using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] GameObject StartScreen;
    [SerializeField] GameObject DifficultyScreen;
    [SerializeField] GameObject Slider;

    public static int difficulty;

    private void Start()
    {
        if (SceneManager.GetSceneByName("MenuScene").isLoaded)
        {
            StartScreen.SetActive(true);
            DifficultyScreen.SetActive(false);
        }
    }

    public void PlayButtonClicked()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void DifficultyButtonClicked()
    {
        StartScreen.SetActive(false);
        DifficultyScreen.SetActive(true);
    }

    public void QuickButtonClicked()
    {
        Application.Quit();
    }

    public void BackButtonClicked()
    {
        StartScreen.SetActive(true);
        DifficultyScreen.SetActive(false);

        difficulty = (int)Slider.GetComponent<Slider>().value;
    }

    public void MainMenuButtonClicked()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
