using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Credits : MonoBehaviour
{
    public GameObject FirstButton;

    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(FirstButton);
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}