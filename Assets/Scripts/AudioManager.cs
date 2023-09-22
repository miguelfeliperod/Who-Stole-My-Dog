using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public AudioPool audioPool;

    [SerializeField] Slider masterSlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;

    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource sfxSource;
    [SerializeField] AudioSource chargeSfxSource;
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

    private void Start()
    {
        audioMixer.SetFloat(masterMixerName, Mathf.Log10(PlayerPrefs.GetFloat(masterMixerName)) * 20);
        audioMixer.SetFloat(musicMixerName, Mathf.Log10(PlayerPrefs.GetFloat(musicMixerName)) * 20);
        audioMixer.SetFloat(sfxMixerName, Mathf.Log10(PlayerPrefs.GetFloat(sfxMixerName)) * 20);

        if (masterSlider != null)
        {
            masterSlider.value = PlayerPrefs.GetFloat(masterMixerName, 0.8f);
            musicSlider.value = PlayerPrefs.GetFloat(musicMixerName, 0.8f);
            sfxSlider.value = PlayerPrefs.GetFloat(sfxMixerName, 0.8f);
        }
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
    public void FadeInMusic(AudioClip audioClip, float duration = 1, float volume =1)
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

    public void StartCrossFadeMusic(AudioClip audioClip, float duration, float volume = 1)
    {
        StartCoroutine(CrossFadeMusic(audioClip, duration, volume = 1));
    }

    public IEnumerator CrossFadeMusic(AudioClip audioClip, float duration, float volume = 1)
    {
        FadeOutMusic(duration/2);
        yield return new WaitForSeconds(duration/2);
        FadeInMusic(audioClip, duration / 2, volume);
    }

    public void PlayTestSFX(AudioClip audioClip)
    {
        if (sfxSource.isPlaying) return;
        sfxSource.PlayOneShot(audioClip);
    }

    public void PlaySFX(AudioClip audioClip) => PlaySFX(audioClip, false);

    public void PlaySFX(AudioClip audioClip, bool randomizePitch = false, float volume = 1f, float pitch = 1f, bool loop = false)
    {
        AudioSource sfxCurrentSource = GetSourceFromPool();
        sfxCurrentSource.clip = audioClip;
        sfxCurrentSource.volume = volume;
        sfxCurrentSource.loop = loop;
        sfxCurrentSource.pitch = randomizePitch ? Random.Range(.8f, 1.2f) : pitch;
        sfxCurrentSource.loop = false;

        sfxCurrentSource.PlayOneShot(audioClip);
    }

    public AudioSource PlaySFX(AudioClip audioClip, bool loop = false)
    {
        AudioSource sfxCurrentSource = GetSourceFromPool();
        sfxCurrentSource.clip = audioClip;
        sfxCurrentSource.pitch = 1;
        sfxCurrentSource.loop = false;

        sfxCurrentSource.PlayOneShot(audioClip);
        return sfxCurrentSource;
    }

    public void StopChargeAudioSource() => chargeSfxSource.Stop();

    public void FadeInSFXLoop(AudioClip audioClip, float duration = 1, bool randomizePitch = false, float volume = 0.6f, float pitch = 1f)
    {
        chargeSfxSource.clip = audioClip;
        chargeSfxSource.volume = volume;
        chargeSfxSource.pitch = randomizePitch ? Random.Range(.8f, 1.2f) : pitch;

        chargeSfxSource.Play();
        StartCoroutine(FadeInSFXCoroutine(duration, volume));
    }

    private IEnumerator FadeInSFXCoroutine(float duration = 1, float volume = 1f)
    {
        float deltaTime = 0f;

        while (deltaTime < duration)
        {
            deltaTime += Time.deltaTime;
            chargeSfxSource.volume = Mathf.Lerp(0, volume, deltaTime / duration);
            chargeSfxSource.pitch = Mathf.Lerp(0.2f, 1f, deltaTime / duration);
            yield return null;
        }
        chargeSfxSource.volume = volume;
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

    public AudioClip GetCurrentLevelMusic(string currentLevelName)
    {
        switch (currentLevelName.ToLower())
        {
            case "level1":
                return audioPool.Level1;
            case "level2":
                return audioPool.Level2;
            case "level3":
                return audioPool.Level3;
            case "credits":
                return audioPool.Credits;
            case "Menu":
            default:
                return audioPool.Menu;
        }
    }
}
