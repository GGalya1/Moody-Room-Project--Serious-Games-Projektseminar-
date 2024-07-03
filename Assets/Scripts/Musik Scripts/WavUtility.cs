using System;
using System.IO;
using UnityEngine;

public static class WavUtility
{
    // Convert AudioClip to byte array
    public static byte[] FromAudioClip(AudioClip audioClip)
    {
        float[] samples = new float[audioClip.samples * audioClip.channels];
        audioClip.GetData(samples, 0);

        byte[] byteArray = new byte[samples.Length * sizeof(float)];
        Buffer.BlockCopy(samples, 0, byteArray, 0, byteArray.Length);
        return byteArray;
    }

    // Convert byte array to AudioClip
    public static AudioClip ToAudioClip(byte[] byteArray, string clipName)
    {
        float[] samples = new float[byteArray.Length / sizeof(float)];
        Buffer.BlockCopy(byteArray, 0, samples, 0, byteArray.Length);

        AudioClip audioClip = AudioClip.Create(clipName, samples.Length / 2, 2, 44100, false);
        audioClip.SetData(samples, 0);
        return audioClip;
    }
}
