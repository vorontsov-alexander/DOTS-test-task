using Components;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Systems
{
    [RequireComponent(typeof(UIDocument))]
    public class GameUISystem : MonoBehaviour
    {
        private VisualElement _root;
        private Button _restartButton;
        private Button _quitButton;
        private Button _pauseButton;
        private VisualElement _pauseMenu;
        private Label _healthLabel;

        private EntityManager _entityManager;
        private EntityQuery _playerQuery;
        private bool isDead;
        
        private void Awake()
        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            isDead = false;
            _playerQuery = _entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<PlayerTag>(),
                ComponentType.ReadOnly<PlayerHealthComponent>()
            );

            var uiDocument = GetComponent<UIDocument>();
            _root = uiDocument.rootVisualElement;

            _healthLabel = _root.Q<Label>("healthLabel");
            _restartButton = _root.Q<Button>("restartButton");
            _quitButton = _root.Q<Button>("quitButton");
            _pauseButton = _root.Q<Button>("pauseButton");
            _pauseMenu = _root.Q<VisualElement>("pauseMenu");

            _pauseMenu.style.display = DisplayStyle.None;
        }

        private void OnEnable()
        {
            _restartButton.clicked += OnRestartGame;
            _quitButton.clicked += OnQuitGame;
            _pauseButton.clicked += OnPauseClicked;
        }

        private void OnDisable()
        {
            _restartButton.clicked -= OnRestartGame;
            _quitButton.clicked -= OnQuitGame;
            _pauseButton.clicked -= OnPauseClicked;
        }

        private void Update()
        {
            if (_playerQuery.IsEmpty || isDead) 
                return;
            PlayerHealthComponent healthComponent = _playerQuery.GetSingleton<PlayerHealthComponent>();
            if (healthComponent.Health <= 0)
            {
                healthComponent.Health = 0;
                OnPauseClicked();
                isDead = true;
            }
            _healthLabel.text = $"Здоровье: {healthComponent.Health}";
        }

        private void OnPauseClicked()
        {
            bool shouldPause = _pauseMenu.style.display == DisplayStyle.None;
            _pauseMenu.style.display = shouldPause ? DisplayStyle.Flex : DisplayStyle.None;
            Time.timeScale = shouldPause ? 0f : 1f;
        }

        private void OnQuitGame()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenu");
        }

        private void OnRestartGame()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
