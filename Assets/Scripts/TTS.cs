using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpeechLib;

public class TTS : MonoBehaviour
{
    public string fileName;
    public AudioSource audioSource;
    private AudioClip audioClip;

    public int currentVoiceIndex;
    private SpVoice voice;
    private ISpeechObjectTokens voices;
    private SpFileStream objFSTRM;

    void Start()
    {
        objFSTRM = new SpFileStream();
        voice = new SpVoice();
        voices = voice.GetVoices();
        Debug.Log(voices.Item(currentVoiceIndex).GetDescription());
    }


    public void speak(string text)
    {
        Debug.Log(voices.Item(currentVoiceIndex).GetDescription() +": "+ text);
        voice.Voice=voices.Item(currentVoiceIndex);
        objFSTRM.Open(Application.streamingAssetsPath + "/" + fileName, SpeechStreamFileMode.SSFMCreateForWrite, false);
        voice.AudioOutputStream = objFSTRM;
        voice.Speak(text);
        objFSTRM.Close();
        StartCoroutine(LoadAudio());
    }

    private IEnumerator LoadAudio()
    {
        WWW request = GetAudioFromFile();
        yield return request;
        audioClip = request.GetAudioClip();
        PlayAudio();
    }

    private WWW GetAudioFromFile()
    {
        return new WWW("file://" + Application.streamingAssetsPath + "/" + fileName);
    }

    private void PlayAudio()
    {
        audioSource.clip = audioClip;
        audioSource.Play();
    }
}
