using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Video;
using UnityEngine.XR;

[System.Serializable]
public struct AudioClip_Json
{
    public byte[] bins;
    public int channels;
    public int frequency;

    public AudioClip_Json(byte[] bins, int channels, int frequency)
    {
        this.bins = bins;
        this.channels = channels;
        this.frequency = frequency;
    }
}

public class DataManager : MonoBehaviour
{
    public static DataManager instance;

    public int serial_Room;
    public int recordTime = 30;
    public bool myTurn = false;

    public string nickName;

    public int microphoneIndex = 0;

    public Topic topic;
    public AudioClip topicClip;

    public Coroutine coroutine_Record;
    private AudioSource voiceRecord;


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
    }


    public string SaveAudioClip(AudioClip audioClip)
    {
        return SavWav.Save(audioClip.name, audioClip);
    }

    public string SaveAudioClip_Binary(AudioClip audioClip)
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        string saveFileName = "Voice";
        string savePath = Application.persistentDataPath + "/" + saveFileName;
        FileStream fileStream = new FileStream(savePath, FileMode.Create);
        binaryFormatter.Serialize(fileStream, audioClip);
        fileStream.Close();
        return saveFileName;
    }

    public byte[] LoadAudioClip(string filePath)
    {
        
        if (File.Exists(filePath))
        {
            byte[] data = File.ReadAllBytes(filePath);
            return data;
        }
        return null;
    }

    public AudioClip LoadWav(byte[] wavFileData, string clipName = "AudioClip")
    {
        // WAV ���� ��� �˻� �� �Ľ�
        int channels = BitConverter.ToInt16(wavFileData, 22);
        int sampleRate = BitConverter.ToInt32(wavFileData, 24);
        int bitDepth = BitConverter.ToInt16(wavFileData, 34);
        int dataStartIndex = 44; // �����ʹ� WAV ��� �ڿ��� ���� (44����Ʈ)

        // ����� �������� ���� �� ���
        int bytesPerSample = bitDepth / 8;
        int sampleCount = (wavFileData.Length - dataStartIndex) / bytesPerSample;

        // ����� �����͸� float �迭�� ��ȯ
        float[] audioData = new float[sampleCount];
        if (bitDepth == 16)
        {
            for (int i = 0; i < sampleCount; i++)
            {
                audioData[i] = BitConverter.ToInt16(wavFileData, dataStartIndex + i * bytesPerSample) / 32768f;
            }
        }
        else if (bitDepth == 8)
        {
            for (int i = 0; i < sampleCount; i++)
            {
                audioData[i] = (wavFileData[dataStartIndex + i] - 128) / 128f;
            }
        }
        else
        {
            Debug.LogError("�������� �ʴ� ��Ʈ ����: " + bitDepth);
            return null;
        }

        // AudioClip ����
        AudioClip audioClip = AudioClip.Create(clipName, sampleCount, channels, sampleRate, false);
        audioClip.SetData(audioData, 0);

        return audioClip;
    }

    #region �Ⱦ�
    // ����� Ŭ���� ����Ʈ �迭�� ��ȯ�ϱ�
    public void AudioClipToByteArray(AudioClip audioClip)
    {
        float[] data = new float[audioClip.samples * audioClip.channels];
        audioClip.GetData(data, 0);
        byte[] bins = new byte[data.Length * sizeof(short)];

        for (int i = 0; i < data.Length; i++)
        {
            float clampedSamples = Mathf.Clamp(data[i], -1.0f, 1.0f);
            short shortSamples = (short)(clampedSamples * short.MaxValue);
            byte[] bytes = BitConverter.GetBytes(shortSamples);
            Buffer.BlockCopy(bytes, 0, bins, i * sizeof(short), sizeof(short));
        }

        //HttpManager.instance.PostVoiceClip(new AudioClip_Json(bins, audioClip.channels, audioClip.frequency));

        //audioClip2 = FromBianryToAudioClip(bins, audioClip.channels, audioClip.frequency);
        //AudioSource audioSource = GetComponent<AudioSource>();
        //audioSource.clip = audioClip2;
        //audioSource.Play();
    }

    public AudioClip FromBianryToAudioClip(byte[] bins, int channels, int frequency)
    {
       
        // short �������� ��ȯ�� �����͸� ���� �迭 ����
        short[] shortData = new short[bins.Length / sizeof(short)];

        // byte �迭�� short �迭�� ��ȯ
        Buffer.BlockCopy(bins, 0, shortData, 0, bins.Length);

        // float �������� ��ȯ�� �����͸� ���� �迭 ����
        float[] floatData = new float[shortData.Length];

        // short �����͸� float�� ��ȯ
        for (int i = 0; i < shortData.Length; i++)
        {
            floatData[i] = shortData[i] / (float)short.MaxValue;
        }

        // AudioClip ����
        AudioClip audioClip = AudioClip.Create("GeneratedClip", floatData.Length / channels, channels, frequency, false);

        // AudioClip�� float ������ ����
        audioClip.SetData(floatData, 0);

        return audioClip;
    }
    #endregion

    public void SetTopic_Text(Topic topic)
    {
        this.topic = topic;
    }

    public void SetTopic_Voice(AudioClip audioClip)
    {
        topicClip = audioClip;


        ModeratorSound.instance.SpeakPlayer(audioClip);
        SaveAudioClip(audioClip);
        PlayAudio(audioClip);
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
    }

    public void PlayAudio(AudioClip audioClip)
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    private void VideoPlayer_prepareCompleted(VideoPlayer source)
    {
        // ���� �غ� �Ϸ� �� ���
        source.Play();
    }

    public void RecordMicrophone()
    {
        print("Start Record");
        coroutine_Record = StartCoroutine(Corr_RecordAndPost());
    }

    private IEnumerator Corr_RecordAndPost()
    {
        voiceRecord.clip = Microphone.Start(Microphone.devices[microphoneIndex].ToString(), false, recordTime, 44100);
        yield return new WaitForSeconds(recordTime + 1);

        HttpManager.instance.PostVoiceClip_FormData("test", "test_room",LoadAudioClip(SaveAudioClip(voiceRecord.clip)));
        //LoadWav(LoadAudioClip(SaveAudioClip(record)));
        //audioClip2 = LoadWav(LoadAudioClip(SaveAudioClip(record)));
        coroutine_Record = null;
    }

    public void StopRecord()
    {
        if(coroutine_Record != null)
        {
            StopCoroutine(coroutine_Record);
            voiceRecord.Stop();
            HttpManager.instance.PostVoiceClip_FormData("test", "test_room", LoadAudioClip(SaveAudioClip(voiceRecord.clip)));
        }
    }
}
