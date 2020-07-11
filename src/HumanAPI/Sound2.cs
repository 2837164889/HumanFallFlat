using System;
using UnityEngine;

namespace HumanAPI
{
	public class Sound2 : MonoBehaviour
	{
		public string group;

		public string sample;

		public SoundLibrarySample localSample;

		internal SoundLibrary.SerializedSample soundSample;

		public bool useMaster;

		public char currentChoice = 'A';

		public bool startOnLoad;

		public AudioChannel channel = AudioChannel.Effects;

		public float rtVolume = 1f;

		public float rtPitch = 1f;

		public float baseVolume = 1f;

		public float basePitch = 1f;

		[NonSerialized]
		private float mute = 1f;

		public float maxDistance = 30f;

		public float falloffStart = 1f;

		public float falloffPower = 0.5f;

		public float lpStart = 2f;

		public float lpPower = 0.5f;

		public float spreadNear = 1f;

		public float spreadFar;

		public float spatialNear = 0.5f;

		public float spatialFar = 1f;

		private bool isSourcePaused;

		private AudioSource activeSource;

		private AudioSource inactiveSource;

		internal SoundLibrary.SerializedClip activeClip;

		private SoundLibrary.SerializedClip inactiveClip;

		private SoundLibrary.SerializedClip queuedClip;

		private float activeVolume;

		private float inactiveVolume;

		private float activePitch;

		private float inactivePitch;

		private bool activeLoop;

		private bool fadeQueued;

		private float fadeInTime;

		private float fadeInDelay;

		private float fadeInDuration;

		private float fadeOutTime;

		private float fadeOutDuration;

		private float pauseVol = 1f;

		private float pauseSpeed;

		private SoundLibrary.SerializedClip currentClip;

		private float lowPass = 22000f;

		public string fullName => group + ":" + base.name;

		public string sampleLabel => (soundSample == null) ? ("MISS:" + sample) : (soundSample.category + ":" + sample);

		public bool isPlaying
		{
			get
			{
				if (activeClip == null || (activeSource == null && !fadeQueued))
				{
					return false;
				}
				if (isSourcePaused)
				{
					return true;
				}
				if (fadeInDuration != 0f || fadeInDelay != 0f)
				{
					return true;
				}
				if (activeSource.loop)
				{
					return true;
				}
				if (activeSource.isPlaying && activeSource.clip != null && activeSource.clip.length > 10f)
				{
					return true;
				}
				return false;
			}
		}

		private bool processUpdate => fadeInDelay != 0f || fadeInDuration != 0f || fadeOutDuration != 0f || pauseSpeed != 0f;

		private void OnDestroy()
		{
			if (soundSample != null)
			{
				soundSample.Unsubscribe(OnSampleLoaded);
			}
		}

		protected virtual void OnSampleLoaded()
		{
			if (isPlaying)
			{
				SoundLibrary.SerializedClip clip = soundSample.GetClip(currentClip, currentChoice, SoundLibrary.SampleContainerChildType.Loop);
				if (clip != null)
				{
					CrossfadeSound(clip, loop: true, 0f, 0.1f);
				}
			}
			else if (startOnLoad)
			{
				Play(forceLoop: true);
			}
		}

		public void SetSample(SoundLibrary.SerializedSample sample)
		{
			if (soundSample != null)
			{
				soundSample.Unsubscribe(OnSampleLoaded);
			}
			this.sample = sample?.name;
			soundSample = sample;
			if (soundSample != null)
			{
				soundSample.Subscribe(OnSampleLoaded);
			}
		}

		public void RefreshSampleParameters()
		{
			if (soundSample != null)
			{
				if (activeClip != null)
				{
					activeVolume = activeClip.volume * soundSample.volume;
					activePitch = activeClip.pitch * soundSample.pitch;
				}
				if (inactiveClip != null)
				{
					inactiveVolume = inactiveClip.volume * soundSample.volume;
					inactivePitch = inactiveClip.pitch * soundSample.pitch;
				}
				ApplyPitch();
				ApplyVolume();
			}
		}

		public void PlayOneShot(float volume = 1f, float pitch = 1f, float delay = 0f)
		{
			PlayOneShot(Vector3.zero, volume, pitch, delay);
		}

		public void PlayOneShot(Vector3 pos, float volume = 1f, float pitch = 1f, float delay = 0f)
		{
			if (soundSample != null)
			{
				SoundLibrary.SerializedClip clip = soundSample.GetClip(null, currentChoice, SoundLibrary.SampleContainerChildType.None);
				if (clip != null)
				{
					PlayOneShot(clip, pos, volume, pitch, delay);
				}
			}
		}

