using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Message : MonoBehaviour
{
    public GameObject FirstButton;

    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(FirstButton);
    }

    public void GoToGame()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}