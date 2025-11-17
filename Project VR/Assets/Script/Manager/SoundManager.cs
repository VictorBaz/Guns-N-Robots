using System;
using Script.Controller;
using UnityEngine;

namespace Script.Manager
{
    public class SoundManager : MonoBehaviour
    {
        #region Fields

        public AudioSource audioSource;
        [SerializeField] private AudioSource musicAudioSource;

        [Range(0,1)] public float masterVolume = 1f;

        public AudioClip emptyLoad;
        public AudioClip fullLoad;
        
        [Header("Music BPM Settings")]
        [SerializeField] private AudioClip gameMusic;
        [SerializeField] private float musicBPM = 90f;

        #endregion

        #region Singleton

        private static SoundManager instance;
        
        public static SoundManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindFirstObjectByType<SoundManager>();
                    
                    if (instance == null)
                    {
                        GameObject soundManager = new GameObject("SoundManager");
                        instance = soundManager.AddComponent<SoundManager>();
                    }
                }
                return instance;
            }
        }
        #endregion

        #region Unity Methods

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
            
            SetupMusicAudioSource();
        }
        
        private void Start()
        {
            audioSource.volume = masterVolume; 
        }

        #endregion

        #region Music Setup

        private void SetupMusicAudioSource()
        {
            if (musicAudioSource == null)
            {
                GameObject musicObject = new GameObject("MusicAudioSource");
                musicObject.transform.SetParent(transform);
                musicAudioSource = musicObject.AddComponent<AudioSource>();
            }

            musicAudioSource.loop = true;
            musicAudioSource.playOnAwake = false;
            musicAudioSource.volume = masterVolume;
        }

        public void StartGameMusic()
        {
            if (gameMusic != null && musicAudioSource != null)
            {
                musicAudioSource.clip = gameMusic;
                musicAudioSource.Play();
            }
        }

        public void StopGameMusic()
        {
            if (musicAudioSource != null && musicAudioSource.isPlaying)
            {
                musicAudioSource.Stop();
            }
        }

        public void PauseGameMusic()
        {
            if (musicAudioSource != null && musicAudioSource.isPlaying)
            {
                musicAudioSource.Pause();
            }
        }

        public void ResumeGameMusic()
        {
            if (musicAudioSource != null && !musicAudioSource.isPlaying)
            {
                musicAudioSource.UnPause();
            }
        }

        public float GetBeatInterval()
        {
            return 60f / musicBPM;
        }

        public void UpdateMusicSpeed(float currentTimeBetweenTick, float defaultTimeBetweenTick)
        {
            if (musicAudioSource != null && musicAudioSource.isPlaying)
            {
                float speedRatio = defaultTimeBetweenTick / currentTimeBetweenTick;
                musicAudioSource.pitch = speedRatio;
            }
        }

        #endregion

        #region Sound Methods

        public void PlayMusicOneShot(AudioClip _audioClip)
        {
            if (_audioClip == null)
            {
                Debug.LogError("The audioClip you tried to play is null");
                return;
            }
            audioSource.PlayOneShot(_audioClip);
        }
        
        
        public void UpdateMasterVolume(float volume)
        {
            masterVolume = volume;
            audioSource.volume = masterVolume;
            if (musicAudioSource != null)
            {
                musicAudioSource.volume = masterVolume;
            }
        }

        public GameObject InitialisationAudioObjectDestroyAtEnd(AudioClip audioClipTarget, bool looping, 
            bool playingAwake, float volumeSound, string _name)
        {
            GameObject emptyObject = new GameObject(_name);
            emptyObject.transform.SetParent(gameObject.transform);

            AudioSource audioSourceGeneral = emptyObject.AddComponent<AudioSource>();
            audioSourceGeneral.clip = audioClipTarget;
            audioSourceGeneral.loop = looping;
            audioSourceGeneral.playOnAwake = playingAwake;
            audioSourceGeneral.volume = volumeSound * masterVolume;
            audioSourceGeneral.Play();
            
            if (!looping)
            {
                Destroy(emptyObject, audioClipTarget.length);
            }
            
            return emptyObject;
        }

        #endregion
        
        #region Observer

        private void OnEnable()
        {
            EventManager.OnGameStart += StartGameMusic;
            EventManager.OnGameEnd += StopGameMusic;
            //EventManager.OnRoundEnd += PauseGameMusic;
            //EventManager.OnRoundStart += ResumeGameMusic;
        }

        private void OnDisable()
        {
            EventManager.OnGameStart -= StartGameMusic;
            EventManager.OnGameEnd -= StopGameMusic;
            //EventManager.OnRoundEnd -= PauseGameMusic;
            //EventManager.OnRoundStart -= ResumeGameMusic;
        }

        #endregion
    }
}