		public void Switch(char choice, bool crossfade = true)
		{
			if (currentChoice == choice)
			{
				return;
			}
			currentChoice = choice;
			if (soundSample != null && isPlaying && crossfade)
			{
				SoundLibrary.SerializedClip serializedClip = currentClip = soundSample.GetClip(currentClip, currentChoice, SoundLibrary.SampleContainerChildType.Loop);
				if (serializedClip != null)
				{
					CrossfadeSound(serializedClip, loop: true, 0f, soundSample.crossFade);
				}
			}
		}

		public void Play(bool forceLoop = false)
		{
			startOnLoad = true;
			if (soundSample == null || !soundSample.loaded)
			{
				return;
			}
			SoundLibrary.SerializedClip serializedClip = currentClip = soundSample.GetClip(null, currentChoice, SoundLibrary.SampleContainerChildType.Start);
			if (currentClip != null)
			{
				PlaySound(currentClip, forceLoop && !soundSample.isLoop);
			}
			if (forceLoop || soundSample.isLoop)
			{
				serializedClip = soundSample.GetClip(currentClip, currentChoice, SoundLibrary.SampleContainerChildType.Loop);
				if (serializedClip != null)
				{
					CrossfadeSound(serializedClip, loop: true, 0f, soundSample.crossFade);
				}
			}
		}

		public void Stop()
		{
			startOnLoad = false;
			if (soundSample != null)
			{
				SoundLibrary.SerializedClip clip = soundSample.GetClip(currentClip, currentChoice, SoundLibrary.SampleContainerChildType.Stop);
				if (clip != null)
				{
					CrossfadeSound(clip, loop: false, 0f, soundSample.crossFade);
				}
				else
				{
					FadeOut(soundSample.crossFade);
				}
			}
		}

		public void PlayClip(AudioClip clip, float volume, float pitch, bool loop)
		{
			soundSample = new SoundLibrary.SerializedSample
			{
				pB = 1f,
				vB = 1f
			};
			PlaySound(new SoundLibrary.SerializedClip
			{
				clip = clip,
				pB = pitch,
				vB = volume
			}, loop);
		}

		public void PlaySound(SoundLibrary.SerializedClip clip, bool loop)
		{
			FadeIn(clip, loop, 0f, 0f);
		}

		public void CrossfadeClip(AudioClip clip, float volume, float pitch, bool loop, float after, float duration, float outDuration, float inDuration)
		{
			soundSample = new SoundLibrary.SerializedSample
			{
				pB = 1f,
				vB = 1f
			};
			CrossfadeSound(new SoundLibrary.SerializedClip
			{
				clip = clip,
				pB = pitch,
				vB = volume
			}, loop, after, duration, outDuration, inDuration);
		}

		public void CrossfadeSound(SoundLibrary.SerializedClip clip, bool loop, float after, float duration)
		{
			FadeOut(duration);
			FadeIn(clip, loop, 0f, duration);
		}

		public void CrossfadeSound(SoundLibrary.SerializedClip clip, bool loop, float after, float duration, float outDuration, float inDuration)
		{
			FadeOut(outDuration);
			FadeIn(clip, loop, duration - inDuration, inDuration);
		}

		public void PlayOneShot(SoundLibrary.SerializedClip clip, Vector3 pos, float volume = 1f, float pitch = 1f, float delay = 0f)
		{
			AudioSource source = AcquireAudioSource(pos);
			source.clip = clip.clip;
			source.volume = volume * clip.volume * soundSample.volume * baseVolume * rtVolume * pauseVol * mute;
			source.pitch = pitch * clip.pitch * soundSample.pitch * basePitch * rtPitch;
			source.loop = false;
			if (delay == 0f)
			{
				source.Play();
			}
			else
			{
				source.PlayDelayed(delay);
			}
			ReleaseAudioSource(ref source, waitToFinish: true);
		}

		public AudioSource GetActiveSource()
		{
			return activeSource;
		}

		public void FadeOut(float duration)
		{
			if (activeClip != null)
			{
				if (fadeQueued)
				{
					if (activeClip != null)
					{
						OnClipDeactivated(activeClip);
					}
					fadeQueued = false;
					activeClip = null;
				}
				else
				{
					if (inactiveClip != null)
					{
						ReleaseAudioSource(ref inactiveSource, waitToFinish: false);
					}
					if (inactiveClip != null)
					{
						OnClipDeactivated(inactiveClip);
					}
					inactiveSource = activeSource;
					inactiveClip = activeClip;
					inactivePitch = activePitch;
					inactiveVolume = activeVolume;
					activeClip = null;
					activeSource = null;
					if (fadeInDuration > 0f)
					{
						inactiveVolume *= fadeInTime / fadeInDuration;
					}
					fadeOutTime = 0f;
					fadeOutDuration = duration;
				}
			}
			if (inactiveClip != null)
			{
				if (duration == 0f)
				{
					ReleaseAudioSource(ref inactiveSource, waitToFinish: false);
					OnClipDeactivated(inactiveClip);
					inactiveClip = null;
				}
				else if (fadeOutDuration - fadeOutTime > duration)
				{
					inactiveVolume *= 1f - fadeOutTime / duration;
					fadeOutTime = 0f;
					fadeOutDuration = duration;
				}
			}
			fadeInDelay = 0f;
			fadeInDuration = 0f;
			fadeInTime = 0f;
		}

