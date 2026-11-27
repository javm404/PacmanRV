using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    private const string GameSceneName = "SampleScene";

    [SerializeField] private Slider volumeSlider;
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip gameMusic;

    private static AudioManager instance;
    private AudioSource musicSource;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            enabled = false;
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        musicSource = GetComponent<AudioSource>();
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
        }

        musicSource.loop = true;
        musicSource.playOnAwake = false;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        if (!PlayerPrefs.HasKey("musicVolume"))
        {
            PlayerPrefs.SetFloat("musicVolume", 1);
        }

        AttachVolumeSlider();
        Load();
        PlayMusicForScene(SceneManager.GetActiveScene().name);
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            instance = null;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AttachVolumeSlider();
        PlayMusicForScene(scene.name);
    }

    private void AttachVolumeSlider()
    {
        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.RemoveListener(OnVolumeSliderChanged);
        }

        volumeSlider = FindAnyObjectByType<Slider>();
        if (volumeSlider != null)
        {
            volumeSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("musicVolume", 1));
            volumeSlider.onValueChanged.AddListener(OnVolumeSliderChanged);
        }
    }

    private void PlayMusicForScene(string sceneName)
    {
        AudioClip clip = sceneName == GameSceneName ? gameMusic : menuMusic;
        if (clip == null)
        {
            Debug.LogError("No music clip is assigned for scene: " + sceneName, this);
            return;
        }

        if (musicSource.clip == clip && musicSource.isPlaying)
        {
            return;
        }

        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void ChangeVolume()
    {
        if (volumeSlider == null)
        {
            Debug.LogError("AudioManager requires a volume slider to change the music volume.", this);
            return;
        }

        SetMusicVolume(volumeSlider.value);
    }

    private void OnVolumeSliderChanged(float volume)
    {
        SetMusicVolume(volume);
    }

    private void SetMusicVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);
        musicSource.volume = volume;
        PlayerPrefs.SetFloat("musicVolume", volume);
    }

    private void Load()
    {
        float volume = Mathf.Clamp01(PlayerPrefs.GetFloat("musicVolume"));
        musicSource.volume = volume;
        if (volumeSlider != null)
        {
            volumeSlider.SetValueWithoutNotify(volume);
        }
    }
}
