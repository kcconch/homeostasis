using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class FadeInOut : MonoBehaviour
{
    public Image FadeImg;
    public float fadeSpeed = 1.0f;
    public bool sceneStarting = true;
    private float timer = 0.0f;
    private readonly float waitTime = 4.0f;
    private readonly float durationTime = 60.0f;


    void Awake()
    {
        FadeImg.rectTransform.localScale = new Vector2(Screen.width, Screen.height);
    }

    void Update()
    {
        timer += Time.deltaTime;
        // Debug.Log(timer);

        // If the scene is starting...
        if (sceneStarting && timer > waitTime)
            // ... call the StartScene function.
            StartScene();
            

            if (timer > durationTime)
        {
            EndScene();
        }
    }


    void FadeToClear()
    {
        // Lerp the colour of the image between itself and transparent.
        FadeImg.color = Color.Lerp(FadeImg.color, Color.clear, fadeSpeed * Time.deltaTime);
    }


    void FadeToBlack()
    {
        // Lerp the colour of the image between itself and black.
        FadeImg.color = Color.Lerp(FadeImg.color, Color.black, fadeSpeed * Time.deltaTime);
    }


    void StartScene()
    {
        // Fade the texture to clear.
        FadeToClear();

        // If the texture is almost clear...
        if (FadeImg.color.a <= 0.05f)
        {
            // ... set the colour to clear and disable the RawImage.
            FadeImg.color = Color.clear;
            FadeImg.enabled = false;

            // The scene is no longer starting.
            sceneStarting = false;
        }

    }

    void EndScene()
    {
        FadeImg.enabled = true;
        FadeToBlack();
    }



    //public IEnumerator EndSceneRoutine(int SceneNumber)
    //{
    //    // Make sure the RawImage is enabled.
    //    FadeImg.enabled = true;
    //    do
    //    {
    //        // Start fading towards black.
    //        FadeToBlack();

    //        // If the screen is almost black...
    //        if (FadeImg.color.a >= 0.95f)
    //        {
    //            // ... reload the level
    //            SceneManager.LoadScene(SceneNumber);
    //            yield break;
    //        }
    //        else
    //        {
    //            yield return null;
    //        }
    //    } while (true);
    //}

    //public void EndScene(int SceneNumber)
    //{
    //    sceneStarting = false;
    //    StartCoroutine("EndSceneRoutine", SceneNumber);
    //}
}