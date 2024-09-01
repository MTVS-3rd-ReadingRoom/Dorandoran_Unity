using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
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

    public int recordTime = 30;
    public bool myTurn = false;
    public int count = 5;

    public int microphoneIndex = 0;
    public AudioClip audioClip1;
    public AudioClip audioClip2;

    public string theme = "";

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

    public void SaveAudioClip(AudioClip audioClip)
    {
        SavWav.Save(audioClip.name, audioClip);
    }

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

    public void RecordMicrophone()
    {
        print("Start Record");
        StartCoroutine(Corr_Record());
    }

    private IEnumerator Corr_Record()
    {
        AudioClip record = Microphone.Start(Microphone.devices[microphoneIndex].ToString(), false, recordTime, 44100);
        yield return new WaitForSeconds(recordTime + 1);
        audioClip1 = record;
        SaveAudioClip(record);
        //AudioClipToByteArray(record);
    }
}
