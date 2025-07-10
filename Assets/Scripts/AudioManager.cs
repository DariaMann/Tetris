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