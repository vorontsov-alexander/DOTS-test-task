using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MenuUISystem : MonoBehaviour
{
    public VisualElement Ui;
    public Button StartButton;
    public Button QuitButton;

    private void Awake()
    {
        Ui = GetComponent<UIDocument>().rootVisualElement;
    }

    private void OnEnable()
    {
        StartButton = Ui.Q<Button>("startButton");
        StartButton.clicked += OnStartGame;
        
        QuitButton = Ui.Q<Button>("exitButton");
        QuitButton.clicked += OnQuitGame;
    }

    private void OnQuitGame()
    {
        Application.Quit();
    }

    private void OnStartGame()
    {
        SceneManager.LoadScene("GameScene");
    }
}