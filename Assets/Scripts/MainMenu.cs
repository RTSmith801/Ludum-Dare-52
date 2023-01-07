using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    GameManager gm;
    public AudioMixer audioMixer;
    int sceneIndex;
    public GameObject loadingScreen;
    public Slider loadingScreenSlider;
    public TextMeshProUGUI progressText;


    private void Awake()
    {
        gm = FindObjectOfType<GameManager>();
    }

    public void PlayGame()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        sceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        StartCoroutine(LoadAsynchronously(sceneIndex));
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ButtonSound()
    {
        gm.am.Play("ButtonSound");
    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("volume", Mathf.Log10(volume) * 20);
    }

    IEnumerator LoadAsynchronously (int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        loadingScreen.SetActive(true);

        while(!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);

            loadingScreenSlider.value = progress;
            progressText.text = progress * 100f + "%";

            yield return null;
        }
    }

}
