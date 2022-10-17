using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameOverUI : StateUI
{
    Label perish;
    public VisualTreeAsset GameOverTitle;
    public VisualTreeAsset GameWonTitle;

    public AudioClip WonClip;
    public AudioSource localAudioSource;

    public override void OnCreate()
    {
        UIDocument doc = GetComponent<UIDocument>();
        if (StateManager.g.PlayerWon)
        {
            doc.visualTreeAsset = GameWonTitle;
            localAudioSource.clip = WonClip;
        }


        localAudioSource.Play();
        perish = doc.rootVisualElement.Q<Label>("perish-title");
        perish.style.opacity = 0;
        perish.style.translate = new StyleTranslate(new Translate(0,-200,0));
        doc.rootVisualElement.Q<Button>("continue-button").clicked += Button_Continue;
        StartCoroutine(FadeIn());
    }


    public void Button_Continue()
    {
        PlayButtonClick();
        StateManager.g.PopState();
        StateManager.g.PushState(StateManager.States.InGame);
    }

    IEnumerator FadeIn()
    {
        float value = 0;

        while (value < 1)
        {
            value += 0.2f;
            perish.style.opacity = value;
            perish.style.translate = new StyleTranslate(new Translate(0, Mathf.Lerp(-200,0,value * 0.7f), 0));
            yield return new WaitForSeconds(0.1f);
        }

        value = 1;
        perish.style.translate = new StyleTranslate(new Translate(0, Mathf.Lerp(100,0,value), 0));
        perish.style.opacity = 1;
    }


    public void PlayButtonClick()
    {
        StateManager.g.globalAudioSource.PlayOneShot(Resources.Load<AudioClip>("AudioClips/SFX/MenuContinue"));
    }
}
