using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject FirstButton;
    public GameObject OptionsFirstButton;
    public GameObject OptionsButton;
    public GameObject Options_Menu;
    public GameObject Main_Menu;

    void Start()
    {
        Options_Menu.GetComponent<Transform>().localScale = new Vector3(0, 0, 0);
        EventSystem.current.SetSelectedGameObject(FirstButton);
    }

    public void Exit()
    {
        Debug.Log("Exit");
        Application.Quit();
    }

    public void NewGame()
    {
        //SceneManager.LoadScene("SampleScene");
        Debug.Log("New Game");
    }

    public void Continue()
    {
        //SceneManager.LoadScene("SampleScene");
        Debug.Log("Continue");
    }

    public void Credits()
    {
        //SceneManager.LoadScene("Credits");
        Debug.Log("Credits");
    }

    public void Options()
    {
        Debug.Log("Options");
        Main_Menu.GetComponent<Transform>().localScale = new Vector3(0, 0, 0);
        Options_Menu.GetComponent<Transform>().localScale = new Vector3(1, 1, 1);
        EventSystem.current.SetSelectedGameObject(OptionsFirstButton);
    }

    public void Back()
    {
        Debug.Log("Back");
        Main_Menu.GetComponent<Transform>().localScale = new Vector3(1, 1, 1);
        Options_Menu.GetComponent<Transform>().localScale = new Vector3(0, 0, 0);
        EventSystem.current.SetSelectedGameObject(OptionsButton);
    }
}
