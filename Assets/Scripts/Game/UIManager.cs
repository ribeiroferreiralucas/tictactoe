using UnityEngine;
using UnityEngine.UI;

public class UIManager: MonoBehaviour
{
    public static UIManager Instance;

    public GameObject StartMenu;
    public InputField UsernameField;

    private void Awake() 
    {
        if( Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.Log("Instance already existis, destroing object!");
            Destroy(this);
        }
    }

    public void Connect()
    {
        StartMenu.SetActive(false);
        UsernameField.interactable = false;

        Client.Instance.ConnectToServer();
    }
}