using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;

public static class WavUtility
{
    public static byte[] ConvertAudioClipToWav(AudioClip clip)
    {
        // Get audio data as float array
        float[] samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 0);

        // Convert float samples [-1..1] to 16-bit PCM
        byte[] pcmData = new byte[samples.Length * 2];
        int rescaleFactor = 32767; // to convert float to Int16

        for (int i = 0; i < samples.Length; i++)
        {
            short intData = (short)(samples[i] * rescaleFactor);
            pcmData[i * 2] = (byte)(intData & 0xFF);
            pcmData[i * 2 + 1] = (byte)((intData >> 8) & 0xFF);
        }

        // Create WAV header
        byte[] wav = WriteWavHeader(pcmData.Length, clip.channels, clip.frequency);

        // Combine header and PCM data
        byte[] wavBytes = new byte[wav.Length + pcmData.Length];
        Buffer.BlockCopy(wav, 0, wavBytes, 0, wav.Length);
        Buffer.BlockCopy(pcmData, 0, wavBytes, wav.Length, pcmData.Length);

        return wavBytes;
    }

    public static byte[] WriteWavHeader(int pcmDataLength, int channels, int sampleRate)
    {
        int headerSize = 44;
        int fileSize = pcmDataLength + headerSize - 8;

        using (var stream = new MemoryStream())
        using (var writer = new BinaryWriter(stream))
        {
            // RIFF header
            writer.Write(new char[] { 'R', 'I', 'F', 'F' });
            writer.Write(fileSize);
            writer.Write(new char[] { 'W', 'A', 'V', 'E' });

            // fmt chunk
            writer.Write(new char[] { 'f', 'm', 't', ' ' });
            writer.Write(16); // Sub-chunk size (16 for PCM)
            writer.Write((short)1); // Audio format (1 = PCM)
            writer.Write((short)channels);
            writer.Write(sampleRate);
            writer.Write(sampleRate * channels * 2); // Byte rate
            writer.Write((short)(channels * 2)); // Block align
            writer.Write((short)16); // Bits per sample

            // data chunk
            writer.Write(new char[] { 'd', 'a', 't', 'a' });
            writer.Write(pcmDataLength);

            writer.Flush();
            return stream.ToArray();
        }
    }
}