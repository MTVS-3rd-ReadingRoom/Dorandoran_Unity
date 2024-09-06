using System.Collections;
using UnityEngine;

public class SFXAudioSource : MonoBehaviour
{
    public AudioSource audioSource;
    private Sound sound;
    private Transform target;
    private Coroutine playCheck;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = false;
    }

    private void Update()
    {
        if(target != null)
        {
            transform.position = target.position;
        }
    }

    private void OnDisable()
    {
        target = null;
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        playCheck = null;
    }

    public void SetVolume(float volume)
    {
        audioSource.volume = volume;
    }

    public void ActiveSound(Sound sound, Transform target)
    {
        if (playCheck != null)
        {
            StopCoroutine(playCheck);
            playCheck = null;
        }
        this.target = target;
        this.sound = sound;
        audioSource.clip = sound.audioClip;
        audioSource.Play();

        playCheck = StartCoroutine(InactiveSFXSound());
    }


    public void StopSound_FadeOut()
    {
        StartCoroutine(FadeOut());
    }
    public void StopSound()
    {
        audioSource.Stop();
    }
    public void ReplayAudio()
    {
        audioSource.Play();
    }

    private IEnumerator InactiveSFXSound()
    {
        yield return new WaitForSeconds(audioSource.clip.length);
        SoundManager.instance.InactiveSFXSound(this);
        playCheck = null;
    }

    private IEnumerator FadeOut()
    {
        while(audioSource.volume > 0)
        {
            audioSource.volume -= 0.05f;
            yield return new WaitForSeconds(0.01f);
        }

        audioSource.Stop();
    }
}
