using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using System.IO;

class WavConverter
{

    public static void AudioWavConverter(string inputFilePath, string outputFilePath)
    {
        // 16��Ʈ�� ��ȯ
        int targetBitDepth = 16;

        ConvertWavBitDepth(inputFilePath, outputFilePath, targetBitDepth);
        Console.WriteLine($"��Ʈ ���� {targetBitDepth}��Ʈ�� ��ȯ �Ϸ�: {outputFilePath}");
    }

    static void ConvertWavBitDepth(string inputFilePath, string outputFilePath, int targetBitDepth)
    {
        // ���� ���� �б�
        byte[] wavFile = File.ReadAllBytes(inputFilePath);

        // WAV ������ ��� ���� ����
        int bitDepthOffset = 34; // 34��° ����Ʈ�� ��Ʈ ���� ������ ����
        int currentBitDepth = BitConverter.ToInt16(wavFile, bitDepthOffset);

        if (currentBitDepth == targetBitDepth)
        {
            Console.WriteLine("��Ʈ ���̰� �̹� �����մϴ�.");
            return;
        }

        // ������� ���ø� ����Ʈ, ä�� �� �� �ʿ��� ���� ����
        int sampleRate = BitConverter.ToInt32(wavFile, 24); // 24��° ����Ʈ�� ���ø� ����Ʈ�� ����
        int numChannels = BitConverter.ToInt16(wavFile, 22); // 22��° ����Ʈ�� ä�� ������ ����

        // ��Ʈ ���̸� ���� ����
        byte[] newWavFile = new byte[wavFile.Length];
        Array.Copy(wavFile, newWavFile, wavFile.Length); // ���� ����
        Array.Copy(BitConverter.GetBytes((short)targetBitDepth), 0, newWavFile, bitDepthOffset, 2);

        // ������ ���� ��ȯ (���� ������ �� ��Ʈ ���̿� �°� ��ȯ)
        int dataOffset = 44; // �����ʹ� 44��° ����Ʈ���� ����
        int currentSampleSize = currentBitDepth / 8;
        int targetSampleSize = targetBitDepth / 8;
        int sampleCount = (wavFile.Length - dataOffset) / currentSampleSize;

        // ���ο� ��Ʈ ���̿� �´� �����͸� ��ȯ�Ͽ� ����
        for (int i = 0; i < sampleCount; i++)
        {
            int oldSampleIndex = dataOffset + i * currentSampleSize;
            int newSampleIndex = dataOffset + i * targetSampleSize;

            // ���� ������ �а� �� ��Ʈ ���̿� �°� ��ȯ
            short sample = BitConverter.ToInt16(wavFile, oldSampleIndex);
            short newSample = (short)(sample * (Math.Pow(2, targetBitDepth) / Math.Pow(2, currentBitDepth)));

            Array.Copy(BitConverter.GetBytes(newSample), 0, newWavFile, newSampleIndex, targetSampleSize);
        }

        // ��ȯ�� WAV ���� ����
        File.WriteAllBytes(outputFilePath, newWavFile);
    }
}