		public virtual void OnClipDeactivated(SoundLibrary.SerializedClip clip)
		{
		}

		public virtual void OnClipActivating(SoundLibrary.SerializedClip clip)
		{
		}

		public void FadeIn(SoundLibrary.SerializedClip clip, bool loop, float after, float duration)
		{
			if (activeClip != null)
			{
				OnClipDeactivated(activeClip);
			}
			fadeQueued = true;
			activeClip = clip;
			OnClipActivating(activeClip);
			activeVolume = clip.volume * soundSample.volume;
			activePitch = clip.pitch * soundSample.pitch;
			activeLoop = loop;
			fadeInDelay = after;
			fadeInDuration = duration;
			fadeInTime = 0f;
			HandleFadeIn();
		}

		private void HandleFadeIn()
		{
			fadeInTime += Time.unscaledDeltaTime;
			if (fadeQueued && fadeInDelay <= fadeInTime)
			{
				fadeQueued = false;
				fadeInTime -= fadeInDelay;
				fadeInDelay = 0f;
				float num = (fadeInDuration != 0f) ? (fadeInTime / fadeInDuration) : 1f;
				if (num >= 1f)
				{
					fadeInDuration = 0f;
					num = 1f;
				}
				if (activeSource == null)
				{
					activeSource = AcquireAudioSource(Vector3.zero);
				}
				activeSource.clip = activeClip.clip;
				activeSource.volume = Mathf.Sqrt(num) * activeVolume * baseVolume * rtVolume * Mathf.Sqrt(pauseVol) * mute;
				activeSource.pitch = activePitch * basePitch * rtPitch;
				activeSource.loop = activeLoop;
				activeSource.Play();
			}
			else if (activeSource != null)
			{
				float num2 = (fadeInDuration != 0f) ? (fadeInTime / fadeInDuration) : 1f;
				if (num2 >= 1f)
				{
					fadeInDuration = 0f;
					num2 = 1f;
				}
				activeSource.volume = Mathf.Sqrt(num2) * activeVolume * baseVolume * rtVolume * Mathf.Sqrt(pauseVol) * mute;
				activeSource.pitch = activePitch * basePitch * rtPitch;
			}
		}

		private void HandleFadeOut()
		{
			fadeOutTime += Time.unscaledDeltaTime;
			if (inactiveSource == null)
			{
				return;
			}
			float num = (fadeOutDuration != 0f) ? (fadeOutTime / fadeOutDuration) : 1f;
			if (num >= 1f)
			{
				fadeInDuration = 0f;
				num = 1f;
				ReleaseAudioSource(ref inactiveSource, waitToFinish: false);
				if (inactiveClip != null)
				{
					OnClipDeactivated(inactiveClip);
				}
				inactiveClip = null;
			}
			else
			{
				inactiveSource.volume = Mathf.Sqrt(1f - num) * inactiveVolume * baseVolume * rtVolume * Mathf.Sqrt(pauseVol) * mute;
				inactiveSource.pitch = inactivePitch * basePitch * rtPitch;
			}
		}

		public void Pause()
		{
			pauseSpeed = -1f;
		}

		public void Resume()
		{
			pauseSpeed = 0.33333334f;
			if (pauseVol == 0f && isSourcePaused)
			{
				isSourcePaused = false;
				activeSource.UnPause();
			}
		}

		private void HandlePause()
		{
			if (pauseSpeed == 0f)
			{
				return;
			}
			pauseVol = Mathf.Clamp01(pauseVol + pauseSpeed * Time.unscaledDeltaTime);
			if (pauseVol == 0f)
			{
				ReleaseAudioSource(ref inactiveSource, waitToFinish: false);
				if (inactiveClip != null)
				{
					OnClipDeactivated(inactiveClip);
				}
				inactiveClip = null;
				if (activeSource != null && activeSource.isPlaying)
				{
					activeSource.Pause();
					isSourcePaused = true;
				}
				pauseSpeed = 0f;
			}
			else if (pauseVol == 1f)
			{
				pauseSpeed = 0f;
			}
		}

		public void StopSound()
		{
			if (base.name == "Underwater")
			{
				base.name = "Underwater";
			}
			FadeOut(0f);
		}

