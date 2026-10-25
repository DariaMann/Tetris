using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource soundSource;

    [SerializeField] private AudioClip clickSound;
    [SerializeField] private AudioClip clickChipSound;
    [SerializeField] private AudioClip successLineSound;
    [SerializeField] private AudioClip checkersDownSound;
    [SerializeField] private AudioClip click4Sound;
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioClip timerSound;
    [SerializeField] private AudioClip timerFinishSound;
    [SerializeField] private AudioClip startLogoSound;
    [SerializeField] private AudioClip endLogoSound;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Чтобы не удалялся при смене сцен
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "Menu") StartMusic();
    }

    public void StartLogoSound()
    {
        if (!GameHelper.Sound)
        {
            return;
        }
        soundSource.PlayOneShot(startLogoSound);
    }
    
    public void EndLogoSound()
    {
        if (!GameHelper.Sound)
        {
            return;
        }
        soundSource.PlayOneShot(endLogoSound);
    }

    public void StartMusic()
    {
        musicSource.mute = !GameHelper.Music;
        if (!musicSource.isPlaying) musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }
    
    public void PlayWinsSound()
    {
        if (!GameHelper.Sound)
        {
            return;
        }
        soundSource.PlayOneShot(winSound);
    }
    
    public void PlayClick4Sound()
    {
        if (!GameHelper.Sound)
        {
            return;
        }
        soundSource.PlayOneShot(click4Sound);
    }   
    
    public void PlayCheckersDownSound()
    {
        if (!GameHelper.Sound)
        {
            return;
        }
        soundSource.PlayOneShot(checkersDownSound);
    }

    public void PlayClickSound()
    {
        if (!GameHelper.Sound)
        {
            return;
        }
        soundSource.PlayOneShot(clickSound);
    }   
    
    public void PlayTimerSound()
    {
        if (!GameHelper.Sound)
        {
            return;
        }
        soundSource.PlayOneShot(timerSound);
    }
    
    public void PlayTimerFinishSound()
    {
        if (!GameHelper.Sound)
        {
            return;
        }
        soundSource.PlayOneShot(timerFinishSound);
    }

    public void PlayClickChipSound()
    {
        if (!GameHelper.Sound)
        {
            return;
        }
        soundSource.PlayOneShot(clickChipSound);
    }  
    
    public void PlaySuccessLineSound()
    {
        if (!GameHelper.Sound)
        {
            return;
        }
        soundSource.PlayOneShot(successLineSound);
    }

    public void ToggleMusic(bool isOn)
    {
        musicSource.mute = !isOn;
    }

    public void ToggleSound(bool isOn)
    {
        soundSource.mute = !isOn;
    }
}
