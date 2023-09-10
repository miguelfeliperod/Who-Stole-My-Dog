using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }

    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource sfxSource;
    [SerializeField] AudioMixer audioMixer;

    [SerializeField] GameObject OSTComponents;
    [SerializeField] GameObject SFXComponents;
    public AudioMixer AudioMixer { get { return audioMixer; } }
    [SerializeField] List<AudioSource> sfxSourcePool = null;

    [SerializeField] AudioMixerGroup masterMixer;
    public AudioMixerGroup MasterMixer { get => masterMixer; }
    [SerializeField] AudioMixerGroup musicMixer;
    public AudioMixerGroup MusicMixer { get => musicMixer; }
    [SerializeField] AudioMixerGroup sfxMixer;
    public AudioMixerGroup SfxMixer { get => sfxMixer; }

    public static string masterMixerName = "masterVolume";
    public static string musicMixerName = "musicVolume";
    public static string sfxMixerName = "sfxVolume";

    [SerializeField] GameObject configMenu;

    float masterVolume;
    float musicVolume;
    float sfxVolume;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one AudioManager in the scene");
            Destroy(gameObject);
        }
        instance = this;
    }

    private void Start()
    {
        print("master: " + PlayerPrefs.GetFloat(masterMixerName, 0.8f));
        print("music: " + PlayerPrefs.GetFloat(musicMixerName, 0.8f));
        print("sfx: " + PlayerPrefs.GetFloat(sfxMixerName, 0.8f));

        audioMixer.SetFloat(masterMixerName, Mathf.Log10(PlayerPrefs.GetFloat(masterMixerName)) * 20);
        audioMixer.SetFloat(musicMixerName, Mathf.Log10(PlayerPrefs.GetFloat(musicMixerName)) * 20);
        audioMixer.SetFloat(sfxMixerName, Mathf.Log10(PlayerPrefs.GetFloat(sfxMixerName)) * 20);
    }

    public void PlayMusic(AudioClip audioClip, float volume = 1.0f)
    {
        musicSource.Stop();
        musicSource.clip = audioClip;
        musicSource.volume = volume;
        musicSource.Play();
    }
    public void StopMusic()
    {
        if (musicSource.isPlaying)
            musicSource.Stop();
    }
    public void PauseMusic()
    {
        if (musicSource.isPlaying)
            musicSource.Pause();
        else
            musicSource.UnPause();
    }
    public void FadeInMusic(AudioClip audioClip, float duration, float volume)
    {
        if (musicSource.isPlaying)
        {
            musicSource.Stop();
        }
        musicSource.clip = audioClip;
        musicSource.volume = 0;
        musicSource.Play();
        StartCoroutine(FadeInMusicCoroutine(duration, volume));
    }

    public void FadeOutMusic(float duration)
    {
        StartCoroutine(FadeOutMusicCoroutine(duration));
    }

    public IEnumerator CrossFadeMusic(AudioClip audioClip, float duration, float volume = 1)
    {
        StartCoroutine(FadeOutMusicCoroutine(duration/2));
        yield return new WaitForSeconds(duration / 2);
        StartCoroutine(FadeInMusicCoroutine(duration/2, volume));
    }

    public void PlayTestSFX(AudioClip audioClip)
    {
        if (sfxSource.isPlaying) return;
        sfxSource.PlayOneShot(audioClip);
    }

    public void PlaySFX(AudioClip audioClip, bool randomizePitch = false, float volume = 1f, float pitch = 1f)
    {
        AudioSource sfxCurrentSource = GetSourceFromPool();
        sfxCurrentSource.clip = audioClip;
        sfxCurrentSource.volume = volume;
        sfxCurrentSource.pitch = randomizePitch ? Random.Range(.8f, 1.2f) : pitch;

        sfxCurrentSource.PlayOneShot(audioClip);
    }

    public void PlaySFXWithPitchRange(AudioClip audioClip, float minPitch, float maxPitch, float volume = 1f)
    {
        AudioSource sfxCurrentSource = GetSourceFromPool();
        sfxCurrentSource.clip = audioClip;
        sfxCurrentSource.volume = volume;
        sfxCurrentSource.pitch = Random.Range(minPitch, maxPitch);

        sfxCurrentSource.PlayOneShot(audioClip);
    }

    public void PlayDelayedSFX(AudioClip audioClip, float delayTime, bool randomizePitch = false, float volume = 1f, float pitch = 1f)
    {
        AudioSource sfxCurrentSource = GetSourceFromPool();
        sfxCurrentSource.clip = audioClip;
        sfxCurrentSource.volume = volume;
        sfxCurrentSource.pitch = randomizePitch ? Random.Range(.8f, 1.2f) : pitch;

        sfxCurrentSource.PlayDelayed(delayTime);
    }

    public AudioSource GetSourceFromPool()
    {
        if (sfxSource.isPlaying)
        {
            foreach (AudioSource source in sfxSourcePool)
            {
                if (!source.isPlaying)
                    return source;
            }
            GameObject sfxSourceObject = new GameObject("SFXSource");
            sfxSourceObject.transform.parent = transform;
            sfxSourceObject.AddComponent<AudioSource>();
            var newSfxSourceComponent = sfxSourceObject.GetComponent<AudioSource>();
            sfxSourcePool.Add(newSfxSourceComponent);
            newSfxSourceComponent.outputAudioMixerGroup = sfxMixer;
            return newSfxSourceComponent;
        }
        else
        {
            return sfxSource;
        }
    }

    public void ToggleMuteSounds() { }

    private IEnumerator FadeInMusicCoroutine(float time, float volume = 1f)
    {
        float deltaTime = 0f;
        float currentVolume = musicSource.volume;

        while (deltaTime < time)
        {
            deltaTime += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(currentVolume, volume, deltaTime / time);
            yield return null;
        }
        musicSource.volume = volume;
    }

    private IEnumerator FadeOutMusicCoroutine(float time)
    {
        float deltaTime = 0f;
        float currentVolume = musicSource.volume;

        while (deltaTime < time)
        {
            deltaTime += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(currentVolume, 0f, deltaTime / time);
            yield return null;
        }
        musicSource.Stop();
    }

    public void SetMasterVolume(float sliderValue)
    {
        audioMixer.SetFloat(masterMixerName, Mathf.Log10(sliderValue) * 20);
        masterVolume = sliderValue;
    }

    public void SetMusicVolume(float sliderValue)
    {
        audioMixer.SetFloat(musicMixerName, Mathf.Log10(sliderValue) * 20);
        musicVolume = sliderValue;
    }

    public void SetSfxVolume(float sliderValue)
    {
        audioMixer.SetFloat(sfxMixerName, Mathf.Log10(sliderValue) * 20);
        sfxVolume = sliderValue;
    }

    public void OnConfigButtonClick()
    {
        configMenu.SetActive(true);
    }
    public void OnCloseButtonClick()
    {
        PlayerPrefs.SetFloat(masterMixerName, masterVolume);
        PlayerPrefs.SetFloat(musicMixerName, musicVolume);
        PlayerPrefs.SetFloat(sfxMixerName, sfxVolume);
        configMenu.SetActive(false);
    }
}
