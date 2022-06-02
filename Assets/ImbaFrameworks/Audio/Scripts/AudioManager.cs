/*
Kaka @ 2020
Audio manager: manage all sound in game
TODO: Pooling audio source
*/


using System.Collections;
using System.Collections.Generic;


using Imba.Utils;
using UnityEngine;

namespace Imba.Audio
{
	public class AudioManager : ManualSingletonMono<AudioManager>
	{

		#region VARIABLES

		private const float TIME_TO_CHECK_IDLE_AUDIO_SOURCE = 5f;

		public const string MUTE_MUSIC_KEY = "Imba_MuteMusic";
		
		public const string MUTE_SFX_KEY = "Imba_MuteSFX";

		private List<AudioData> _listAudioData;//get from database

		private float _timeToReset;

		private float _timeToCheckIdleAudioSource;

		private bool _timerIsSet;

		private string _tmpName;

		private float _tmpVol;

		private bool _isLowered;

		private bool _fadeOut;

		private bool _fadeIn;

		private string _fadeInUsedString;

		private string _fadeOutUsedString;

		private bool _isMuteMusic;

		private bool _isMuteSfx;
		
		public bool IsMuteMusic
		{
			get { return _isMuteMusic; }
		}

		public bool IsMuteSfx
		{
			get { return _isMuteSfx; }
		}

		#endregion

		#region UNITY METHOD
		


		// Use this for initialization
		public override void Awake()
		{
			base.Awake();

			_isMuteMusic = PlayerPrefs.GetInt(MUTE_MUSIC_KEY, 0) == 1;
			_isMuteSfx = PlayerPrefs.GetInt(MUTE_SFX_KEY, 0) == 1;

			_listAudioData = new List<AudioData>();

			var database = AudioDataManager.Instance.Database;
			
			foreach (var d in database)
			{
				foreach (var s in d.Database)
				{
					_listAudioData.Add(s);

					if (s.playOnAwake)
					{
						s.source = CreateAudioSource(s);
						if (IsMuteAudio(s.type))
						{
							s.source.mute = true;
						}

						s.source.Play();
					}
				}
			}
		}

		#endregion

		#region CLASS METHODS

		private AudioSource CreateAudioSource(AudioData a)
		{
			AudioSource s = Instance.gameObject.AddComponent<AudioSource>();
			s.clip = a.audioClip;
			s.volume = a.volume;
			s.playOnAwake = a.playOnAwake;
			s.priority = a.priority;
			s.loop = a.isLooping;
			return s;
		}

		private bool IsMuteAudio(AudioType type)
		{
			if (type == AudioType.BGM && IsMuteMusic) return true;
			if (type == AudioType.SFX && IsMuteSfx) return true;

			return false;
		}

		private AudioData GetAudioData(string audioName)
		{
			AudioData s = _listAudioData.Find(a => a.audioName == audioName);

			return s;
		}

		public void PlaySFX(string audioName)
		{
			if (IsMuteSfx || string.IsNullOrEmpty(audioName)) return;

			AudioData s = GetAudioData(audioName);
			if (s == null)
			{
				Debug.LogError("Sound name" + audioName + "not found!");
			}
			else
			{
				if (s.source == null)
				{
					s.source = CreateAudioSource(s);
				}

				s.source.PlayOneShot(s.audioClip, s.volume);
			}
		}

		public void PlaySoundFromSource(string audioName, AudioSource audioSource, bool isChangeSound = false)
		{
			if (IsMuteSfx || string.IsNullOrEmpty(audioName)) return;

			if (audioSource == null) return;

			if (audioSource.clip == null || isChangeSound)
			{
				AudioData s = GetAudioData(audioName);
				if (s == null)
				{
					Debug.LogError("Sound name" + audioName + "not found!");
					return;
				}

				if (audioSource.clip == null)
				{
					audioSource.clip = s.audioClip;
					audioSource.volume = s.volume;
					audioSource.priority = s.priority;
					audioSource.playOnAwake = s.playOnAwake;
					audioSource.spatialBlend = s.spatialBlend;
					audioSource.rolloffMode = AudioRolloffMode.Linear;
					audioSource.minDistance = 1;
					audioSource.maxDistance = 50;
				}
			}

			audioSource.PlayOneShot(audioSource.clip, audioSource.volume);

		}


		public void PlayMusic(string audioName)
		{
			if (IsMuteMusic || string.IsNullOrEmpty(audioName)) return;

			AudioData s = GetAudioData(audioName);
			if (s == null)
			{
				Debug.LogError("Sound name" + audioName + "not found!");
				return;
			}

			if (s.source == null)
			{
				s.source = CreateAudioSource(s);
			}

			if (!s.source.isPlaying)
			{
				s.source.Play();
			}
		}

		public void MuteMusic(bool isMute = true)
		{
			_isMuteMusic = isMute;
			
			PlayerPrefs.SetInt(MUTE_MUSIC_KEY, _isMuteMusic? 1: 0);
			PlayerPrefs.Save();
			
			foreach (var s in _listAudioData)
			{
				if (s.audioClip == null)
					continue;
				if (s.type == AudioType.BGM)
				{
					if (s.source == null)
					{
						continue;
					}

					s.source.volume = (IsMuteMusic) ? 0f : 1f;
				}
			}
			
		}

