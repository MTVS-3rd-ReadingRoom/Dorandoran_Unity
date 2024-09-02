using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public class Sound
{
    public enum AudioClipName
    {
        None,
        DefaultBackground,
        Basicattack1, Basicattack2, Basicattack3,
        Dash,
        RSkill_Start, RSkill_Fire,
        QSkill,
        CSkill,
        TSkill_Ready, TSkill_Active,
        PlayerStep1,
        Boss_Talk_Default,
        Boss_Talk_Skill,
        Boss_Die,
        Boss_BasicAttack,
        Boss_BookSpawn,
        Boss_NormalSkill1,
        Boss_NormalSkill2_Start,
        Boss_MainSkill1,
        Boss_MainSkill2,
        Boss_MainSkill3_Start,
        Boss_MainSkill3_Throw,
        Boss_SpawnMinion,
        Boss_Explosion,
        PlayerUH,
        PlayerHit,
        BossHit,
        BossStart,
        BossBGM,
        Door_SFX,
        Enemy_Sword,
        UI_Sound,
        Bass808,
        BloodDrain_Start,
        ESkill_1Combo,
        ESkill_3Combo,
        Ending,
        InplaceBGM,
        StartCut,
        BloodDrain_End,
        Light,
        Fire,
        Rock,
        Explosion,
    }
    public AudioClipName audioClipName;
    public AudioClip audioClip
    {
        get => audioClips[Random.Range(0, audioClips.Length)];
    }
    public AudioClip[] audioClips;
}

public class SoundManager : MonoBehaviour
{
    // 싱글턴
    public static SoundManager instance { get; private set; }
    
    // 효과음 AudioSource Prefab
    public SFXAudioSource prefab_SFXAudioSource;
    // 배경음악 AudioSource
    private AudioSource bgmAudioSource;

    [SerializeField]
    private Sound[] sounds;

    public Dictionary<string, Sound.AudioClipName> enumToString = new Dictionary<string, Sound.AudioClipName>(); 

    public Dictionary<Sound.AudioClipName, Sound> soundDictionary = new Dictionary<Sound.AudioClipName, Sound>();

    // 효과음 AudioSource 오브젝트 리스트
    private Queue<SFXAudioSource> sfxAudioSources = new Queue<SFXAudioSource>();
    // 풀링을 위한 비활성화 효과음 AudioSource 오브젝트 리스트
    private Queue<SFXAudioSource> inactiveSFXAudioSources = new Queue<SFXAudioSource>();

    // 효과음 볼륨
    public float sfxVolume = 0.5f;
    public float bgmVolume = 0.5f;

    SFXAudioSource source;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);

        DontDestroyOnLoad(this);
        bgmAudioSource = GetComponent<AudioSource>();
        bgmAudioSource.volume = bgmVolume;
        InitSounds();
    }
    
    // 인스펙터 창에서 받아온 사운드를 Dictionary로 정리
    private void InitSounds()
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            soundDictionary.Add(sounds[i].audioClipName, sounds[i]);
            enumToString.Add(sounds[i].audioClipName.ToString(), sounds[i].audioClipName);
        }
    }


    // 효과음 볼륨 변경
    public void ChangeBGMVolum(float voluem)
    {
        bgmVolume = Mathf.Clamp(voluem, 0, 1);

        foreach (SFXAudioSource item in sfxAudioSources)
        {
            item.SetVolume(bgmVolume);
        }
    }

    // 효과음 볼륨 변경
    public void ChangeSFXVolum(float voluem)
    {
        sfxVolume = Mathf.Clamp(voluem, 0, 1);

        foreach (SFXAudioSource item in sfxAudioSources)
        {
            item.SetVolume(sfxVolume);
        }
    }

    public void ActiveBGM(Sound.AudioClipName audioClipName)
    {
        bgmAudioSource.Stop();
        if (audioClipName != Sound.AudioClipName.None)
        {
            bgmAudioSource.PlayOneShot(soundDictionary[audioClipName].audioClip);
        }
        else
        {
            bgmAudioSource.Stop();
        }
    }

    // 효과음 audioSource 활성화
    public SFXAudioSource ActiveOnShotSFXSound(Sound.AudioClipName audioClipName, Transform target, Vector3 position, float spatial = 1)
    {
        // 오디오 타입이 오지 않았다면 return
        if (audioClipName == 0)
        {
            return null;
        }

        // 풀에 오브젝트가 없으면 생성
        if (inactiveSFXAudioSources.Count == 0)
        {
            source = Instantiate(prefab_SFXAudioSource);
            source.transform.SetParent(transform);
            source.SetVolume(sfxVolume);
            sfxAudioSources.Enqueue(source);
        }
        else // 풀에 오브젝트가 있으면 액티브
        {
            source = inactiveSFXAudioSources.Dequeue();
            source.SetVolume(sfxVolume);
            source.gameObject.SetActive(true);
        }
        source.audioSource.spatialBlend = spatial;
        if (target == null)
        {
            source.transform.position = position;
        }
        source.ActiveSound(soundDictionary[audioClipName], target);

        return source;
    }

    //  풀링을 위한 효과음 오브젝트 비활성화
    public void InactiveSFXSound(SFXAudioSource sfxAudioSource)
    {
        sfxAudioSource.StopSound_FadeOut();
        sfxAudioSource.transform.SetParent(transform);
        sfxAudioSource.gameObject.SetActive(false);
        inactiveSFXAudioSources.Enqueue(sfxAudioSource);

        if(inactiveSFXAudioSources.Count > 100)
        {
            for (int i = 0; i < 50; i++)
            {
                Destroy(inactiveSFXAudioSources.Dequeue().gameObject);
            }
        }
    }

    public void FadeOut_BGM()
    {
        StartCoroutine(Coroutine_FadeOut_BGM());
    }

    private IEnumerator Coroutine_FadeOut_BGM()
    {
        while (bgmAudioSource.volume > 0)
        {
            bgmAudioSource.volume -= 0.01f;
            yield return new WaitForSeconds(0.02f);
        }

        bgmAudioSource.Stop();
    }

}

