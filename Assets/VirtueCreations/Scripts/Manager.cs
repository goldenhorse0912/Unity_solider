using UnityEngine;

namespace VIRTUE
{
    [DefaultExecutionOrder(-2)]
    public class Manager : MonoBehaviour
    {
        public static Manager Instance { get; private set; }

        public VirtueAnalytics VirtueAnalytics { get; private set; }

        // public AdManager AdManager { get; private set; }
        public CameraFollow CameraFollow { get; private set; }
        public CameraController CameraController { get; private set; }
        public PlayerController PlayerController { get; private set; }
        public GameController GameController { get; private set; }
        public UIManager UIManager { get; private set; }
        public AudioManager AudioManager { get; private set; }
        public PoolManager PoolManager { get; private set; }
        public AssetManager AssetManager { get; private set; }
        public EmojiController EmojiController { get; private set; }
        public Confetti Confetti { get; private set; }
        public FadeScreen FadeScreen { get; private set; }
        public NotificationManager NotificationManager { get; private set; }
        public LevelManager LevelManager { get; private set; }
        public BuildingManager BuildingManager { get; private set; }
        public TroopManager TroopManager { get; private set; }

        void Awake()
        {
            Application.targetFrameRate = 60;
            Instance = this;
            VirtueAnalytics = FindObjectOfType<VirtueAnalytics>();
            CameraFollow = FindObjectOfType<CameraFollow>();
            CameraController = FindObjectOfType<CameraController>();
            PlayerController = FindObjectOfType<PlayerController>();
            PoolManager = FindObjectOfType<PoolManager>();
            AssetManager = FindObjectOfType<AssetManager>();
            NotificationManager = FindObjectOfType<NotificationManager>();
            FadeScreen = FindObjectOfType<FadeScreen>();
            GameController = GetComponentInChildren<GameController>();
            UIManager = GetComponentInChildren<UIManager>();
            AudioManager = GetComponentInChildren<AudioManager>();
            // EmojiController = PlayerController.GetComponentInChildren<EmojiController> ();
            Confetti = GetComponentInChildren<Confetti>();
            LevelManager = GetComponentInChildren<LevelManager>();
            BuildingManager = GetComponentInChildren<BuildingManager>(true);
            TroopManager = GetComponentInChildren<TroopManager>();
        }
    }
}