		public void MuteSfx(bool isMute = true)
		{
			
			_isMuteSfx = isMute;
			PlayerPrefs.SetInt(MUTE_SFX_KEY, _isMuteSfx? 1: 0);
			PlayerPrefs.Save();
			
			foreach (var s in _listAudioData)
			{
				if (s.audioClip == null)
					continue;
				if (s.type == AudioType.SFX)
				{
					if (s.source == null)
					{
						continue;
					}

					s.source.mute = IsMuteSfx;
				}
			}
			
		}

		public void StopMusic(string audioName)
		{
			AudioData s = GetAudioData(audioName);
			if (s == null)
			{
				Debug.LogError("Sound name" + audioName + "not found!");

			}
			else
			{
				if (s.type == AudioType.BGM && s.source != null)
				{
					s.source.Stop();
				}
			}
		}

		public void PauseMusic(string audioName)
		{
			AudioData s = GetAudioData(audioName);
			if (s == null)
			{
				Debug.LogError("Sound name" + audioName + "not found!");
			}
			else
			{
				if (s.type == AudioType.BGM && s.source != null)
				{
					s.source.Pause();
				}
			}
		}

		public void UnPauseMusic(string audioName)
		{
			AudioData s = GetAudioData(audioName);
			if (s == null)
			{
				Debug.LogError("Sound name" + audioName + "not found!");
			}
			else
			{
				if (s.type == AudioType.BGM && s.source != null)
				{
					s.source.UnPause();
				}
			}
		}

		public void LowerVolume(string audioName, float duration)
		{
			if (Instance._isLowered == false)
			{
				AudioData s = GetAudioData(audioName);
				if (s == null)
				{
					Debug.LogError("Sound name" + audioName + "not found!");
					return;
				}
				else
				{
					Instance._tmpName = audioName;
					Instance._tmpVol = s.volume;
					Instance._timeToReset = Time.time + duration;
					Instance._timerIsSet = true;
					s.source.volume = s.source.volume / 3;
				}

				Instance._isLowered = true;
			}
		}

		public void FadeOut(string audioName, float duration)
		{
			Instance.StartCoroutine(Instance.IFadeOut(audioName, duration));
		}

		public void FadeIn(string audioName, float duration)
		{
			Instance.StartCoroutine(Instance.IFadeIn(audioName, duration));
		}



		//not for use
		private IEnumerator IFadeOut(string audioName, float duration)
		{
			AudioData s = GetAudioData(audioName);
			if (s == null)
			{
				Debug.LogError("Sound name" + name + "not found!");
				yield return null;
			}
			else
			{
				if (_fadeOut == false)
				{
					_fadeOut = true;
					float startVol = s.source.volume;
					_fadeOutUsedString = name;
					while (s.source.volume > 0)
					{
						s.source.volume -= startVol * Time.deltaTime / duration;
						yield return null;
					}

					s.source.Stop();
					yield return new WaitForSeconds(duration);
					_fadeOut = false;
				}
				else
				{
					Debug.Log("Could not handle two fade outs at once : " + name + " , " + _fadeOutUsedString +
					          "! Stopped the music " + name);
					StopMusic(audioName);
				}
			}
		}

		private IEnumerator IFadeIn(string audioName, float duration)
		{
			AudioData s = GetAudioData(audioName);
			if (s == null)
			{
				Debug.LogError("Sound name" + audioName + "not found!");
				yield return null;
			}
			else
			{
				if (s.source == null)
				{
					s.source = CreateAudioSource(s);
				}

				//mute => volum = 0
				//if (_isMuteMusic)
				//    s.Source.volume = 0f;
				if (s.source.isPlaying)
					yield return null;
				else
				{
					if (_fadeIn == false)
					{
						_fadeIn = true;
						Instance._fadeInUsedString = name;
						s.source.volume = 0f;
						s.source.Play();
						if (IsMuteMusic) //check mute music
						{
							s.source.volume = 0f;
							_fadeIn = false;
							yield break;
						}

						float targetVolume = s.volume;
						while (s.source.volume < targetVolume)
						{
							s.source.volume += Time.deltaTime / duration;
							yield return null;
						}

						yield return new WaitForSeconds(duration);
						_fadeIn = false;
					}
					else
					{
						Debug.Log("Could not handle two fade ins at once: " + name + " , " + _fadeInUsedString +
						          "! Played the music " + name);
						StopMusic(audioName);
						PlayMusic(audioName);
					}
				}
			}
		}

		void ResetVol()
		{
			AudioData s = GetAudioData(_tmpName);
			s.source.volume = _tmpVol;
			_isLowered = false;
		}

		private void Update()
		{
			if (Time.time >= _timeToReset && _timerIsSet)
			{
				ResetVol();
				_timerIsSet = false;
			}

			_timeToCheckIdleAudioSource += Time.deltaTime;
			if (_timeToCheckIdleAudioSource > TIME_TO_CHECK_IDLE_AUDIO_SOURCE)
			{
				var audios = GetComponents<AudioSource>();
				foreach (var a in audios)
				{
					if (!a.isPlaying)
						Destroy(a);
				}

				_timeToCheckIdleAudioSource = 0;
			}
		}

		#endregion
		
		
	
	
	}

}