		protected virtual void Update()
		{
			if (processUpdate)
			{
				HandlePause();
				HandleFadeIn();
				HandleFadeOut();
			}
		}

		public void Mute(bool mute)
		{
			this.mute = ((!mute) ? 1 : 0);
			ApplyVolume();
		}

		public void SetPitch(float newPitch)
		{
			if (rtPitch != newPitch)
			{
				rtPitch = newPitch;
				ApplyPitch();
			}
		}

		public void SetBasePitch(float newPitch)
		{
			if (basePitch != newPitch)
			{
				basePitch = newPitch;
				ApplyPitch();
			}
		}

		private void ApplyPitch()
		{
			if (!processUpdate)
			{
				if (activeSource != null)
				{
					activeSource.pitch = activePitch * basePitch * rtPitch;
				}
				if (inactiveSource != null)
				{
					inactiveSource.pitch = inactivePitch * basePitch * rtPitch;
				}
			}
		}

		public void SetVolume(float newVolume)
		{
			if (rtVolume != newVolume)
			{
				rtVolume = newVolume;
				ApplyVolume();
			}
		}

		public virtual void SetBaseVolume(float newVolume)
		{
			if (baseVolume != newVolume)
			{
				baseVolume = newVolume;
				ApplyVolume();
			}
		}

		private void ApplyVolume()
		{
			if (!processUpdate)
			{
				if (activeSource != null)
				{
					activeSource.volume = 1f * activeVolume * baseVolume * rtVolume * Mathf.Sqrt(pauseVol) * mute;
				}
				if (inactiveSource != null)
				{
					inactiveSource.volume = 0f * inactiveVolume * baseVolume * rtVolume * Mathf.Sqrt(pauseVol) * mute;
				}
			}
		}

		internal void SetLowPass(float frequency)
		{
			lowPass = frequency;
			SetLowPass(activeSource, lowPass);
			SetLowPass(activeSource, lowPass);
		}

		private void SetLowPass(AudioSource source, float frequency)
		{
			if (source != null)
			{
				AudioLowPassFilter component = source.GetComponent<AudioLowPassFilter>();
				component.cutoffFrequency = frequency;
			}
		}

		private AudioSource AcquireAudioSource(Vector3 pos)
		{
			AudioSource audioSource = SoundSourcePool.instance.GetAudioSource();
			if (pos == Vector3.zero)
			{
				audioSource.transform.SetParent(base.transform, worldPositionStays: false);
				audioSource.transform.localPosition = Vector3.zero;
			}
			else
			{
				audioSource.transform.SetParent(null, worldPositionStays: false);
				audioSource.transform.localPosition = pos;
			}
			audioSource.outputAudioMixerGroup = AudioRouting.GetChannel(channel);
			ApplyAttenuation(audioSource);
			if (lowPass < 22000f)
			{
				SetLowPass(audioSource, lowPass);
			}
			return audioSource;
		}

		private void ReleaseAudioSource(ref AudioSource source, bool waitToFinish)
		{
			if (!(source != null))
			{
				return;
			}
			if (source.isPlaying)
			{
				if (waitToFinish && !source.loop)
				{
					SoundSourcePool.instance.ReleaseAudioSourceOnComplete(source);
				}
				else
				{
					source.Stop();
					SoundSourcePool.instance.ReleaseAudioSource(source);
				}
			}
			else
			{
				SoundSourcePool.instance.ReleaseAudioSource(source);
			}
			source = null;
		}

		public void ApplyAttenuation()
		{
			if (activeSource != null)
			{
				ApplyAttenuation(activeSource);
			}
			if (inactiveSource != null)
			{
				ApplyAttenuation(inactiveSource);
			}
		}

		private void ApplyAttenuation(AudioSource source)
		{
			source.maxDistance = maxDistance;
			source.rolloffMode = AudioRolloffMode.Custom;
			source.SetCustomCurve(AudioSourceCurveType.CustomRolloff, CalculateAttenuation.VolumeFalloffFromTo(1f, 0f, falloffStart, source.maxDistance, falloffPower));
			source.SetCustomCurve(AudioSourceCurveType.Spread, CalculateAttenuation.VolumeFalloffFromTo(spreadNear / 2f, spreadFar / 2f, falloffStart, source.maxDistance, falloffPower));
			source.SetCustomCurve(AudioSourceCurveType.SpatialBlend, CalculateAttenuation.VolumeFalloffFromTo(spatialNear, (spatialNear != 0f) ? 1 : 0, falloffStart, source.maxDistance, falloffPower));
			AudioLowPassFilter component = source.GetComponent<AudioLowPassFilter>();
			if (component != null)
			{
				component.customCutoffCurve = CalculateAttenuation.LowPassFalloff(lpStart / maxDistance, lpPower);
			}
		}
	}
}
