using System;
using Script.Controller;
using Script.Enum;
using UnityEngine;
using UnityEngine.Audio;

namespace Script.Manager
{
    public class SoundManager : MonoBehaviour
    {
        /// <summary>
        /// IN THE FUTURE WILL USE A SCRIPTABLE OBJECT AS BANK DATA FOR SOUND
        /// </summary>
        
        #region Fields

        public AudioSource audioSource;
        [SerializeField] private AudioSource musicAudioSource;

        [Range(0,1)] public float masterVolume = 1f;

        public AudioClip emptyLoad;
        public AudioClip fullLoad;
        
        [Header("Music BPM Settings")]
        [SerializeField] private AudioClip gameMusic;
        [SerializeField] private float musicBPM = 90f;

        [Header("SFX")]
        
        [SerializeField] private AudioClip badShoot;
        [SerializeField] private AudioClip goodShoot;
        [SerializeField] private AudioClip perfectShoot;
        [SerializeField] private AudioClip missShoot;
        
        [SerializeField] private AudioClip reload;
        
        [SerializeField] private AudioClip robotWalk1;
        [SerializeField] private AudioClip robotWalk2;
        [SerializeField] private AudioClip robotAttackRange;
        [SerializeField] private AudioClip robotAttackMelee;
        [SerializeField] private AudioClip robotAttackMeleeCharge;
        [SerializeField] private AudioClip robotDeath;
        [SerializeField] private AudioClip woodHit;

        [SerializeField] private AudioClip doorSound;

        [SerializeField] private AudioClip roundSound;
        [SerializeField] private AudioClip fireSound;
        
        
        [Header("Audio Mixer Group")]
        [SerializeField] private AudioMixerGroup _audioMixerGroupMainMusic;

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
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void Start()
        {
            SetupMusicAudioSource();
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
            musicAudioSource.outputAudioMixerGroup = _audioMixerGroupMainMusic;
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

        public void PlaySoundWithAudioSource(AudioSource source, AudioClip clip)
        {
            if (clip == null)
            {
                Debug.LogError("The audioClip you tried to play is null");
                return;
            }
            source.PlayOneShot(clip);
        }
        
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
            EventManager.OnShootState += PlayShootSound;
            EventManager.OnReloadStart += PlayReloadSound;
        }

        private void OnDisable()
        {
            EventManager.OnGameStart -= StartGameMusic;
            EventManager.OnGameEnd -= StopGameMusic;
            EventManager.OnShootState -= PlayShootSound;
            EventManager.OnReloadStart -= PlayReloadSound;
        }

        #endregion

        #region Utility Sound
        
        public AudioClip BadShootSound()        => badShoot;
        public AudioClip GoodShootSound()       => goodShoot;
        public AudioClip PerfectShootSound()    => perfectShoot;

        public AudioClip ReloadSound()          => reload;


        public AudioClip RobotWalkSound1()       => robotWalk1;
        public AudioClip RobotWalkSound2()       => robotWalk2;
        public AudioClip RobotAttackRangeSound()=> robotAttackRange;
        public AudioClip RobotAttackMeleeSound()=> robotAttackMelee;
        public AudioClip RobotAttackMeleeSoundCharge()=> robotAttackMeleeCharge;
        public AudioClip RobotDeathSound()      => robotDeath;

        public AudioClip DoorSound()            => doorSound;

        public AudioClip RoundSound()           => roundSound;
        
        public AudioClip FireSound()           => fireSound;
        public AudioClip MissSound()           => missShoot;
        public AudioClip WoodHit()           => woodHit;
        

        #endregion

        #region Utility Method

        private void PlayShootSound(ShotDone shotState)
        {
            switch (shotState)
            {
                case ShotDone.Bad:
                    PlayMusicOneShot(BadShootSound());
                    break;
                case ShotDone.Good:
                    PlayMusicOneShot(GoodShootSound());
                    break;
                case ShotDone.Perfect:
                    PlayMusicOneShot(PerfectShootSound());
                    break;
                case ShotDone.Miss:
                    PlayMusicOneShot(MissSound());
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(shotState), shotState, null);
            }
        }

        private void PlayReloadSound()
        {
            PlayMusicOneShot(ReloadSound());
        }

        #endregion
    }
}