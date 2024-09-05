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
        // 16비트로 변환
        int targetBitDepth = 16;

        ConvertWavBitDepth(inputFilePath, outputFilePath, targetBitDepth);
        Console.WriteLine($"비트 깊이 {targetBitDepth}비트로 변환 완료: {outputFilePath}");
    }

    static void ConvertWavBitDepth(string inputFilePath, string outputFilePath, int targetBitDepth)
    {
        // 원본 파일 읽기
        byte[] wavFile = File.ReadAllBytes(inputFilePath);

        // WAV 파일의 헤더 정보 추출
        int bitDepthOffset = 34; // 34번째 바이트에 비트 깊이 정보가 있음
        int currentBitDepth = BitConverter.ToInt16(wavFile, bitDepthOffset);

        if (currentBitDepth == targetBitDepth)
        {
            Console.WriteLine("비트 깊이가 이미 동일합니다.");
            return;
        }

        // 헤더에서 샘플링 레이트, 채널 수 등 필요한 정보 추출
        int sampleRate = BitConverter.ToInt32(wavFile, 24); // 24번째 바이트에 샘플링 레이트가 있음
        int numChannels = BitConverter.ToInt16(wavFile, 22); // 22번째 바이트에 채널 정보가 있음

        // 비트 깊이를 새로 설정
        byte[] newWavFile = new byte[wavFile.Length];
        Array.Copy(wavFile, newWavFile, wavFile.Length); // 원본 복사
        Array.Copy(BitConverter.GetBytes((short)targetBitDepth), 0, newWavFile, bitDepthOffset, 2);

        // 데이터 영역 변환 (기존 샘플을 새 비트 깊이에 맞게 변환)
        int dataOffset = 44; // 데이터는 44번째 바이트부터 시작
        int currentSampleSize = currentBitDepth / 8;
        int targetSampleSize = targetBitDepth / 8;
        int sampleCount = (wavFile.Length - dataOffset) / currentSampleSize;

        // 새로운 비트 깊이에 맞는 데이터를 변환하여 저장
        for (int i = 0; i < sampleCount; i++)
        {
            int oldSampleIndex = dataOffset + i * currentSampleSize;
            int newSampleIndex = dataOffset + i * targetSampleSize;

            // 원본 샘플을 읽고 새 비트 깊이에 맞게 변환
            short sample = BitConverter.ToInt16(wavFile, oldSampleIndex);
            short newSample = (short)(sample * (Math.Pow(2, targetBitDepth) / Math.Pow(2, currentBitDepth)));

            Array.Copy(BitConverter.GetBytes(newSample), 0, newWavFile, newSampleIndex, targetSampleSize);
        }

        // 변환된 WAV 파일 저장
        File.WriteAllBytes(outputFilePath, newWavFile);
    }
}